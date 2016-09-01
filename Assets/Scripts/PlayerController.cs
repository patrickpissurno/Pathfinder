using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
    private GridController grid;
    public Vector3 defaultPosition;
    public Vector3 targetPosition;

    private int H = 0;
    public int X;
    public int Y;

    private List<GridItem> path;

	void Start () {
        defaultPosition = transform.position;
        gameObject.SetActive(false);
        path = new List<GridItem>();
	}

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, .05f);
    }

    public void StartPathfind(GridController grid)
    {
        this.grid = grid;
        CancelInvoke();
        path.Clear();
        InvokeRepeating("PathFinderStep", 1f, 1f);
    }

    int CalculateH(GridItem i)
    {
        return CalculateH(i.X, i.Y);
    }

    int CalculateH(int X, int Y)
    {
        return H = (Mathf.Abs(grid.endItem.X - X) + Mathf.Abs(grid.endItem.Y - Y)) * 10;
    }

    void PathFinderStep()
    {
        int smallerCoast = -1;
        GridItem item = null;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                if(!(X - i >= 0 && X - i < grid.grid.Length && Y - j >=0 && grid.grid.Length > 0 && Y - j < grid.grid[0].Length))
                    continue;

                GridItem _item = grid.grid[X - i][Y - j];

                if (_item.itemType.baseWeight == -1)
                    continue;
                if (path.Contains(_item))
                    continue;

                int G = Mathf.Abs(i) == Mathf.Abs(j) && Mathf.Abs(i) == 1 ? _item.itemType.baseWeight + 4 : _item.itemType.baseWeight;
                int cost = G + CalculateH(_item);
                Debug.Log("G: " + G + ", H: " + H + ", X: " + X + ", Y: " + Y);
                if (smallerCoast == -1 || cost < smallerCoast)
                {
                    item = _item;
                    smallerCoast = cost;
                }
            }
        }

        if (item == null)
            return;

        UpdatePosition(item);

        if (item == grid.endItem)
        {
            CancelInvoke();
            grid.StopPathfinding();
        }
    }

    public void UpdatePosition(GridItem i)
    {
        targetPosition = i.transform.position + Vector3.up * defaultPosition.y;
        X = i.X;
        Y = i.Y;
        path.Add(i);
    }
}
