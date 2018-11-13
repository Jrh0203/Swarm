using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetails : MonoBehaviour {
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        // face canvas towards the camera
        transform.LookAt(transform.position + UnityEngine.Camera.main.transform.rotation * Vector3.back,
            UnityEngine.Camera.main.transform.rotation * Vector3.up);
        
    }
}
