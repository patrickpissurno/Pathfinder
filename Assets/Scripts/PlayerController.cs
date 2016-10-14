using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private GridController grid;
    public Vector3 defaultPosition;
    public Vector3 targetPosition;

    private int H = 0;
    public int X;
    public int Y;

    private Path path;

    private List<Path> paths;

    void Start()
    {
        defaultPosition = transform.position;
        gameObject.SetActive(false);
        path = new Path();
    }

    void FixedUpdate()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, .25f);
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
        paths = new List<Path>();
        int X = grid.startItem.X;
        int Y = grid.startItem.Y;
        paths.Add(new Path());
        paths[0].Add(grid.startItem);

        int EXECUTION_LIMIT = 5000;

        bool didChange = true;
        int limit = 0;
        while (didChange)
        {
            didChange = false;
            Path[] _paths = paths.ToArray();
            foreach (Path path in _paths)
            {
                GridItem current = path.steps[path.Size() - 1];
                if (current == grid.endItem)
                    continue;
                List<ItemCost> nexts = PathFinderStep(current.X, current.Y, path);
                Debug.Log("NEXTS: " + DebugItems(nexts));
                for (int i = 0; i < nexts.Count; i++)
                {
                    Path newPath = path;
                    if (newPath.Contains(nexts[i].gridItem))
                        continue;
                    if (i != nexts.Count - 1)
                    {
                        Path p = new Path();
                        p.steps = new List<GridItem>(path.steps);
                        paths.Add(p);
                        newPath = paths[paths.Count - 1];
                    }
                    newPath.Add(nexts[i].gridItem);
                    newPath.cost += nexts[i].cost;
                    didChange = true;
                    limit++;
                    if (limit >= EXECUTION_LIMIT)
                        break;
                }
            }
            limit++;
            if (limit >= EXECUTION_LIMIT)
                break;
        }

        Debug.Log("LIMIT: " + limit);

        int minCost = -1;
        int minID = -1;
        for (int i = 0; i < paths.Count; i++)
        {
            Path path = paths[i];
            if (path.steps[path.Size() - 1] != grid.endItem)
                continue;
            if (minCost == -1 || path.cost < minCost)
            {
                minCost = path.cost;
                minID = i;
            }
            Debug.Log(DebugItems(paths[i].steps));
        }

        if (minID != -1)
        {
            Debug.Log(paths[minID].cost);
        }
        if (minID != -1)
            StartCoroutine(Walk(paths[minID]));
    }

    IEnumerator Walk(Path path)
    {
        yield return new WaitForSeconds(.25f);
        foreach (GridItem i in path.steps)
        {
            UpdatePosition(i);
            yield return new WaitForSeconds(.25f * (i.itemType.baseWeight/GridItem.ITEM_TYPE.DIRT.baseWeight));
        }
        yield return new WaitForSeconds(1f);
        grid.StopPathfinding();
    }

    string DebugItems(List<GridItem> path)
    {
        string str = "";
        foreach (GridItem it in path)
            str += "(" + it.X + "," + it.Y + ")";
        return str;
    }

    string DebugItems(List<ItemCost> path)
    {
        string str = "";
        foreach (ItemCost it in path)
            str += "(" + it.gridItem.X + "," + it.gridItem.Y + ")";
        return str;
    }

    List<ItemCost> PathFinderStep(int X, int Y, Path path)
    {
        int smallerCoast = -1;
        List<ItemCost> items = new List<ItemCost>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                if (!IsInsideGrid(X - i, Y - j))
                    continue;

                GridItem _item = grid.grid[X - i][Y - j];
                if (Mathf.Abs(i) == 1 && Mathf.Abs(j) == 1)
                {
                    int cornerX = _item.X + (i == 1 ? -1 : 1);
                    int cornerY = _item.Y + (j == 1 ? -1 : 1);
                    bool hasBlockingCorner = false;
                    if (IsInsideGrid(X, cornerY))
                    {
                        GridItem _corner = grid.grid[X][cornerY];
                        if (_corner.itemType.baseWeight == -1)
                            hasBlockingCorner = true;
                    }
                    if (IsInsideGrid(cornerX, Y))
                    {
                        GridItem _corner = grid.grid[cornerX][Y];
                        if (_corner.itemType.baseWeight == -1)
                            hasBlockingCorner = true;
                    }
                    if (hasBlockingCorner)
                        continue;
                }

                if (_item.itemType.baseWeight == -1)
                    continue;
                if (path.Contains(_item))
                    continue;

                int G = Mathf.Abs(i) == Mathf.Abs(j) && Mathf.Abs(i) == 1 ? _item.itemType.baseWeight + 4 : _item.itemType.baseWeight;
                int cost = G + CalculateH(_item);

                int deltaH = CalculateH(_item) - CalculateH(X, Y);


                if (smallerCoast == -1 || cost < smallerCoast)
                {
                    items.Clear();
                    items.Add(new ItemCost(_item, cost));
                    smallerCoast = cost;
                }
                else if (smallerCoast == cost)// || deltaH > 0)
                {
                    items.Add(new ItemCost(_item, cost));
                }
            }
        }
        return items;
    }

    public void UpdatePosition(GridItem i)
    {
        targetPosition = i.transform.localPosition + Vector3.up * (defaultPosition.y + (i.itemType == GridItem.ITEM_TYPE.WATER ? -.5f : 0));
        X = i.X;
        Y = i.Y;
        //path.Add(i);
    }

    bool IsInsideGrid(int X, int Y)
    {
        return (X >= 0 && X < grid.grid.Length && Y >= 0 && grid.grid.Length > 0 && Y < grid.grid[0].Length);
    }

    private class ItemCost
    {
        public GridItem gridItem;
        public int cost;
        public ItemCost(GridItem item, int cost)
        {
            this.gridItem = item;
            this.cost = cost;
        }
    }

    private class Path
    {
        public List<GridItem> steps;
        public int cost;

        public Path()
        {
            steps = new List<GridItem>();
        }

        public int Size()
        {
            return steps.Count;
        }

        public bool Contains(GridItem item)
        {
            return steps.Contains(item);
        }

        public void Add(GridItem item)
        {
            steps.Add(item);
        }

        public void Clear()
        {
            steps.Clear();
        }
    }
}
