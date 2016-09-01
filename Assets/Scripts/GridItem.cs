using UnityEngine;
using System.Collections;

public class GridItem : MonoBehaviour {
    private static Material Dirt = null;
    private static Material Wall = null;

    public GridController father;
    public bool IsWall = false;
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
        renderer.material = IsWall ? Wall : Dirt;
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
    }

    void LoadComponents()
    {
        if(renderer == null)
            renderer = GetComponent<MeshRenderer>();
    }
}
