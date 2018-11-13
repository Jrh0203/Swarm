using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFinding : MonoBehaviour {

	public Transform goal;
    private UnityEngine.AI.NavMeshAgent agent;

	void Start () {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		InvokeRepeating("UpdateLocation", 0.0f, 0.3f); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void UpdateLocation() {
		agent.destination = goal.position;
	}
}
