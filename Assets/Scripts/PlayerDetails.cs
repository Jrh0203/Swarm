using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetails : MonoBehaviour {
    [Tooltip("What camera should this canvas face")]
    [SerializeField] private Camera mainCamera;


    // Use this for initialization
    void Start () {
        // if we havn't set the camera grab it
        // if (mainCamera == null) mainCamera = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {

        // face canvas towards the camera
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.back,
            mainCamera.transform.rotation * Vector3.up);
        
    }
}
