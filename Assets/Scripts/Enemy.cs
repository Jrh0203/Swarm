﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public float enemySpeed;
	private Vector3 enemyVelocity;

    [Tooltip("HP that the enemy starts With")]
    [SerializeField] private float startHp = 100;
    private float hp;
	private CharacterController controller;
	private bool hit;

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
        enemyDetails.UpdateHealthBar(hp, startHp);
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
