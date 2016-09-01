using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public Vector3 defaultPosition;
	void Start () {
        defaultPosition = transform.position;
        gameObject.SetActive(false);
	}
	
	void Update () {
	
	}
}
