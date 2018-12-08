using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public bool defeat;
	public bool victory;
	public int remainingEnemies;

	private bool isPaused;
	private static GameManager instance;
	public GameObject defeatPrefab;
	public GameObject victoryPrefab;
	public static GameManager Instance {
		get {
			return instance;
		}
	}

	HashSet<Enemy> enemies;
	Player player;
	Grid grid;
	HashSet<Node> circleSpots;
	Canvas hud;

	public static List<GameObject> cubes;

	private Node oldPlayerNode;

	void Awake () {
		isPaused = false;
		defeat = false;
		victory = false;
		cubes = new List<GameObject>();
		if(instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>();
		GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
		enemies = new HashSet<Enemy>();
		foreach(GameObject enemyObj in enemyObjects) {
			enemies.Add(enemyObj.GetComponent<Enemy>());
		}
		circleSpots = new HashSet<Node>();
		grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
	}

	public Player PlayerObj {
		get {
			return player;
		}
	}

	public int EnemiesLeft {
		get {
			return remainingEnemies;
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
		if(player.hp <= 0) {
			defeat = true;
		}
		if(grid != null) {
			Node newNode = grid.NodeFromWorldPos(player.transform.position);
			if(oldPlayerNode == null || oldPlayerNode != newNode) {
				grid.UpdateCover();
				circleSpots = grid.UpdateBattleCircle();
				oldPlayerNode = newNode;
				//grid.UpdateCover();
			}
		}
	}

	void OnGUI() {
		if(victory && !isPaused) {
			Time.timeScale = 0;
			Instantiate(victoryPrefab, new Vector3(Screen.width * .5f, 0, Screen.height * .5f), Quaternion.identity, hud.transform);
			isPaused = true;
		} else if (defeat && !isPaused){
			Time.timeScale = 0;
			Instantiate(defeatPrefab, new Vector3(Screen.width * .5f, Screen.height * .5f, 0), Quaternion.identity, hud.transform);
			isPaused = true;
		}
	}
}
