using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    private GridController grid;
    public Vector3 defaultPosition;
    private int H = 0;
    public int X;
    public int Y;

	void Start () {
        defaultPosition = transform.position;
        gameObject.SetActive(false);
	}

    public void StartPathfind(GridController grid)
    {
        this.grid = grid;
        CancelInvoke();
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
                if (grid.grid[X - i][Y - j] == null)
                    continue;
                GridItem _item = grid.grid[X - i][Y - j];
                if (_item.itemType.baseWeight == -1)
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
        Debug.Log(item.X + " " + item.Y);
        //CancelInvoke();
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
        transform.position = i.transform.position + Vector3.up * defaultPosition.y;
        X = i.X;
        Y = i.Y;
    }
}
