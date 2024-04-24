using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LinkedItemsSO;

[CreateAssetMenu()]
public class FixedFieldItemSO : ScriptableObject
{
    [Serializable]
    public struct FixedItem
    {
        public int requiredNum;
        public Vector2Int pos;
        public bool isCorrect;
        public Vector2Int target;
        public Vector2Int finalTarget;
        public Sprite sprite;
    }

    public FixedItem[] FixedItems;
}
