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

    private List<List<GridItem>> paths;

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
        //InvokeRepeating("PathFinderStep", 1f, 1f);
        CalculatePaths();
    }

    int CalculateH(GridItem i)
    {
        return CalculateH(i.X, i.Y);
    }

    int CalculateH(int X, int Y)
    {
        return H = (Mathf.Abs(grid.endItem.X - X) + Mathf.Abs(grid.endItem.Y - Y)) * 10;
    }

    void CalculatePaths()
    {
        paths = new List<List<GridItem>>();
        int X = grid.startItem.X;
        int Y = grid.startItem.Y;
        paths.Add(new List<GridItem>());
        paths[0].Add(grid.startItem);

        bool didChange = true;
        while (didChange)
        {
            didChange = false;
            List<GridItem>[] _paths = paths.ToArray();
            foreach (List<GridItem> path in _paths)
            {
                GridItem current = path[path.Count - 1];
                if (current == grid.endItem)
                    continue;
                List<GridItem> nexts = PathFinderStep(current.X, current.Y);
                for (int i = 0; i < nexts.Count; i++)
                {
                    if (i == 0)
                        didChange = true;
                    List<GridItem> newPath = path;
                    if (i > 0)
                    {
                        paths.Add(new List<GridItem>(path));
                        newPath = paths[paths.Count - 1];
                    }
                    newPath.Add(nexts[i]);
                }
            }
        }

        int minCost = -1;
        int minID = -1;
        for(int i = 0; i<paths.Count; i++)
        {
            if (minCost == -1 || paths[i].Count < minCost)
            {
                minCost = paths[i].Count;
                minID = i;
            }
        }
        
        if(minID != -1)
            StartCoroutine(Walk(paths[minID]));
    }

    IEnumerator Walk(List<GridItem> path)
    {
        foreach (GridItem i in path)
        {
            yield return new WaitForSeconds(1f);
            UpdatePosition(i);
        }
    }

    List<GridItem> PathFinderStep(int X, int Y)
    {
        int smallerCoast = -1;
        List<GridItem> items = new List<GridItem>();
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

                if (smallerCoast == -1 || cost < smallerCoast)
                {
                    items.Clear();
                    items.Add(_item);
                    smallerCoast = cost;
                }
                else if (smallerCoast == cost)
                {
                    items.Add(_item);
                }
            }
        }
        return items;
    }

    public void UpdatePosition(GridItem i)
    {
        targetPosition = i.transform.position + Vector3.up * defaultPosition.y;
        X = i.X;
        Y = i.Y;
        //path.Add(i);
    }
}
