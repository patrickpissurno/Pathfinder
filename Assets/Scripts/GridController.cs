using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridController : MonoBehaviour {

    [SerializeField]
    private Transform gridParent;
    [SerializeField]
    private PlayerController player;

    private GameObject canvas;

    private static GameObject gridPrefab;
    public GridItem[][] grid;

    [HideInInspector]
    public GridItem startItem = null;
    [HideInInspector]
    public GridItem endItem = null;

    private const float DIST = .1f;

    public bool IsSimulating = false;

	// Use this for initialization
	void Start () {
        LoadResources();
        Generate(8, 8);
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
        if (!IsSimulating)
        {
            if (i.itemType == GridItem.ITEM_TYPE.Dirt)
                i.itemType = GridItem.ITEM_TYPE.Wall;
            else if (i.itemType == GridItem.ITEM_TYPE.Wall)
            {
                if (startItem == null)
                {
                    i.itemType = GridItem.ITEM_TYPE.Start;
                    startItem = i;
                }
                else if (endItem == null)
                {
                    i.itemType = GridItem.ITEM_TYPE.End;
                    endItem = i;
                }
                else
                    i.itemType = GridItem.ITEM_TYPE.Dirt;
            }
            else if (i.itemType == GridItem.ITEM_TYPE.Start)
            {
                if (endItem == null)
                {
                    i.itemType = GridItem.ITEM_TYPE.End;
                    endItem = i;
                }
                else
                    i.itemType = GridItem.ITEM_TYPE.Dirt;
                startItem = null;
            }
            else if (i.itemType == GridItem.ITEM_TYPE.End)
            {
                i.itemType = GridItem.ITEM_TYPE.Dirt;
                endItem = null;
            }
            i.Invalidate();
        }
    }

    public void Generate(int width, int height)
    {
        Vector3 basePos = gridParent.transform.position;
        basePos = new Vector3(basePos.x - (width * (1 + DIST) / 2f) + .5f, basePos.y, basePos.z + (height * (1 + DIST) / 2f) - .5f);
        gridParent.position = basePos;
        grid = new GridItem[width][];
        for (int i = 0; i < width; i++)
        {
            grid[i] = new GridItem[height];
            for (int j = 0; j < height; j++)
            {
                GameObject gridObj = Instantiate(gridPrefab, new Vector3(basePos.x + i * (1 + DIST), basePos.y,basePos.z + (-j * (1 + DIST))), Quaternion.identity) as GameObject;
                gridObj.transform.SetParent(gridParent);
                grid[i][j] = gridObj.AddComponent<GridItem>();
                grid[i][j].father = this;
                grid[i][j].X = i;
                grid[i][j].Y = j;
                grid[i][j].itemType = GridItem.ITEM_TYPE.Dirt;
                grid[i][j].Invalidate();
            }
        }
    }

    public void StartPathfinding()
    {
        if (startItem != null && endItem != null)
        {
            canvas = GameObject.Find("Canvas");
            canvas.SetActive(false);
            //player.transform.position = startItem.transform.position + Vector3.up * player.defaultPosition.y;
            player.UpdatePosition(startItem);
            player.X = startItem.X;
            player.Y = startItem.Y;
            player.gameObject.SetActive(true);
            IsSimulating = true;
            player.StartPathfind(this);
        }
        else
            Debug.Log("You must set the starting point and the ending point for the pathfind to work");
    }

    public void StopPathfinding()
    {
        IsSimulating = false;
        canvas.SetActive(true);
        player.gameObject.SetActive(false);
    }
}
