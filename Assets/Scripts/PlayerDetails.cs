using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetails : MonoBehaviour {
    [Tooltip("What camera should this canvas face")]
    [SerializeField] Camera mainCamera;


	// Use this for initialization
	void Start () {
        // if we havn't set the camera grab it
        if (mainCamera = null) mainCamera;

	}
	
	// Update is called once per frame
	void Update () {
        
        // face canvas towards the camera
        Vector3 v = mainCamera.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(mainCamera.transform.position - v);
        transform.Rotate(0, 180, 0);
        
    }
}
