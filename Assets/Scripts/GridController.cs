using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GridController : MonoBehaviour {

    [SerializeField]
    private Transform gridParent;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private Slider widthSlider;

    [SerializeField]
    private Slider heightSlider;

    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    private GameObject sizeMenuUI;

    [SerializeField]
    private GameObject mapMenuUI;

    private static GameObject gridPrefab;
    public GridItem[][] grid;

    [HideInInspector]
    public GridItem startItem = null;
    [HideInInspector]
    public GridItem endItem = null;

    private const float DIST = .1f;

    public bool IsSimulating = false;
    public bool IsUIOpen = true;

	// Use this for initialization
	void Start () {
        LoadResources();
        Generate(4, 4);
	}

    private static void LoadResources()
    {
        if(gridPrefab == null)
            gridPrefab = Resources.Load<GameObject>("Prefabs/Cube");
    }

    public void GridPressed(GridItem item)
    {
        #region GridPressed
        if (!IsSimulating && !IsUIOpen)
        {
            for (int i = 0; i < GridItem.ITEM_TYPE.ITEMS.Length; i++)
            {
                GridItem.ITEM_TYPE.Item type = GridItem.ITEM_TYPE.ITEMS[i];
                if (item.itemType.Equals(type))
                {
                    bool foundNext = false;
                    GridItem.ITEM_TYPE.Item nextItem = null;
                    if (item == startItem)
                        startItem = null;
                    if (item == endItem)
                        endItem = null;

                    while(!foundNext)
                    {
                        int next = i + 1;
                        if (next >= GridItem.ITEM_TYPE.ITEMS.Length)
                            next = 0;
                        nextItem = GridItem.ITEM_TYPE.ITEMS[next];
                        if (nextItem.Equals(GridItem.ITEM_TYPE.START))
                        {
                            if (startItem != null)
                            {
                                i++;
                                continue;
                            }
                            else
                            {
                                startItem = item;
                                foundNext = true;
                                break;
                            }

                        }
                        else if (nextItem.Equals(GridItem.ITEM_TYPE.END))
                        {
                            if (endItem != null)
                            {
                                i++;
                                continue;
                            }
                            else
                            {
                                endItem = item;
                                foundNext = true;
                                break;
                            }

                        }
                        else
                        {
                            foundNext = true;
                            break;
                        }
                    }
                    item.itemType = nextItem;
                    item.Invalidate();
                    break;
                }
            }
        }
        #endregion
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
                grid[i][j].itemType = GridItem.ITEM_TYPE.DIRT;
                grid[i][j].Invalidate();
            }
        }
    }

    public void Reset()
    {
        gridParent.transform.position = Vector3.zero;
        foreach (GridItem[] items in grid)
        {
            if(items != null)
            {
                foreach (GridItem i in items)
                {
                    if (i != null)
                        Destroy(i.gameObject); 
                }
            }
        }
    }

    public void StartPathfinding()
    {
        if (startItem != null && endItem != null)
        {
            canvas.SetActive(false);
            //player.transform.position = startItem.transform.position + Vector3.up * player.defaultPosition.y;
            player.UpdatePosition(startItem);
            player.transform.position = player.targetPosition;
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

    public void ClearGrid()
    {
        foreach (GridItem[] g in grid)
        {
            foreach (GridItem i in g)
            {
                i.itemType = GridItem.ITEM_TYPE.DIRT;
                i.Invalidate();
            }
        }
        startItem = null;
        endItem = null;
    }

    public void SizeChanged(float f)
    {
        if (!IsSimulating)
        {
            Reset();
            Generate((int)widthSlider.value, (int)heightSlider.value);
        }
    }

    public void CloseSizeUI()
    {
        CameraController.LOCK_CAMERA = false;
        sizeMenuUI.SetActive(false);
        mapMenuUI.SetActive(true);
        IsUIOpen = false;
    }
}
