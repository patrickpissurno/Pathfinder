using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public static bool LOCK_CAMERA = true;
    private const float OFFSET = .1f;
    private const float SPEED = .1f;
    private Vector3 movDir = Vector3.zero;
	void Update () {
        Vector3 vec = Vector3.zero;
        if (Input.mousePosition.x < OFFSET * Screen.width)
            vec += Vector3.left;
        else if (Input.mousePosition.x > Screen.width * (1f - OFFSET))
            vec += Vector3.right;
        if (Input.mousePosition.y < OFFSET * Screen.height)
            vec += Vector3.down;
        else if (Input.mousePosition.y > Screen.height * (1f - OFFSET))
            vec += Vector3.up;
        movDir = vec;
	}

    void FixedUpdate()
    {
        if(!LOCK_CAMERA)
            transform.Translate(movDir * SPEED);
    }
}
