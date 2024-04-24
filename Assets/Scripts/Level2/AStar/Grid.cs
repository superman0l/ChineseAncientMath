using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject>
{

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellWidthSize;
    private float cellHeightSize;
    private Vector3 originPosition;     // 最左下角
    private TGridObject[,] gridArray;
    TGridObject extraPoint;


    public Grid(int width, int height, float cellSize, Vector3 centerPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellWidthSize = this.cellHeightSize = cellSize;

        Vector3 offset = new Vector3(-(((width + 1) % 2) / 2 + width / 2) * cellSize, -(((height + 1) % 2) / 2 + height / 2) * cellSize, -0.5f); // 传入背景坐标确保生成物在背景之前
        this.originPosition =  centerPosition + offset;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }
    }

    public Grid(int width, int height, float cellWidthSize, float cellHeightSize, Vector3 centerPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellWidthSize = cellWidthSize;
        this.cellHeightSize = cellHeightSize;

        Vector3 offset = new Vector3(-(((width + 1) % 2) / 2 + width / 2) * cellWidthSize, -(((height + 1) % 2) / 2 + height / 2) * cellHeightSize, -0.5f); // 传入背景坐标确保生成物在背景之前
        this.originPosition = centerPosition + offset;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        extraPoint = createGridObject(this, -1, 8);
    }

    public int GetWidth => width;
    public int GetHeight => height;
    public float GetWidthCellSize => cellWidthSize;
    public float GetHeightCellSize => cellHeightSize;

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellWidthSize, y * cellHeightSize) + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellWidthSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellHeightSize);
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
        }
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if(x==-1)return extraPoint;
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

}
