using UnityEngine;
using System.Collections;

public class GridItem : MonoBehaviour {
    private static Material Dirt = null;
    private static Material Wall = null;
    private static Material StartM = null;
    private static Material EndM = null;

    public GridController father;
    public ITEM_TYPE.Item itemType = ITEM_TYPE.DIRT;
    public int X;
    public int Y;

    private MeshRenderer renderer;

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
        if (itemType == ITEM_TYPE.DIRT)
            m = Dirt;
        else if (itemType == ITEM_TYPE.WALL)
            m = Wall;
        else if (itemType == ITEM_TYPE.START)
            m = StartM;
        else if (itemType == ITEM_TYPE.END)
            m = EndM;
        if(m != null)
            renderer.material = m;
    }

    void OnMouseDown()
    {
        father.GridPressed(this);
    }

    void LoadResources()
    {
        if (Dirt == null)
            Dirt = Resources.Load<Material>("Materials/Dirt");
        if (Wall == null)
            Wall = Resources.Load<Material>("Materials/Wall");
        if (StartM == null)
            StartM = Resources.Load<Material>("Materials/Start");
        if (EndM == null)
            EndM = Resources.Load<Material>("Materials/End");
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
        public static Item WALL = new Item(-1);
        public static Item START = new Item(-1);
        public static Item END = new Item(10);
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
