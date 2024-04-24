using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Drawing;
using UnityEngine.SceneManagement;
using Fungus;
using System.Runtime.CompilerServices;

public enum ItemState
{
    Selected,
    Unselected
}
[Serializable]
public struct Rect
{
    public int minx, miny;
    public int maxx, maxy;
}
public static class Utils
{
    public static Rect TwoPointToRect(Vector2Int a, Vector2Int b)
    {
        Rect rect = new Rect();
        rect.minx = Mathf.Min(a.x, b.x);
        rect.miny = Mathf.Min(a.y, b.y);
        rect.maxx = Mathf.Max(a.x, b.x);
        rect.maxy = Mathf.Max(a.y, b.y);
        return rect;
    }
    public static bool TwoRectIsSafe(Rect a, Rect b)
    {
        if (a.minx > b.maxx || b.minx > a.maxx)
        {
            return true;
        }

        if (a.miny > b.maxy || b.miny > a.maxy)
        {
            return true;
        }

        return false;
    }
}
public class FieldManager : MonoBehaviour
{
    public static FieldManager Instance { get; private set; }
    [Header("LevelInfo")]
    [SerializeField] float itemSize;
    [SerializeField] float itemSpacing;
    [SerializeField] int rowNum;
    [SerializeField] int colNum;
    [Header("Object")]
    [SerializeField] Transform item;
    [SerializeField] Transform background;
    [SerializeField] FixedFieldItemSO fixedFieldItemSO;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Flowchart flowchart;
    [SerializeField] GameObject cover;

    const string backLevel = "LevelPick";

    private ItemState state;
    private int selectedIndex;
    private FieldItem[,] itemList;
    private bool complete = false;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one instance");
        }
        Instance = this;
    }

    private void Start()
    {
        flowchart = GameObject.Find("FlowChart").GetComponent<Flowchart>();
        BlockSignals.OnBlockEnd += OnBlockEnd;
        state = ItemState.Unselected;
        itemList = new FieldItem[rowNum, colNum];
        for (int i = 0; i < rowNum; i++)
        {
            for (int j = 0; j < colNum; j++)
            {
                Transform newItem = Instantiate(item, IndexToWorldPos(i, j), Quaternion.identity);
                newItem.localScale = new Vector3(itemSize, itemSize, 1f);
                itemList[i, j] = newItem.GetComponent<FieldItem>();
            }
        }
        for (int i = 0; i < fixedFieldItemSO.FixedItems.Length; i++)
        {
            fixedFieldItemSO.FixedItems[i].target = fixedFieldItemSO.FixedItems[i].pos;
            itemList[fixedFieldItemSO.FixedItems[i].pos.y, fixedFieldItemSO.FixedItems[i].pos.x].SetNumber(fixedFieldItemSO.FixedItems[i].requiredNum);
        }
        Render();

        flowchart.ExecuteBlock("EnterLevel3");
    }

    private Vector2Int WorldPosToIndex(Vector3 pos)
    {
        Vector3 offset = pos - background.position;
        int x = Mathf.Clamp(Mathf.RoundToInt(offset.x / itemSpacing + (-0.5f) * ((colNum + 1) & 1)) + colNum / 2, 0, colNum - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(offset.y / itemSpacing + (-0.5f) * ((rowNum + 1) & 1)) + rowNum / 2, 0, rowNum - 1);
        return new Vector2Int(x, y);
    }

    private Vector3 IndexToWorldPos(int i, int j)
    {
        float xpos = (j - colNum / 2 + (float)((colNum + 1) & 1) / 2) * itemSpacing;
        float ypos = (i - rowNum / 2 + (float)((rowNum + 1) & 1) / 2) * itemSpacing;
        Vector3 deltaPos = new Vector3(xpos, ypos, -0.5f);
        return deltaPos + background.position;
    }

    private void Update()
    {
        if(state == ItemState.Selected)
        {
            Vector3 mouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int index = WorldPosToIndex(mouseDownPosition);
            Vector2Int preTarget = fixedFieldItemSO.FixedItems[selectedIndex].target;
            fixedFieldItemSO.FixedItems[selectedIndex].target = index;
            if (CheckValid())
            {
                Render();
            }
            else
            {
                fixedFieldItemSO.FixedItems[selectedIndex].target = preTarget;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int index = WorldPosToIndex(mouseDownPosition);
            Debug.Log(index);

            for (int i = 0; i < fixedFieldItemSO.FixedItems.Length; i++)
            {
                Vector2Int fixedItem = new Vector2Int(fixedFieldItemSO.FixedItems[i].pos.x, fixedFieldItemSO.FixedItems[i].pos.y);
                if(fixedItem == index)
                {
                    SoundManager.Instance.PlayGrassSound();
                    state = ItemState.Selected;
                    selectedIndex = i;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            SoundManager.Instance.StopGrassSound();
            state = ItemState.Unselected;
        }
    }

    private void Render()
    {
        for(int i = 0; i < rowNum; i++)
        {
            for (int j = 0;j< colNum; j++)
            {
                itemList[i,j].Change(defaultSprite);
            }
        }
        for(int i = 0; i < fixedFieldItemSO.FixedItems.Length; i++)
        {
            fill(i);
        }

        if (CheckComplete()&&!complete)
        {
            complete = true;
            flowchart.ExecuteBlock("Level3Done");
        }
    }

    private void OnBlockEnd(Block block)
    {
        if (cover == null) return;
        if (block == flowchart.FindBlock("EnterLevel3"))
        {
            cover.SetActive(true);
            flowchart.ExecuteBlock("Level3Tip");
        }
        else if (block == flowchart.FindBlock("Level3Tip"))
            cover.SetActive(false);
        else if (block == flowchart.FindBlock("Level3Done"))
        {
            SoundManager.Instance.StopGrassSound();
            SceneManager.LoadScene(backLevel);
        }
    }

    private bool CheckComplete()
    {
        for (int i = 0; i < fixedFieldItemSO.FixedItems.Length; i++)
        {
            if (fixedFieldItemSO.FixedItems[i].target != fixedFieldItemSO.FixedItems[i].finalTarget) return false;
        }
        return true;
    }

    public bool CheckValid()
    {
        for(int i = 0; i < fixedFieldItemSO.FixedItems.Length; i++)
        {
            for(int j = i+1;j< fixedFieldItemSO.FixedItems.Length; j++)
            {
                Rect rect1 = Utils.TwoPointToRect(fixedFieldItemSO.FixedItems[i].pos, fixedFieldItemSO.FixedItems[i].target);
                Rect rect2 = Utils.TwoPointToRect(fixedFieldItemSO.FixedItems[j].pos, fixedFieldItemSO.FixedItems[j].target);
                if (!Utils.TwoRectIsSafe(rect1, rect2)) return false;
            }
        }
        return true;
    }

    public void fill(int fixedID)
    {
        Rect rect = Utils.TwoPointToRect(fixedFieldItemSO.FixedItems[fixedID].pos, fixedFieldItemSO.FixedItems[fixedID].target);
        for(int i = rect.miny; i <= rect.maxy; i++)
        {
            for(int j = rect.minx; j <= rect.maxx; j++)
            {
                itemList[i, j].Change(fixedFieldItemSO.FixedItems[fixedID].sprite);
            }
        }
    }

}
