using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    private Grid<GridNode> grid;
    public int x;
    public int y;

    public int gCost;   // 起始节点到该节点实际距离
    public int hCost;   // 该节点到目标节点估计距离
    public int fCost;   // g + h

    public bool isWalkable;
    public bool becomeBarrier;
    public GridNode cameFromNode;
    public int passNodeNum = 0;     // 每个节点寻路中找到的走过最多的路 

    public GridNode(Grid<GridNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
        becomeBarrier = false;
    }

    public GridNode(Grid<GridNode> grid, int x, int y, bool isWalkable)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        becomeBarrier = false;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
