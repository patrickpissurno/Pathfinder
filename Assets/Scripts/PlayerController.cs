using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
    public const bool IS_DEBUG = false;

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
        transform.position = Vector3.Lerp(transform.position, targetPosition, .25f);
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

        int EXECUTION_LIMIT = IS_DEBUG ? 400 * ((grid.grid.Length / 4 + grid.grid[0].Length / 4)/2) : -1;

        bool didChange = true;
        int limit = 0;
        while (didChange)
        {
            didChange = false;
            List<GridItem>[] _paths = paths.ToArray();
            foreach (List<GridItem> path in _paths)
            {
                GridItem current = path[path.Count - 1];
                if (current == grid.endItem)
                    continue;
                List<GridItem> nexts = PathFinderStep(current.X, current.Y, path);
                Log("NEXTS: " + DebugItems(nexts));
                for (int i = 0; i < nexts.Count; i++)
                {
                    List<GridItem> newPath = path;
                    if (newPath.Contains(nexts[i]))
                        continue;
                    if (i != nexts.Count - 1)
                    {
                        paths.Add(new List<GridItem>(path));
                        newPath = paths[paths.Count - 1];
                    }
                    newPath.Add(nexts[i]);
                    didChange = true;
                    limit++;
                    if (EXECUTION_LIMIT != -1 && limit >= EXECUTION_LIMIT)
                        break;
                }
            }
            limit++;
            if (EXECUTION_LIMIT != -1 && limit >= EXECUTION_LIMIT)
                break;
        }

        Log("LIMIT: " + limit);

        int minCost = -1;
        int minID = -1;
        for(int i = 0; i<paths.Count; i++)
        {
            List<GridItem> path = paths[i];
            //if (path[path.Count - 1] != grid.endItem)
            //    continue;
            if (minCost == -1 || path.Count < minCost)
            {
                minCost = path.Count;
                minID = i;
            }
            Log(DebugItems(paths[i]));
        }

        if (minID != -1)
        {
            Log(paths[minID].Count);
        }
        else
            Log("Impossible Path");
        if(minID != -1)
            StartCoroutine(Walk(paths[minID]));
    }

    IEnumerator Walk(List<GridItem> path)
    {
        foreach (GridItem i in path)
        {
            yield return new WaitForSeconds(.25f);
            UpdatePosition(i);
        }
    }

    string DebugItems(List<GridItem> path)
    {
        string str = "";
        foreach (GridItem it in path)
            str += "(" + it.X + "," + it.Y + ")";
        return str;
    }

    public void Log(int str) { Log(str.ToString()); }
    public void Log(float str) { Log(str.ToString()); }
    public void Log(string str)
    {
        if(IS_DEBUG)
            Debug.Log(str);
    }

    List<GridItem> PathFinderStep(int X, int Y, List<GridItem> path)
    {
        int smallerCoast = -1;
        List<GridItem> items = new List<GridItem>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                /*if(!(X - i >= 0 && X - i < grid.grid.Length && Y - j >=0 && grid.grid.Length > 0 && Y - j < grid.grid[0].Length))
                    continue;*/
                if (!IsInsideGrid(X - i, Y - j))
                    continue;

                GridItem _item = grid.grid[X - i][Y - j];
                int cornerX = X - i + (i == 1 ? -1 : 1);
                int cornerY = Y - j + (j == 1 ? -1 : 1);

                bool hasBlockingCorner = false;
                if(IsInsideGrid(X, cornerY))
                {
                    GridItem _corner = grid.grid[X][cornerY];
                    Log(_corner.itemType.baseWeight);
                    if (_corner.itemType.baseWeight == -1)
                        hasBlockingCorner = true;
                }
                if (IsInsideGrid(cornerX, Y))
                {
                    GridItem _corner = grid.grid[cornerX][Y];
                    Log(_corner.itemType.baseWeight);
                    if (_corner.itemType.baseWeight == -1)
                        hasBlockingCorner = true;
                }

                if (hasBlockingCorner)
                    continue; 

                float weight = _item.itemType.baseWeight;
                if (weight == -1)
                    continue;

                if (path.Contains(_item))
                    weight *= 1.5f;

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

    bool IsInsideGrid(int X, int Y)
    {
        return (X >= 0 && X < grid.grid.Length && Y >= 0 && grid.grid.Length > 0 && Y < grid.grid[0].Length);
    }

    public void UpdatePosition(GridItem i)
    {
        targetPosition = i.transform.position + Vector3.up * defaultPosition.y;
        X = i.X;
        Y = i.Y;
        //path.Add(i);
    }
}
