using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SetRangeSO : ScriptableObject
{
    public int id;
    public float checkRange;
    public Vector2[] pitPos;
    [Header("Angle")]
    public int angleNumPerPos;  // 可能有一个坐标对应多种角度
    public int angleNum;
    public int[] angle;
    
    [Header("Index & Range")]
    public int[] startIndex;
    public int indexNum;
    public int[] coverRange;
    public int rangeNum;    // 几种range
    public int rangeSize;   // range大小
}
