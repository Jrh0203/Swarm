using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {
    /*Note: Position should be based on player control
    Rotation should be set at X: 45, Y: 135 Z:0*/
    
    //relative to the player
    private Vector3 cameraDistVect;
	private float smoothSpeed = .075f;
    public int DEFAULT_CAMERA_DIST = 50;
    private bool usingSniper = false;

	// Use this for initialization
	void Start () {
        //move the camera away from the player in the same direction it faces.
        cameraDistVect = GameObject.FindWithTag("Player").transform.up * DEFAULT_CAMERA_DIST;
	}
	
    public void SetSniperView(bool shouldPan) {
        usingSniper = shouldPan;
    }

	void LateUpdate () {
        //this.transform.position = cameraDistVect + GameObject.FindWithTag("Player").transform.position;
        Vector3 desiredPos;
        if (usingSniper) {
            float sniperZoomRatio = 1.25f;
            desiredPos = cameraDistVect * sniperZoomRatio + GameObject.FindWithTag("Player").transform.position + GameObject.FindWithTag("Player").transform.forward * 20;
        }
        else {
		    desiredPos = cameraDistVect + GameObject.FindWithTag("Player").transform.position;
        }
		Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
		transform.position = smoothedPos;
    }
}
