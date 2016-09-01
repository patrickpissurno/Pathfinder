using UnityEngine;
using System.Collections;

public class GridItem : MonoBehaviour {
    private static Material Dirt = null;
    private static Material Wall = null;
    private static Material StartM = null;
    private static Material EndM = null;

    public GridController father;
    public ITEM_TYPE itemType = ITEM_TYPE.Dirt;
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
        switch (itemType)
        {
            case ITEM_TYPE.Dirt:
                m = Dirt;
                break;
            case ITEM_TYPE.Wall:
                m = Wall;
                break;
            case ITEM_TYPE.Start:
                m = StartM;
                break;
            case ITEM_TYPE.End:
                m = EndM;
                break;
        }
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

    public enum ITEM_TYPE
    {
        Dirt,
        Wall,
        Start,
        End
    }
}
