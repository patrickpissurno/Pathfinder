using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    private GridController GridController;
	// Use this for initialization
	IEnumerator Start () {
        GridController = GameObject.Find("GridController").GetComponent<GridController>();
        yield return new WaitForSeconds(.05f);
        transform.position = GridController.grid[0][0].transform.position + transform.position.y * Vector3.up;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
