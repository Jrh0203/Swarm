
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    private float cubeSize = 0.2f;
    private int cubesInRow = 5;
    private static int maxDeathCubes = 1000;
    public GameObject enemyBit;

    float cubesPivotDistance;
    Vector3 cubesPivot;

    private float explosionForce = 100f;
    private float explosionRadius = 4f;
    private float explosionUpward = 0.4f;
    float shotTimer;

    

    // Use this for initialization
    void Start() {

        shotTimer = 0.0f;
        //calculate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //use this value to create pivot vector)
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
        
    }

    // Update is called once per frame
    void Update() {
        /*
        shotTimer+=Time.deltaTime;
        if (shotTimer>3){
            shotTimer = 3.0f;
            explode();
        }
        */
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "Enemy") {
            explode();
        }

    }

    public void explode() {
        //make object disappear
        gameObject.SetActive(false);

        //loop 3 times to create 5x5x5 pieces in x,y,z coordinates
        for (int x = 0; x < cubesInRow; x++) {
            for (int y = 0; y < cubesInRow; y++) {
                for (int z = 0; z < cubesInRow; z++) {
                    createPiece(x, y, z);
                }
            }
        }

        //get explosion position
        Vector3 explosionPos = transform.position;
        //get colliders in that position and radius
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        //add explosion force to all colliders in that overlap sphere
        foreach (Collider hit in colliders) {
            //get rigidbody from collider object
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null) {
                //add explosion force to this body with given parameters
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
            }
        }

    }

    void createPiece(int x, int y, int z) {
        //set piece position and scale
        GameObject piece = Instantiate(enemyBit, transform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot, Quaternion.identity);
        GameManager.cubes.Add(piece);
        if (GameManager.cubes.Count>maxDeathCubes){
            GameObject temp = GameManager.cubes[0];
            GameManager.cubes.RemoveAt(0);
            Destroy(temp);
        }
    }

}