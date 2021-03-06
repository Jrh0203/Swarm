﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockMember : MonoBehaviour {

    private CharacterController cc;

    [Tooltip("Wait time for each call to determine path")]
    [SerializeField] private float flockUpdateTime = 0.2f;

    [Tooltip("FlockSpeed, prety obv. my dood")]
    [SerializeField] private float flockSpeed = 1;

    [Tooltip("Person space bubble, no one shall enter this radius")]
    [SerializeField] private float noEntryRadius = 1;

    [Tooltip("radius for gameobjects it should know about")]
    [SerializeField] private float neighborhoodRadius = 10;

    [SerializeField] private float alignWeight = 1;
    [SerializeField] private float cohWeight = 1;
    [SerializeField] private float sepWeight = 1;
    [SerializeField] private float sepHeavyWeight = 100;
    [SerializeField] private bool distanceModHeavy = true;
    [SerializeField] private float target = 2;
    [SerializeField] private AnimationCurve c;

         
    public Vector3 view;
    public float mag;
    [Tooltip("Only will flock with flockMembers on the same flockLayer")]
    public int flockLayer = 0;

    private Vector3 randDir;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        // StartCoroutine("UpdateFlock");

        // chosen direction
        randDir = (new Vector3((Random.value - .5f) * Time.deltaTime, (Random.value - .5f) * Time.deltaTime, (Random.value - .5f) * Time.deltaTime)).normalized;
    }

    private void Start()
    {
        //cc.Move(Vector3.forward);
    }

    // Update is called once per frame
    void Update () {
        //Vector3 vel = flockSpeed * GetResultant() * Time.deltaTime;
        //cc.Move(vel);
    }

    // Finds all the FlockMembers in a radius around oneself, and sets a list containing all the FlockMember components, as well as 
    // a list of the other colliders in the neighborhoot
    private void GetNeighborhood(float radius, out List<FlockMember> enemies, out List<GameObject> otherCol)
    {
        enemies = new List<FlockMember>();
        otherCol = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider c in hitColliders)
        {
            GameObject g = c.gameObject;
            FlockMember f = g.GetComponent<FlockMember>();
            if (f != null) enemies.Add(f);
            else if (g.transform.tag != "Ground") otherCol.Add(g);
        }
    }

    // returns a vector that steers towards a target 
    private Vector3 targetPos(Vector3 tgt)
    {
        Vector3 desPos = tgt - transform.position;
        desPos = Vector3.Min(desPos, desPos.normalized);
        return desPos.normalized;
    }

    // get the resultant vector
    public Vector3 GetResultant()
    {
        List<FlockMember> enemies;
        List<GameObject> otherCols;

        GetNeighborhood(neighborhoodRadius, out enemies, out otherCols);
        if (enemies.Count == 0) return cc.velocity;
        Vector3 alignment = GetAlignment(enemies);
        Vector3 cohesion = GetCohesion(enemies);
        Vector3 seperation = GetSeperation(enemies, otherCols);
         


        Vector3 resultant = (cohWeight *cohesion + sepWeight * seperation + alignWeight * alignment );
        resultant.Normalize();
        // Vector3 resultant = (GetSepAttr(enemies, otherCols));

            
        // TODO: rot to deg with allignment
        view = resultant;
        mag = resultant.magnitude;
        // resultant.Normalize();
        // resultant -= new Vector3(0, resultant.y, 0);

        return resultant;
    }

    // get the allignment with its neighboring FlockMembers
    private Vector3 GetAlignment(List<FlockMember> neighbors)
    {
        Vector3 total = Vector3.zero;
        foreach (FlockMember f in neighbors)
        {
            total += f.GetComponent<CharacterController>().velocity;
        }
        return cc.velocity - total.normalized;
    }

  
    // get Cohesion comonent for the flock behavior
    private Vector3 GetCohesion(List<FlockMember> neighbors)
    {
        // if it has no neighbors return zero
        if (neighbors.Count == 0) return Vector3.zero;

        // get neighbors average possition
        Vector3 sum = Vector3.zero;
        foreach (FlockMember fm in neighbors)
        {
            sum += fm.transform.position;
        }
        Vector3 avg = sum / neighbors.Count;
        return targetPos(avg);
    }

    // get Seperation compontnet for the flock behavior 
    private Vector3 GetSeperation(List<FlockMember> neighbors, List<GameObject> avoid)
    {
        Vector3 total = Vector3.zero;
        foreach (FlockMember f in neighbors)
        {
            // sum the direction vectors from the current element
            Vector3 rawDir = transform.position - f.transform.position;
            total += rawDir;
           
        }
        foreach (GameObject a in avoid)
        {
            // sum the direction vectors from the current element

            total += transform.position - a.transform.position;
        }                                                                                               
        return total.normalized;
    }

    private Vector3 GetSepAttr(List<FlockMember> neighbors, List<GameObject> avoid)
    {
        Vector3 total = Vector3.zero;
        foreach (FlockMember f in neighbors)
        {
            // sum the direction vectors from the current element
            Vector3 rawDir = f.transform.position - transform.position;
            total += c.Evaluate(rawDir.magnitude / neighborhoodRadius) * (rawDir).normalized;
        }
        foreach (GameObject a in avoid)
        {
            // sum the direction vectors from the current element

            // total += a.transform.position - transform.position;
        }
        return total.normalized;
    }

    IEnumerator UpdateFlock()
    {
        while(true)
        {
            yield return new WaitForSeconds(flockUpdateTime);
            Vector3 vel = flockSpeed * GetResultant();
            cc.Move(vel);
        }
    }
}