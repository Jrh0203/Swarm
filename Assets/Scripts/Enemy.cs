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

    [Tooltip("HP that the enemy starts With")]
    [SerializeField] private float startHp = 100;

    PlayerDetails enemyDetails;

    // Flocking stuff
    [Tooltip("Person space bubble, no one shall enter this radius")]
    [SerializeField] private float noEntryRadius = 1;

    [Tooltip("radius for gameobjects it should know about")]
    [SerializeField] private float neighborhoodRadius = 10;

    [SerializeField] private float alignWeight = 1;
    [SerializeField] private float cohWeight = 1;
    [SerializeField] private float sepWeight = 1;
    [SerializeField] private float sepHeavyWeight = 100;
    [SerializeField] private bool distanceModHeavy = true;

    // Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
        enemyDetails = GetComponentInChildren<PlayerDetails>();
        hp = startHp;
		hit = false;
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
	public void UpdatePath(int idx) {
		
		Player player = GameManager.Instance.PlayerObj;
		Grid grid = GameManager.Instance.GridObj;
		List<Node> circleSpots = GameManager.Instance.CircleSpotsObj;
		Vector3[] rPath = grid.FindPath(transform.position, player.transform.position);
		float bestDist = 100000.0f;
		Vector3 bestPos = new Vector3(0,0,0);
		Node bestNode = circleSpots[0];
		int maxCapacity = (int)(GameManager.Instance.EnemiesObj.Count/circleSpots.Count)+1;
		if (circleSpots.Count>0){
			foreach (Node n in circleSpots){
				if (n.capacity<maxCapacity){
					Vector3 pos = grid.WorldFromNodeXY(n.gridX,n.gridY);
					float dist = (transform.position-pos).magnitude;
					if (dist<bestDist){
						bestDist = dist;
						bestPos = pos;
						bestNode = n;
					}
				}
			}
			bestNode.capacity+=1;
			
			rPath = grid.FindPath(transform.position, bestPos);
		}
		
		//
		
		if(rPath != null) {
			path = rPath;
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

    // Finds all the enemys in a radius around oneself, and sets a list containing all the enemy components, as well as 
    // a list of the other colliders in the neighborhoot
    private void GetNeighborhood(float radius, out List<Enemy> enemies, out List<GameObject> otherCol)
    {
        enemies = new List<Enemy>();
        otherCol = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider c in hitColliders)
        {
            GameObject g = c.gameObject;
            Enemy e = g.GetComponent<Enemy>();
            if (e != null) enemies.Add(e);
            else otherCol.Add(g);
        }
    }


    // get the resultant vector
    private Vector3 GetResultant()
    {
        List<Enemy> enemies;
        List<GameObject> otherCols;

        GetNeighborhood(neighborhoodRadius, out enemies, out otherCols);

        Vector3 alignment = GetAlignment(enemies);
        Vector3 cohesion = GetCohesion(enemies);
        Vector3 seperation = GetSeperation(enemies, otherCols);

        Vector3 resultant = (alignment + cohesion + seperation);
        resultant.Normalize();

        return resultant;
    }

    // get the allignment with its neighboring enemys
    private Vector3 GetAlignment(List<Enemy> neighbors)
    {
        Vector3 total = Vector3.zero;
        foreach (Enemy e in neighbors)
        {
            total += e.GetComponent<CharacterController>().velocity;
        }
        Vector3 average = total / neighbors.Count;
        return alignWeight * average;
    }

    // get Cohesion comonent for the floc behavior
    private Vector3 GetCohesion(List<Enemy> neighbors)
    {
        return Vector3.zero;
    }

    // get Seperation compontnet for the flock behavior 
    private Vector3 GetSeperation(List<Enemy> neighbors, List<GameObject> avoid)
    {
        return Vector3.zero;
    }
}
