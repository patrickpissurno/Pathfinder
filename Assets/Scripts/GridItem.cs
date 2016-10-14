using UnityEngine;
using System.Collections;

public class GridItem : MonoBehaviour {
    private static Material Dirt = null;
    private static Material Water = null;
    private static Material Wall = null;
    private static Material StartM = null;
    private static Material EndM = null;
    private static GameObject WaterFX = null;

    public GridController father;
    public ITEM_TYPE.Item itemType = ITEM_TYPE.DIRT;
    public int X;
    public int Y;

    private MeshRenderer renderer;

    private GameObject fx = null;

    void Start()
    {
        LoadResources();
        LoadComponents();
    }

    public void Invalidate()
    {
        LoadResources();
        LoadComponents();
        Material m = null;
        SetHeight(0);
        if (fx != null)
            Destroy(fx);
        if (itemType == ITEM_TYPE.DIRT)
            m = Dirt;
        else if (itemType == ITEM_TYPE.WATER)
        {
            m = Water;
            fx = Instantiate(WaterFX, transform.position + (Vector3.up * .49f), Quaternion.Euler(90,transform.rotation.eulerAngles.y,0)) as GameObject;
            fx.transform.SetParent(transform);
        }
        else if (itemType == ITEM_TYPE.WALL)
        {
            m = Wall;
            SetHeight(1);
        }
        else if (itemType == ITEM_TYPE.START)
            m = StartM;
        else if (itemType == ITEM_TYPE.END)
            m = EndM;
        if(m != null)
            renderer.material = m;
    }

    private void SetHeight(int h)
    {
        transform.position = new Vector3(transform.position.x, h * .9f, transform.position.z);
    }

    void OnMouseDown()
    {
        if (!Input.GetMouseButton(2) && !Input.GetKey(KeyCode.LeftAlt))
            father.GridPressed(this);
    }

    void LoadResources()
    {
        if (Dirt == null)
            Dirt = Resources.Load<Material>("Materials/Dirt");
        if (Wall == null)
            Wall = Resources.Load<Material>("Materials/Wall");
        if (Water == null)
            Water = Resources.Load<Material>("Materials/Water");
        if (StartM == null)
            StartM = Resources.Load<Material>("Materials/Start");
        if (EndM == null)
            EndM = Resources.Load<Material>("Materials/End");
        if (WaterFX == null)
            WaterFX = Resources.Load<GameObject>("Prefabs/WaterFX");
    }

    void LoadComponents()
    {
        if(renderer == null)
            renderer = GetComponent<MeshRenderer>();
    }

    //public enum ITEM_TYPE
    //{
    //    Dirt,
    //    Wall,
    //    Start,
    //    End
    //}

    public static class ITEM_TYPE
    {
        public static Item DIRT = new Item(10);
        public static Item WATER = new Item(20);
        public static Item WALL = new Item(-1);
        public static Item START = new Item(-1);
        public static Item END = new Item(10);
        public static Item[] ITEMS = new Item[] { DIRT, WATER, WALL, START, END };
        public class Item
        {
            public int baseWeight = 10;
            public Item(int weight)
            {
                this.baseWeight = weight;
            }
        }
    }

}
