using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public float playerSpeed;
	private Vector3 playerVelocity;
	private CharacterController controller;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		playerVelocity = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 moveDir = GetMoveDirection();
		Vector3 moveVelocity = moveDir * Time.deltaTime * playerSpeed;
		
		//set velocity in x and z plane
		playerVelocity.x = moveVelocity.x;
		playerVelocity.z = moveVelocity.z;

		//set velocity in the y direction
		playerVelocity += Physics.gravity * Time.deltaTime;
		if(controller.isGrounded) {
			playerVelocity.y = 0;
		}

		controller.Move(playerVelocity);
	}

	Vector3 GetMoveDirection() {
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		moveInput = Vector3.ClampMagnitude(moveInput, 1);
		return moveInput;
	}
}
