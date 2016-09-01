using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridController : MonoBehaviour {

    [SerializeField]
    private Transform gridParent;
    private static GameObject gridPrefab;
    private GridItem[][] grid;

    private const float DIST = .1f;

	// Use this for initialization
	void Start () {
        LoadResources();
        Generate(4, 4);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private static void LoadResources()
    {
        if(gridPrefab == null)
            gridPrefab = Resources.Load<GameObject>("Prefabs/Cube");
    }

    public void GridPressed(GridItem i)
    {
        Debug.Log(i.X + " " + i.Y);
        i.IsWall = !i.IsWall;
        i.Invalidate();
    }

    public void Generate(int width, int height)
    {
        grid = new GridItem[width][];
        for (int i = 0; i < width; i++)
        {
            grid[i] = new GridItem[height];
            for (int j = 0; j < height; j++)
            {
                GameObject gridObj = Instantiate(gridPrefab, new Vector3(i * (1 + DIST), 0, -j * (1 + DIST)), Quaternion.identity) as GameObject;
                gridObj.transform.SetParent(gridParent);
                grid[i][j] = gridObj.AddComponent<GridItem>();
                grid[i][j].father = this;
                grid[i][j].X = i;
                grid[i][j].Y = j;
                grid[i][j].IsWall = false;
                grid[i][j].Invalidate();
            }
        }
    }
}
