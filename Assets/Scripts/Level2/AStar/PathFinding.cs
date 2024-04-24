using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    private const int MOVE_STRAIGHT_COST = 10;

    public static PathFinding Instance { get; private set; }

    private Grid<GridNode> grid;
    private List<GridNode> openList;
    private List<GridNode> closedList;

    public PathFinding(int width, int height, float size, Vector3 GridCenter, GridInfo gridInfo)
    {
        Instance = this;
        // LoadGridInfo
        if (gridInfo.isBlockLess)   // �ϰ������
        {
            grid = new Grid<GridNode>(width, height, size, GridCenter, (Grid<GridNode> g, int x, int y) => new GridNode(g, x, y));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid.SetGridObject(x, y, new GridNode(grid, x, y, true));
                }
            }
            foreach (var node in gridInfo.NodeBlocked)
            {
                grid.SetGridObject(node.x, node.y, new GridNode(grid, node.x, node.y, false));
            }
        }
        else                        // ���ƶ��ؿ����
        {
            grid = new Grid<GridNode>(width, height, size, GridCenter, (Grid<GridNode> g, int x, int y) => new GridNode(g, x, y));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid.SetGridObject(x, y, new GridNode(grid, x, y, false));
                }
            }
            foreach (var node in gridInfo.NodeBlocked)
            {
                grid.SetGridObject(node.x, node.y, new GridNode(grid, node.x, node.y, true));
            }
        }
    }

    public PathFinding(int width, int height, float widthSize, float heightSize, Vector3 GridCenter, GridInfo gridInfo)
    {
        Instance = this;
        // LoadGridInfo
        if (gridInfo.isBlockLess)   // �ϰ������
        {
            grid = new Grid<GridNode>(width, height, widthSize, heightSize, GridCenter, (Grid<GridNode> g, int x, int y) => new GridNode(g, x, y));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid.SetGridObject(x, y, new GridNode(grid, x, y, true));
                }
            }
            foreach (var node in gridInfo.NodeBlocked)
            {
                grid.SetGridObject(node.x, node.y, new GridNode(grid, node.x, node.y, false));
            }
        }
        else                        // ���ƶ�����
        {
            grid = new Grid<GridNode>(width, height, widthSize, heightSize, GridCenter, (Grid<GridNode> g, int x, int y) => new GridNode(g, x, y));
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid.SetGridObject(x, y, new GridNode(grid, x, y, false));
                }
            }
            foreach (var node in gridInfo.NodeWalkable)
            {
                grid.SetGridObject(node.x, node.y, new GridNode(grid, node.x, node.y, true));
            }
        }
    }


    public Grid<GridNode> GetGrid()
    {
        return grid;
    }

    public List<Vector2> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        bool complete;
        List<GridNode> path = FindPath(startX, startY, endX, endY, out complete);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector2> vectorPath = new List<Vector2>();
            foreach (GridNode pathNode in path)
            {
                vectorPath.Add(new Vector2(pathNode.x * grid.GetWidthCellSize, pathNode.y * grid.GetHeightCellSize));
            }
            return vectorPath;
        }
    }

    public List<GridNode> FindPath(int startX, int startY, int endX, int endY, out bool complete)
    {
        GridNode startNode = grid.GetGridObject(startX, startY);
        GridNode endNode = grid.GetGridObject(endX, endY);

        if (startNode == null || endNode == null)
        {
            // Invalid Path
            complete = false;
            return null;
        }

        openList = new List<GridNode> { startNode };
        closedList = new List<GridNode>();

        // ��ʼ��
        for (int x = 0; x < grid.GetWidth; x++)
        {
            for (int y = 0; y < grid.GetHeight; y++)
            {
                GridNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.passNodeNum = 0;
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        GridNode keyNode = startNode;   // ��δ����Ŀ�� ȡ�ߵ����·��
        while (openList.Count > 0)
        {
            GridNode currentNode = GetLowestFCostNode(openList);    // ȡ��СFCost�ڵ㣨���ȶ����Ż���
            if(currentNode.passNodeNum > keyNode.passNodeNum)
            {
                keyNode = currentNode;
            }
            if (currentNode == endNode) // ����Ŀ�� �������
            {
                complete = true;
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (GridNode neighbourNode in GetNeighbourList(currentNode))
            {
                neighbourNode.passNodeNum = neighbourNode.passNodeNum == 0 ? currentNode.passNodeNum + 1 : neighbourNode.passNodeNum;   //�Ѿ�������������
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable || neighbourNode.becomeBarrier)      // �ж��ϰ�
                {
                    closedList.Add(neighbourNode);
                    if(neighbourNode.isWalkable && neighbourNode.becomeBarrier && neighbourNode.passNodeNum >= keyNode.passNodeNum)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        complete = false;
                        return CalculatePath(neighbourNode);
                    }
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // �����ߵ�����·��
        complete = false;
        return CalculatePath(keyNode);
    }

    private List<GridNode> CalculatePath(GridNode endNode)  // ���ݵõ����յ�ڵ���Ƶ����
    {
        List<GridNode> path = new List<GridNode>();
        path.Add(endNode);
        GridNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    public GridNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    private List<GridNode> GetNeighbourList(GridNode currentNode)   // �ƶ�����Ϊ��Χ�ĸ�
    {
        List<GridNode> neighbourList = new List<GridNode>();

        if (currentNode.x - 1 >= 0)
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));       // Left
        if (currentNode.x + 1 < grid.GetWidth)
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));       // Right
        if (currentNode.y - 1 >= 0) 
            neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));       // Down
        if (currentNode.y + 1 < grid.GetHeight) 
            neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));       // Up

        return neighbourList;
    }

    private int CalculateDistanceCost(GridNode a, GridNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        return MOVE_STRAIGHT_COST * (xDistance + yDistance);
    }

    private GridNode GetLowestFCostNode(List<GridNode> pathNodeList)    // *���ȶ����Ż�
    {
        GridNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost <= lowestFCostNode.fCost)
            {
                if(pathNodeList[i].fCost < lowestFCostNode.fCost)
                    lowestFCostNode = pathNodeList[i];
                else if (pathNodeList[i].fCost == lowestFCostNode.fCost && pathNodeList[i].hCost < lowestFCostNode.hCost) // f��ȵ����ѡhС�ıض���
                    lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}
