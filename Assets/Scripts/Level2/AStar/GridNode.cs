using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    private Grid<GridNode> grid;
    public int x;
    public int y;

    public int gCost;   // ��ʼ�ڵ㵽�ýڵ�ʵ�ʾ���
    public int hCost;   // �ýڵ㵽Ŀ��ڵ���ƾ���
    public int fCost;   // g + h

    public bool isWalkable;
    public bool becomeBarrier;
    public GridNode cameFromNode;
    public int passNodeNum = 0;     // ÿ���ڵ�Ѱ·���ҵ����߹�����· 

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
