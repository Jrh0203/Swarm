using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	private static GameManager instance;

	public static GameManager Instance {
		get {
			return instance;
		}
	}

	HashSet<Enemy> enemies;
	Player player;
	Grid grid;
	HashSet<Node> circleSpots;

	public static List<GameObject> cubes;

	private Node oldPlayerNode;

	void Awake () {
		cubes = new List<GameObject>();
		if(instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
		
		GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
		enemies = new HashSet<Enemy>();
		foreach(GameObject enemyObj in enemyObjects) {
			enemies.Add(enemyObj.GetComponent<Enemy>());
		}
		circleSpots = new HashSet<Node>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
	}

	public Player PlayerObj {
		get {
			return player;
		}
	}

	public HashSet<Enemy> EnemiesObj {
		get {
			return enemies;
		}
	}

	public Grid GridObj {
		get {
			return grid;
		}
	}

	public HashSet<Node> CircleSpotsObj {
		get {
			return circleSpots;
		}
	}
	
	// Update is called once per frame
	void Update () {

		/*
		GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
		enemies = new HashSet<Enemy>();
		foreach(GameObject enemyObj in enemyObjects) {
			enemies.Add(enemyObj.GetComponent<Enemy>());
		}
		*/
		
		if(grid != null) {
			Node newNode = grid.NodeFromWorldPos(player.transform.position);
			if(oldPlayerNode == null || oldPlayerNode != newNode) {
				//grid.UpdateCover();
				circleSpots = grid.UpdateBattleCircle();
				oldPlayerNode = newNode;
				//grid.UpdateCover();
			}
		}
	}
}
