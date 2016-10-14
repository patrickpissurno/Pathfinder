using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public static bool LOCK_CAMERA = true;
    private const float OFFSET = .1f;
    private const float SPEED = .1f;
    private Vector3 movDir = Vector3.zero;

    private bool isMoving = false;
    private Vector3 movingStartPos = Vector3.zero;
    private Vector3 mouseOffset = Vector3.zero;
	void Update () {
        //Vector3 vec = Vector3.zero;
        //if (Input.mousePosition.x < OFFSET * Screen.width)
        //    vec += Vector3.left;
        //else if (Input.mousePosition.x > Screen.width * (1f - OFFSET))
        //    vec += Vector3.right;
        //if (Input.mousePosition.y < OFFSET * Screen.height)
        //    vec += Vector3.down;
        //else if (Input.mousePosition.y > Screen.height * (1f - OFFSET))
        //    vec += Vector3.up;
        //movDir = vec;
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
        {
            isMoving = true;
            movingStartPos = transform.position;
            mouseOffset = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
        }
	}

    void FixedUpdate()
    {
        //if(!LOCK_CAMERA)
        //    transform.Translate(movDir * SPEED);
        if (isMoving)
        {
            Vector3 pos = Input.mousePosition - mouseOffset;
            transform.position = movingStartPos + (-new Vector3(pos.x, 0, pos.y) * .01f);
        }
    }
}
