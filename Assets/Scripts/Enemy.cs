using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public float enemySpeed;
	public float threshold;
	private Vector3 enemyVelocity;
	Vector3[] path;
	int targetIndex;
	public bool drawGizmos;
	private float hp;
	private CharacterController controller;
	private bool hit;

	private bool playerMoved;
	private Node playerNode;
	private Grid grid;
	private GameObject player;

    [Tooltip("HP that the enemy starts With")]
    [SerializeField] private float startHp = 100;

    PlayerDetails enemyDetails;

   

    // Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		grid = GameObject.FindWithTag("WorldGrid").GetComponent<Grid>();
		player = GameObject.FindWithTag("Player");
        enemyDetails = GetComponentInChildren<PlayerDetails>();
        hp = startHp;
		hit = false;
	}
	
	// Update is called once per frame
	void Update () {
		hit = false;

		Node newNode = grid.NodeFromWorldPos(player.transform.position);
		if(playerNode != newNode) {
			PathRequestManager.RequestPath(transform.position, player.transform.position, OnPathFound);
		}
		playerNode = newNode;
						
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
		if(path != null) {
			Vector3 currentWaypoint = path[targetIndex];
			Vector3 currentPos = transform.position;
			currentPos.y = 0;
			currentWaypoint.y = 0;
			float distance = Vector3.Distance(currentPos, currentWaypoint);
			if (distance < threshold) {
				targetIndex ++;
				currentWaypoint = path[targetIndex];
			}
			Vector3 velocity = currentWaypoint - transform.position;
			return Vector3.Normalize(new Vector3(velocity.x, 0, velocity.z));
		} else {
			return Vector3.zero;
		}
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if(pathSuccessful && newPath != null) {
			path = newPath;
			targetIndex = 0;
		}
	}
	public void OnDrawGizmos() {
		if(drawGizmos) {
			if (path != null) {
				for (int i = targetIndex; i < path.Length; i ++) {
					Gizmos.color = Color.black;
					Gizmos.DrawCube(path[i], Vector3.one);

					if (i == targetIndex) {
						Gizmos.DrawLine(transform.position, path[i]);
					}
					else {
						Gizmos.DrawLine(path[i-1],path[i]);
					}
				}
			}
		}
	}

	void OnTriggerEnter(Collider collider) {
		if(collider.gameObject.tag == "Player") {
			Player p = collider.gameObject.GetComponent<Player>();
			p.hit(10.0f);
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
        //enemyDetails.UpdateHealthBar(hp, startHp);
    }
}
