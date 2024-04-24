using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public static ConveyorBelt Instance { get; private set; }

    [SerializeField] public Transform[] prefabItem; // 物体数组
    [SerializeField] public float speed = 1.0f; // 移动速度
    [SerializeField] public int existNum = 3; // 最大存在物品数量
    [SerializeField] public float spawnInterval = 1.5f; // 生成间隔
    [SerializeField] public Transform spawnPosition; // 生成位置
    [SerializeField] public Vector3 direction;  // 移动方向
    private List<Transform> items = new List<Transform>();
    private List<Transform> existItems = new List<Transform>();

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
        items.Add(prefabItem[0]);
        items.Add(prefabItem[1]);
        items.Add(prefabItem[2]);
        items.Add(prefabItem[2]);
        // 开始生成物体
        InvokeRepeating("SpawnItem", 0, spawnInterval);
    }
    void Update()
    {
        foreach (var item in existItems)
        {
            if (!item.GetComponent<PuzzleItem>().CheckFrontItem(direction)) {
                item.position += direction * speed;
            }
        }
    }

    public void RemoveItem(Transform item)
    {
        List<Transform> itemsToRemove = new List<Transform>();
        foreach (var existItem in existItems)
        {
            if(existItem.position == item.position)
            {
                itemsToRemove.Add(item);
            }
        }
        foreach (var existItem in itemsToRemove)
        {
            existItems.Remove(existItem);
        }
    }

    public void Change()
    {
        items.Clear();
        items.Add(prefabItem[3]);
        items.Add(prefabItem[4]);
        items.Add(prefabItem[5]);
        items.Add(prefabItem[6]);
        items.Add(prefabItem[6]);
    }
    void SpawnItem()
    {
        if (existItems.Count >= existNum) return;
        Transform item = items[UnityEngine.Random.Range(0, items.Count)];
        Transform newItem = Instantiate(item, spawnPosition.position, item.rotation);
        existItems.Add(newItem);
    }
}
