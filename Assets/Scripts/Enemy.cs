using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public float enemySpeed;
	private Vector3 enemyVelocity;
	private float hp;
	private CharacterController controller;
	private bool hit;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		hp = 100.0f;
		hit = false;
	}
	
	// Update is called once per frame
	void Update () {
		hit = false;
						
		if(hp <= 0) {
			Destroy(gameObject);
		}
		Vector3 moveDir = GetMoveDirection();
		Vector3 moveVelocity = moveDir * Time.deltaTime * enemySpeed;

		enemyVelocity.x = moveVelocity.x;
		enemyVelocity.z = moveVelocity.z;
		enemyVelocity += Physics.gravity * Time.deltaTime;
		if(controller.isGrounded) {
			enemyVelocity.y = 0;
		}

		controller.Move(enemyVelocity);
	}

	Vector3 GetMoveDirection() {
		return Vector3.Normalize(GameObject.FindWithTag("Player").transform.position - transform.position);
	}

	void OnTriggerEnter(Collider collider) {
		if(collider.gameObject.tag == "Player") {
			Player p = collider.gameObject.GetComponent<Player>();
			p.hit(10.0f);
			Debug.Log("attacked");
			Destroy(gameObject);
		}
        if(collider.gameObject.layer == LayerMask.NameToLayer("Bullet")) {
			if(!hit) {
				Bullet b = collider.gameObject.GetComponent<Bullet>();
				hit = true;
				hp -= b.getDamage();
				b.Seppuku();
			}
		}
    }
}
