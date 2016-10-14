using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public static bool LOCK_CAMERA = true;
    private const float OFFSET = .1f;
    private const float SPEED = .1f;
    private Vector3 movDir = Vector3.zero;

    private bool isPositionMoving = false;
    private bool isRotationMoving = false;

    private Vector3 movingStartPos = Vector3.zero;
    private Vector3 movingStartRot = Quaternion.identity.eulerAngles;
    private Vector3 mouseOffsetPos = Vector3.zero;
    private Vector3 mouseOffsetRot = Vector3.zero;

    public Transform[] cameras;

    private Vector3[] defaultRotation;
    private Vector3[] defaultPosition;

    public float targetHeight = 0f;

    private Vector3 mousePosPos = Vector3.zero;
    private Vector3 mouseRotPos = Vector3.zero;

    private Quaternion targetRotation;

    public int camera = 0;

    public Transform gridParent;

    void Start()
    {
        defaultPosition = new Vector3[cameras.Length];
        defaultRotation = new Vector3[cameras.Length];
        for (int i = 0; i < cameras.Length; i++)
        {
            defaultPosition[i] = cameras[i].position;
            defaultRotation[i] = cameras[i].rotation.eulerAngles;
        }
        movingStartPos = defaultPosition[0];
        movingStartRot = gridParent.rotation.eulerAngles;
        targetHeight = defaultPosition[0].y;

        targetRotation = transform.rotation;
    }

	void Update () {
        if (Input.GetMouseButtonDown(2))
        {
            isPositionMoving = true;
            movingStartPos = transform.position;
            mouseOffsetPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isPositionMoving = false;
        }
        else if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
        {
            isRotationMoving = true;
            mouseOffsetRot = Input.mousePosition;
            movingStartRot = gridParent.rotation.eulerAngles;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isRotationMoving = false;
        }

        if(!isPositionMoving)
            targetHeight += -Input.GetAxis("Mouse ScrollWheel");
	}

    void FixedUpdate()
    {
        if(isPositionMoving)
            mousePosPos = Input.mousePosition - mouseOffsetPos;
        if(isRotationMoving)
            mouseRotPos = (Input.mousePosition - mouseOffsetRot) * -Mathf.Sign(Input.mousePosition.x - Screen.width / 2);
        transform.position = Vector3.Lerp(transform.position, new Vector3(movingStartPos.x, targetHeight, movingStartPos.z) + (-new Vector3(mousePosPos.x, 0, mousePosPos.y) * .01f), .2f);
        gridParent.transform.rotation = Quaternion.Lerp(gridParent.transform.rotation, Quaternion.Euler(movingStartRot.x, FixR(movingStartRot.y + (mouseRotPos.y/5f)), movingStartRot.z), .2f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .2f);
    }

    float FixR(float r)
    {
        while (r >= 360)
            r -= 360;
        while (r < 0)
            r += 360;
        return r;
    }

    public void ChangeCamera()
    {
        int lastCamera = camera;
        camera++;
        if (camera >= cameras.Length)
            camera = 0;
        movingStartPos = defaultPosition[camera];
        targetHeight = defaultPosition[camera].y + targetHeight - defaultPosition[lastCamera].y;
        targetRotation = Quaternion.Euler(defaultRotation[camera]);
    }
}
