using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level2/GridInfo")]
public class GridInfo : ScriptableObject
{
    public bool isBlockLess;
    
    public List<Vector2Int> NodeBlocked;      // ����赲�ڵ����ڿ��߽ڵ����¼�赲�ڵ�
    public List<Vector2Int> NodeWalkable;     // ����֮
}
