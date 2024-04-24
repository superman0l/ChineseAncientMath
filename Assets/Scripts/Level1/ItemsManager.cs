using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;
using UnityEngine.SceneManagement;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager Instance { get; private set; }

    private int itemID1, itemID2;
    private LinkItem selectedItem1, selectedItem2;
    private int count;

    private bool complete = false;
    const string backLevel = "LevelPick";

    private List<int> posList = new List<int>(); // 编号列表
    private List<int> itemList = new List<int>(); // 编号列表

    [Header("Level Settings")]
    [SerializeField] float unitItem = 1.0f;
    [SerializeField] float itemSize = 0.7f;
    [SerializeField] int rowNum = 5;
    [SerializeField] int colNum = 10;

    [Header("Level Item Data")]
    [SerializeField] LinkedItemsSO linkedItemsSO;
    [SerializeField] LinkItemsSO linkItemSO;
    [SerializeField] Transform itemBackground;
    [SerializeField] Flowchart flowchart;
    [SerializeField] GameObject cover;

    // Start is called before the first frame update
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
        itemID1 = itemID2 = -1;
        count = 0;
        for (int i = 1; i <= 18; i++)
            itemList.Add(i);
        for (int i = 0; i < rowNum * colNum; i++)
            posList.Add(i);
        GenLevel();
        flowchart.ExecuteBlock("EnterLevel1");
    }

    void GenLevel()
    {
        for (int i = 0; i < rowNum * colNum; i++)
        {
            if (itemList.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, itemList.Count);
                int id = itemList[index];
                itemList.RemoveAt(index); // 从编号列表中移除已经选择的编号

                GenItem(id);
            }
            else
            {
                // 如果编号列表为空，则随机生成一个1-18的编号
                int id = UnityEngine.Random.Range(1, 19);
                if (id % 2 == 0)
                {
                    GenItem(id - 1); GenItem(id);
                }
                else
                {
                    GenItem(id); GenItem(id + 1);
                }
                i++;
            }
        }
        // GenItem(1, new Vector3(0f, 0.5f, -1f));
        // GenItem(2, new Vector3(0f, -0.5f, -1f));
        // GenItem(3, new Vector3(1f, 0f, -1f));
    }
    void GenItem(int id)
    {
        int index = UnityEngine.Random.Range(0, posList.Count);
        int posid = posList[index];
        posList.RemoveAt(index); // 从编号列表中移除已经选择的编号
        float xpos = (posid % colNum - colNum / 2 + 0.5f) * unitItem;
        float ypos = (posid / colNum - rowNum / 2) * unitItem;
        Vector3 deltaPos = new Vector3(xpos, ypos, -0.5f);

        foreach (var i in linkItemSO.iD_Objects)
        {
            if (id == i.key)
            {
                GameObject newItem = Instantiate(i.value, itemBackground.position + deltaPos, Quaternion.identity);
                newItem.transform.localScale = new Vector3(itemSize, itemSize, 1f);
                newItem.GetComponent<LinkItem>().SetID(id);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Finish()
    {
        if (count >= rowNum * colNum && !complete)
        {
            complete = true;
            flowchart.ExecuteBlock("Level1Done");
        }
    }

    private void OnBlockEnd(Block block)
    {
        if (cover == null) return;
        if (block == flowchart.FindBlock("EnterLevel1"))
        {
            cover.gameObject.SetActive(true) ;
            flowchart.ExecuteBlock("Level1Tip");
        }
        else if (block == flowchart.FindBlock("Level1Tip"))
        {
            cover.gameObject.SetActive(false);
        }
        else if (block == flowchart.FindBlock("Level1Done"))
            SceneManager.LoadScene(backLevel);
    }

    IEnumerator Check()
    {
        if (CheckIsLinked(itemID1, itemID2))
        {
            yield return new WaitForSeconds(0.3f);
            selectedItem1.DestroySelf();
            selectedItem2.DestroySelf();
            count += 2;
            SoundManager.Instance.PlayElimiSound();
            Finish();

        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            selectedItem1.Close();
            selectedItem2.Close();
        }
        itemID1 = itemID2 = -1;
    }

    public void LinkItemSelected(LinkItem selectedItem)
    {
        if (itemID1 == -1)
        {
            selectedItem1 = selectedItem;
            itemID1 = selectedItem1.GetID();
        }
        else if (itemID2 == -1)
        {
            selectedItem2 = selectedItem;
            itemID2 = selectedItem2.GetID();
            StartCoroutine(Check());
        }
    }

    public void ClearSelected()
    {
        itemID1 = itemID2 = -1;
    }

    bool CheckIsLinked(int itemID1, int itemID2)
    {
        foreach (var pair in linkedItemsSO.linkedPairs)
        {
            if (itemID1 == pair.object1 && itemID2 == pair.object2)
                return true;
            else if (itemID1 == pair.object2 && itemID2 == pair.object1)
                return true;
        }
        return false;
    }
}
