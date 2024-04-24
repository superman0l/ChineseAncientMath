using Fungus;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using System;
using static UnityEngine.GraphicsBuffer;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Collections.LowLevel.Unsafe;
using DG.Tweening;

public class AbacusManager : MonoBehaviour
{
    public static AbacusManager Instance { get; private set; }

    [Header("Gird_Info")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private Transform gridCenter;
    [SerializeField] private GridInfo gridInfo;
    [SerializeField] private float cellWidthSize;
    [SerializeField] private float cellHeightSize;
    [SerializeField] private float ballMoveSpeed = 2f;
    [SerializeField] private AbacusMap abacusMap;
    [SerializeField] public float duration = .2f; // 闪烁的持续时间

    [Header("Player_Info")]
    [SerializeField] private Vector2Int start;
    [SerializeField] private Vector2Int end;
    [SerializeField] private Player player;
    [Header("Prefab")]
    [SerializeField] private Transform ball;
    [SerializeField] private Transform walkableBackground;
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private GameObject cover;

    private Material originalMaterial; 
    private Vector2Int playerCurrentPos;
    private int[,] balls;                                           // 存储当前算珠位置 大小13*5-》 13列每列5珠
    private int selectedColumn;                                     // 当前操作选择列
    private List<Transform> ballTransform = new List<Transform>();  // 所有算珠位置 大小13*5
    private Coroutine[] ballCoroutine;  // 算珠的移动协程
    private List<int> columnNumber = new List<int>();               // 每列当前的数值

    private PathFinding pathFinding;
    private bool nowCoroutineIsStop = false;
    private Flowchart flowchart;

    public bool isComplete { get; private set; } = false;

    const string backLevel = "LevelPick";

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance");
        }
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetNumber(0);
        flowchart = GameObject.Find("FlowChart").GetComponent<Flowchart>();
        BlockSignals.OnBlockEnd += OnBlockEnd;
        originalMaterial = ball.gameObject.GetComponent<Renderer>().sharedMaterial;
        playerCurrentPos = start;
        pathFinding = new PathFinding(width, height, cellWidthSize, cellHeightSize, gridCenter.position, gridInfo);
        player.transform.position = pathFinding.GetGrid().GetWorldPosition(start.x, start.y);
        // LoadGridBackground();

        balls = new int[width, 5];    
        for (int i = 0; i < width; i++)// 算珠位置初始化
        {
            int random = UnityEngine.Random.Range(0, 10);
            if (i == 0) SetNumber(random);
            for(int j = 0; j < 5; j++)
            {
                balls[i, j] = abacusMap.pairs[random].balls[j];
                pathFinding.GetGrid().GetGridObject(i, abacusMap.pairs[random].balls[j]).becomeBarrier = true;
            }
            columnNumber.Add(random);
            /*balls[i, 0] = 0; balls[i, 1] = 1; balls[i, 2] = 2; balls[i, 3] = 3; balls[i, 4] = 8;
            pathFinding.GetGrid().GetGridObject(i, 0).becomeBarrier = true;
            pathFinding.GetGrid().GetGridObject(i, 1).becomeBarrier = true;
            pathFinding.GetGrid().GetGridObject(i, 2).becomeBarrier = true;
            pathFinding.GetGrid().GetGridObject(i, 3).becomeBarrier = true;
            pathFinding.GetGrid().GetGridObject(i, 8).becomeBarrier = true;*/
        }
        ballCoroutine = new Coroutine[width * 5];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < 5; j++) 
            {
                Vector3 spawnPos = pathFinding.GetGrid().GetWorldPosition(i, balls[i, j]);  // Transform创建
                Transform newBall = Instantiate(ball, spawnPos, Quaternion.identity);
                ballTransform.Add(newBall);
                // columnNumber.Add(0);
            }
        }
        selectedColumn = 0;

        flowchart.ExecuteBlock("EnterLevel2");
    }

    private void LoadGridBackground()
    {
        foreach (var node in gridInfo.NodeWalkable)
        {
            Instantiate(walkableBackground, pathFinding.GetGrid().GetWorldPosition(node.x, node.y) + Vector3.forward * 2, Quaternion.identity);
        }
    }

    public void AbacusChange(bool Add)
    {
        ChangeValue(Add);

        Move();
    }

    private void Move()
    {
        bool complete;
        List<GridNode> res = pathFinding.FindPath(playerCurrentPos.x, playerCurrentPos.y, end.x, end.y, out complete);
        Vector2Int endPoint = new Vector2Int(res[res.Count - 1].x, res[res.Count - 1].y);
        if(complete) // 迷宫完成
        {
            Debug.Log("Complete");
            Vector3[] endPath = new Vector3[res.Count];
            for (int i = 0; i < res.Count; i++) 
            {
                endPath[i] = pathFinding.GetGrid().GetWorldPosition(res[i].x, res[i].y);
            }
            StartCoroutine(player.UpdatePoints(endPath));
            isComplete = true;
            // Finish();
            return;
        }
        Debug.Log(res[res.Count - 1].x + " " + res[res.Count - 1].y);
        Vector2Int prePos = playerCurrentPos;
        int tempCol = res[0].x;
        Vector3[] path = new Vector3[res.Count];
        for (int i = 0; i < res.Count; i++) //
        {
            path[i] = pathFinding.GetGrid().GetWorldPosition(res[i].x, res[i].y);
        }
        for (int i = res.Count - 1; i >= 0; i--)
        {
            if(endPoint.x != res[i].x)
            {
                Array.Resize(ref path, i+1);
                if(path.Length > 1)
                    StartCoroutine(player.UpdatePoints(path));
                playerCurrentPos = new Vector2Int(res[i].x,res[i].y);
                Blink(selectedColumn, endPoint.x);
                selectedColumn = endPoint.x;
                // SetNumber(columnNumber[selectedColumn]);
                return;
            }
        }
    }

    private void Blink(int oldValue, int newValue)
    {
        Debug.Log("old:" + oldValue + " new:" + newValue);
        if (oldValue == newValue) return;
        for(int i = 0; i < 5; i++)
        {
            var renderer = ballTransform[oldValue * 5 + i].gameObject.GetComponent<Renderer>();
            var tween = renderer.material.DOFade(0.3f, duration).SetLoops(-1, LoopType.Yoyo);
            tween.Pause();
            tween.Kill();
            renderer.material = originalMaterial;
        }
        for (int i = 0; i < 5; i++)
        {
            ballTransform[newValue * 5 + i].gameObject.GetComponent<Renderer>().material.DOFade(0.3f, duration).SetLoops(-1, LoopType.Yoyo).Play();
        }
    }

    public void Finish()
    {
        flowchart.ExecuteBlock("Level2Done");
    }

    private void OnBlockEnd(Block block)
    {
        if (cover == null) return;
        if (block == flowchart.FindBlock("EnterLevel2"))
        {
            cover.SetActive(true);
            flowchart.ExecuteBlock("Level2Tip");
        }
        else if (block == flowchart.FindBlock("Level2Tip"))
        {
            cover.SetActive(false);
        }
        else if (block == flowchart.FindBlock("Level2Done"))
            SceneManager.LoadScene(backLevel);
    }

    private void ChangeValue(bool Add) 
    {
        int oldValue = columnNumber[selectedColumn], newValue;
        if (Add)
        {
            newValue = Math.Clamp(oldValue + 1, 0 , 9);
        }
        else
        {
            newValue = Math.Clamp(oldValue - 1, 0, 9);
        }
        if (newValue == oldValue) return;
        SetNumber(newValue);
        columnNumber[selectedColumn] = newValue;
        for (int i = 0; i < 5; i++)
        {
            if (abacusMap.pairs[oldValue].balls[i] != abacusMap.pairs[newValue].balls[i])
            {
                // 网格信息更改
                pathFinding.GetGrid().GetGridObject(selectedColumn, abacusMap.pairs[oldValue].balls[i]).becomeBarrier = false;
                pathFinding.GetGrid().GetGridObject(selectedColumn, abacusMap.pairs[newValue].balls[i]).becomeBarrier = true;

                balls[selectedColumn, i] = abacusMap.pairs[newValue].balls[i];      // 算珠编号位置更换
                if (ballCoroutine[selectedColumn * 5 + i] != null)
                    StopCoroutine(ballCoroutine[selectedColumn * 5 + i]);
                ballCoroutine[selectedColumn * 5 + i] = 
                    StartCoroutine(ItemChangePos(ballTransform[selectedColumn * 5 + i], ballTransform[selectedColumn * 5 + i].position, pathFinding.GetGrid().GetWorldPosition(selectedColumn, abacusMap.pairs[newValue].balls[i])));
            }
        }
    }

    public bool isMoving()
    {
        return !nowCoroutineIsStop;
    }

    IEnumerator ItemChangePos(Transform item, Vector3 oldPos, Vector3 newPos) {
        while (Vector3.Distance(item.position, newPos) > 0.1f)
        {
            nowCoroutineIsStop = false;
            item.position = Vector3.MoveTowards(item.position, newPos, Time.deltaTime * ballMoveSpeed);
            yield return null;
        }
        nowCoroutineIsStop = true;
    }

    public void SetNumber(int num)
    {
        string numstr = num.ToString();
        string res = "";
        for (int i = 0; i < numstr.Length; i++)
        {
            res += "<sprite=" + numstr[i] + ">";
        }
        textMesh.text = res;
    }

    public void SetNumber()
    {
        SetNumber(columnNumber[selectedColumn]);
    }
}
