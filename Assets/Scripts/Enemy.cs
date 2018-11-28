using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public float enemySpeed;
	private Vector3 enemyVelocity;
	Vector3[] path;
	int targetIndex;
	public bool drawGizmos;
	private float hp;
	private CharacterController controller;
	private bool hit;
	const float minUpdateTime = .25f;
    [Tooltip("HP that the enemy starts With")]
    [SerializeField] private float startHp = 100;

    // Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
        hp = startHp;
		hit = false;
		StartCoroutine(UpdatePath());
	}
	
	// Update is called once per frame
	void Update () {
		hit = false;
						
		if(hp <= 0) {
			Death();
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
	IEnumerator UpdatePath() {
		Player player = GameManager.Instance.PlayerObj;
		PathRequestManager.RequestPath(new PathRequest(transform.position, player.transform.position, OnPathFound));

		float variance = Random.Range(0, minUpdateTime/2);
		Node playerNodeOld = GameManager.Instance.GridObj.NodeFromWorldPos(player.transform.position);
		while (true) {
			yield return new WaitForSeconds (minUpdateTime/2 + variance);
			Node playerNodeNew = GameManager.Instance.GridObj.NodeFromWorldPos(player.transform.position);
			if (playerNodeNew != playerNodeOld) {
				PathRequestManager.RequestPath (new PathRequest(transform.position, player.transform.position, OnPathFound));
				playerNodeOld = playerNodeNew;
			}
		}
	}

	public void OnPathFound(Vector3[] path, bool pathFound) {
		if(pathFound) {
			this.path = path;
			targetIndex = 0;
		} else {
			print("path not found");
		}
	}
	Vector3 GetMoveDirection() {
		if(path != null && path.Length > targetIndex) {
			Grid grid = GameManager.Instance.GridObj;
			Vector3 currentWaypoint = path[targetIndex];
			Vector3 currentPos = transform.position;
			currentPos.y = 0;
			currentWaypoint.y = 0;
			float distance = Vector3.Distance(currentPos, currentWaypoint);
			if (distance < grid.nodeRadius && targetIndex < path.Length - 1) {
				targetIndex++;
				currentWaypoint = path[targetIndex];
			}
			Vector3 velocity = currentWaypoint - transform.position;
			return Vector3.Normalize(new Vector3(velocity.x, 0, velocity.z));
		} else {
			return Vector3.zero;
		}
	}
	public void OnDrawGizmos() {
		if(drawGizmos) {
			if (path != null) {
				for (int i = targetIndex; i < path.Length; i ++) {
					Grid grid = GameManager.Instance.GridObj;
					Gizmos.color = Color.black;
					Gizmos.DrawCube(path[i], Vector3.one * (grid.nodeRadius * 2 - .1f));
				}
			}
		}
	}

	void OnTriggerEnter(Collider collider) {
		if(collider.gameObject.tag == "Player") {
			Player p = collider.gameObject.GetComponent<Player>();
			p.hit(10.0f);
			Death();	
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

	void Death() {
		GameManager.Instance.EnemiesObj.Remove(this);
		Destroy(gameObject);
	}
}
