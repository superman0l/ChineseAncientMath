using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    [SerializeField] public float radius;

    [SerializeField] public Transform background;
    [SerializeField] public SetRangeSO[] sliceSetRange;

    [SerializeField]private bool[] itemFilled;

    [SerializeField] public GameObject hexagon;
    [SerializeField] public GameObject dodecagon;
    [SerializeField] public Transform[] trianglesForDodecagon;
    private List<GameObject> slices = new List<GameObject>();
    private bool complete = false;
    private bool isFirst = true;

    private Flowchart flowchart;
    const string backLevel = "LevelPick";

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance");
        }
        Instance = this;
    }

    void Start()
    {
        flowchart = GameObject.Find("FlowChart").GetComponent<Flowchart>();
        BlockSignals.OnBlockEnd += OnBlockEnd;
        PrepareHexagon();

        flowchart.ExecuteBlock("EnterLevel4");
    }

    private void PrepareHexagon()
    {
        hexagon.SetActive(true);
        dodecagon.SetActive(false);

        itemFilled = new bool[4 * 6];

        float distance = radius * Mathf.Sin(60 * Mathf.Deg2Rad) * 2f / 3f;
        int deltaAngle = 30, stepAngle = 60;
        CalSlicePos(sliceSetRange[0], distance, deltaAngle, stepAngle);     // �����������Ƭλ�ü��� ����
        CalSlicePos(sliceSetRange[1], distance, deltaAngle, stepAngle);     // ������Ƭλ�ü���
        CalSlicePos(sliceSetRange[3], distance, deltaAngle, stepAngle);     // С�������м�λ�ü���
        //distance = radius * Mathf.Sin(60 * Mathf.Deg2Rad) / 2f;
        CalSlicePos(sliceSetRange[2], distance, deltaAngle, stepAngle);     // ������Ƭλ��

        // С��������Ƭ�ֱ����
        Vector2[] v2s = sliceSetRange[3].pitPos;
        sliceSetRange[3].pitPos = new Vector2[4 * 6];
        distance = radius * Mathf.Sin(60 * Mathf.Deg2Rad) * 2f / 6f;
        for (int i = 0; i < v2s.Length; i++)
        {
            Vector2 origin = v2s[i];
            if (i % 2 == 0)
            {
                Vector2 target = origin + Vector2.left * distance;
                sliceSetRange[3].pitPos[i * 4 + 2] = origin;
                sliceSetRange[3].pitPos[i * 4 + 1] = target;
                sliceSetRange[3].pitPos[i * 4 + 3] = RotatePoint(origin, target, 120);
                sliceSetRange[3].pitPos[i * 4 + 0] = RotatePoint(origin, target, -120);
            }
            else
            {
                Vector2 target = origin + Vector2.right * distance;
                sliceSetRange[3].pitPos[i * 4 + 2] = origin;
                sliceSetRange[3].pitPos[i * 4 + 3] = target;
                sliceSetRange[3].pitPos[i * 4 + 0] = RotatePoint(origin, target, 120);
                sliceSetRange[3].pitPos[i * 4 + 1] = RotatePoint(origin, target, -120);
            }
        }
    }

    private void PrepareDedecagon()
    {
        foreach (var item in slices)
        {
            Destroy(item);
        }
        slices.Clear();

        hexagon.SetActive(false);
        dodecagon.SetActive(true);

        itemFilled = new bool[5 * 4];
        float distance = radius / 2;
        int stepAngle = 90;
        CalSlicePos(sliceSetRange[4], distance, 0, stepAngle);  // ����
        distance = 2.16f;
        CalSlicePos(sliceSetRange[5], distance, 45, stepAngle); // ������
        CalSlicePos(sliceSetRange[6], distance, 45, stepAngle); // �����

        // С�����μ���
        sliceSetRange[7].pitPos = new Vector2[3 * 4];
        for(int i = 0; i < 4; i++)
        {
            sliceSetRange[7].pitPos[i * 3 + 0] = RotatePoint(background.position, trianglesForDodecagon[0].position, i * 90);
            sliceSetRange[7].pitPos[i * 3 + 1] = RotatePoint(background.position, trianglesForDodecagon[1].position, i * 90);
            sliceSetRange[7].pitPos[i * 3 + 2] = RotatePoint(background.position, trianglesForDodecagon[2].position, i * 90);
        }

    }

    private void CalSlicePos(SetRangeSO sliceRange, float distance, int deltaAngle, int stepAngle)
    {
        int num = 360 / stepAngle;
        sliceRange.pitPos = new Vector2[num];
        Vector2 origin = background.position;
        Vector2 target = RotatePoint(origin, origin + Vector2.up * distance, deltaAngle);
        for (int i = 0; i < num; i++)
        {
            Vector2 t = RotatePoint(origin, target, i * stepAngle);
            sliceRange.pitPos[i] = t;
        }
    }

    Vector2 RotatePoint(Vector2 p1, Vector2 p2, float angle) // ��ʱ����ת
    {
        Vector2 p2_prime = p2 - p1;

        float rad = angle * Mathf.Deg2Rad;
        Vector2 p2_double_prime = new Vector2(
            p2_prime.x * Mathf.Cos(rad) - p2_prime.y * Mathf.Sin(rad),
            p2_prime.x * Mathf.Sin(rad) + p2_prime.y * Mathf.Cos(rad)
        );

        Vector2 p2_triple_prime = p2_double_prime + p1;
        return p2_triple_prime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Check(int id, Vector3 mousePos, int angle, out Vector2 res, GameObject obj)
    {
        Vector2 pos = mousePos;
        foreach (var setRange in sliceSetRange)
        {
            if(setRange.id == id)
            {
                for(int i = 0; i < setRange.pitPos.Length; i++)
                {
                    int angleIndexMin = (i * setRange.angleNumPerPos) % setRange.angleNum;  // ͬһλ�ö�Ӧ����Ƕ� 
                    if (Vector2.Distance(pos, setRange.pitPos[i]) < setRange.checkRange) { // λ��һ��
                        for (int j = 0; j < setRange.angleNumPerPos; j++)
                        {
                            // Debug.Log("Angle" + angle);
                            if (setRange.angle[angleIndexMin + j] == angle) // �Ƕ�һ��
                            {
                                int index = (i * setRange.angleNumPerPos + j) % setRange.indexNum;
                                int[] range = new int[setRange.rangeSize];
                                int rangeIndex = ((i * setRange.angleNumPerPos + j) % setRange.rangeNum) * setRange.rangeSize;
                                for (int k = 0; k < setRange.rangeSize; k++)
                                {
                                    range[k] = setRange.coverRange[rangeIndex + k];
                                }
                                res = setRange.pitPos[i];
                                Debug.Log("i" + i + "\nj" + j + "\nindex" + index + "\nrangeIndex" + rangeIndex);
                                slices.Add(obj);
                                if (!fill(setRange.startIndex[index], range)) // �ظ�ƴͼ
                                {
                                    slices.Remove(obj);
                                    res = new Vector2(0, 0);
                                    return false;
                                }
                                SoundManager.Instance.PlayElimiSound();
                                return true;
                            }
                        }
                    }
                }
            }
        }
        res = new Vector2(0, 0);
        return false;
    }

    public bool fill(int startIndex, int[] range)
    {
        for (int i = 0; i < range.Length; i++)
        {
            if (itemFilled[startIndex + range[i]]) return false;
        }

        for (int i = 0; i < range.Length; i++)
        {
            itemFilled[startIndex + range[i]] = true;
        }

        if (checkComplete())
        {
            if (isFirst)
            {
                isFirst = false;
                PrepareDedecagon();
                ConveyorBelt.Instance.Change();
                flowchart.ExecuteBlock("Level4Phase1");
            }
            else
            {
                flowchart.ExecuteBlock("Level4Done");
            }
        }
            
        return true;
    }

    private bool checkComplete()
    {
        for (int i = 0; i < itemFilled.Length; i++)
        {
            if (!itemFilled[i]) return false;
        }
        complete = true;
        return complete;
    }

    private void OnBlockEnd(Block block)
    {
        if(block == flowchart.FindBlock("EnterLevel4"))
        {
            flowchart.ExecuteBlock("Level4Tip");
        }
        else if(block == flowchart.FindBlock("Level4Tip"))
        {

        }
        if (block == flowchart.FindBlock("Level4Done"))
            SceneManager.LoadScene(backLevel);
    }
}
