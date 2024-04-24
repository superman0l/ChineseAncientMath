using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level2/GridInfo")]
public class GridInfo : ScriptableObject
{
    public bool isBlockLess;
    
    public List<Vector2Int> NodeBlocked;      // 如果阻挡节点少于可走节点则记录阻挡节点
    public List<Vector2Int> NodeWalkable;     // 否则反之
}
