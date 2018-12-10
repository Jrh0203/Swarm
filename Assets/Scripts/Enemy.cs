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
    private FlockMember fm;
    public bool isRanger;

    [Tooltip("radius where the enemy will straif to move to its possition")]
    [SerializeField] float straifRadius = 1;

    [Tooltip("the speed at witch the enemy rotates")]
    [SerializeField] float rotSpeed = 1;


    [Tooltip("HP that the enemy starts With")]
        
    PlayerDetails enemyDetails;

	//constant for polling rate: increase to improve performance, but decrease rate at which paths are updated.
	const float minUpdateTime = 1.0f;
    [SerializeField] private float startHp = 20;

    private Explosion explode;

    // Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
        enemyDetails = GetComponentInChildren<PlayerDetails>();
        fm = GetComponent<FlockMember>();
        hp = startHp;
		hit = false;
		StartCoroutine(UpdatePath());
		explode = GetComponent<Explosion>();

		isRanger = false;
		TurretShoot rangerTest = GetComponentInChildren<TurretShoot>();
		if (rangerTest != null)
		{
		    isRanger = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		hit = false;
						
		if(hp <= 0) {
			explode.explode();
			Death();
		}
        Vector3 flockDir = fm.GetResultant();
        flockDir = Vector3.Min(flockDir, flockDir.normalized);
		Vector3 moveDir = GetMoveDirection();

        Vector3 finalDir = (moveDir + flockDir);
        GoToTarget(finalDir);

        Vector3 pos = transform.position;
        Vector3 playerPos = GameManager.Instance.PlayerObj.transform.position;
        float dist = (playerPos-pos).magnitude;
        if (dist<1.9){
        	explode.explode();
			GameManager.Instance.PlayerObj.hurt(10.0f);
			Death();
        }
	}

	IEnumerator UpdatePath() {
		Player player = GameManager.Instance.PlayerObj;
		Grid grid = GameManager.Instance.GridObj;

		float variance = Random.Range(0, minUpdateTime);
		Node oldPlayerNode = null;
		while (true) {
			yield return new WaitForSeconds (variance);
			//update the path if the player has moved within the last update time
			Node newPlayerNode = grid.NodeFromWorldPos(player.transform.position);
			if (oldPlayerNode == null || oldPlayerNode != newPlayerNode) {
				Node dest = findClosestCircleNode();
				Vector3 newPos;
				if(dest != null && isRanger == true) {
					newPos = grid.WorldFromNodeXY(dest.gridX, dest.gridY);
				} else {
					newPos = player.transform.position;
				}
				PathRequestManager.RequestPath (new PathRequest(transform.position, newPos, OnPathFound));
				oldPlayerNode = newPlayerNode;
			}
			yield return new WaitForSeconds (minUpdateTime - variance);
		}
	}

	public Node findClosestCircleNode() {
		Player player = GameManager.Instance.PlayerObj;
		Grid grid = GameManager.Instance.GridObj;
		HashSet<Node> circleSpots = GameManager.Instance.CircleSpotsObj;
		if(circleSpots.Count <= 0) {
			return null;
		}

		int maxCapacity = (int)(GameManager.Instance.EnemiesObj.Count/circleSpots.Count)+1;
		float bestDist = float.MaxValue;
		Vector3 bestPos;
		Node bestNode = null;
		
		foreach (Node n in circleSpots){
			if (n.capacity < maxCapacity){
				Vector3 pos = grid.WorldFromNodeXY(n.gridX, n.gridY);
				float dist = (transform.position - pos).magnitude;
				if (dist < bestDist){
					bestDist = dist;
					bestPos = pos;
					bestNode = n;
				}
			}
		}
		if(bestNode != null) {
			bestNode.capacity++;
		} else {
			print("no circle spots" + GameManager.Instance.EnemiesObj.Count + " " + circleSpots.Count + " " + maxCapacity);
		}
		return bestNode;
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
			p.hurt(10.0f);
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

		GameManager gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();;

		gm.countDecrease();
	}

    public Vector3 GoToTarget(Vector3 tgt)
    {
        Vector3 curr = transform.forward;
        Vector3 finalDir;
        if ((tgt - curr).magnitude <= straifRadius)
        {
            finalDir = tgt;
        }
        else
        {
            Quaternion rot = Quaternion.RotateTowards(Quaternion.LookRotation(controller.velocity), Quaternion.LookRotation(tgt), rotSpeed);
            finalDir = rot * Vector3.forward;
        }
        Vector3 moveVelocity = finalDir * Time.deltaTime * enemySpeed;

        enemyVelocity.x = moveVelocity.x;
        enemyVelocity.z = moveVelocity.z;
        enemyVelocity += Physics.gravity * Time.deltaTime;
        if (controller.isGrounded)
        {
            enemyVelocity.y = 0;
        }

        controller.Move(enemyVelocity);
        return enemyVelocity;
    }

	public void TakeDamage(float damage) {
		hp -= damage;
	}
}
