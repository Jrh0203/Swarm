using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	private Vector3 enemyVelocity;
	private float hp;
	private CharacterController controller;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		hp = 100.0f;
	}
	
	// Update is called once per frame
	void Update () {
		enemyVelocity += Physics.gravity * Time.deltaTime;
		if(controller.isGrounded) {
			enemyVelocity.y = 0;
		}

		controller.Move(enemyVelocity);
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
        if(hit.gameObject.layer == LayerMask.NameToLayer("Bullet")) {
			hp -= 10.0f;
		}
    }
}
