using System.Collections;
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
	
	// Update is called once per frame
	void Update () {
        Vector3 vel = flockSpeed * GetResultant() * Time.deltaTime;
        cc.Move(vel);
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
            else otherCol.Add(g);
        }
    }


    // get the resultant vector
    private Vector3 GetResultant()
    {
        List<FlockMember> enemies;
        List<GameObject> otherCols;

        GetNeighborhood(neighborhoodRadius, out enemies, out otherCols);

        Vector3 alignment = GetAlignment(enemies);
        // Vector3 cohesion = GetCohesion(enemies);
        //  Vector3 seperation = GetSeperation(enemies, otherCols);



        // Debug.Log("cohesion = " + cohesion + " | seperation = " + seperation + " | there sum = " + (cohesion + seperation));
        // Debug.Log("cohesions mag = " + cohesion.magnitude + " | serpations mag = " + seperation.magnitude);


        // Vector3 resultant = (cohWeight *cohesion + sepWeight * seperation);
        Vector3 resultant = (GetSepAttr(enemies, otherCols) + randDir);

            
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
        return total.normalized;
    }

    // get Cohesion comonent for the floc behavior
    private Vector3 GetCohesion(List<FlockMember> neighbors)
    {
        Vector3 total = Vector3.zero;
        foreach (FlockMember f in neighbors)
        {
            // sum the direction vectors from the current element
            Vector3 rawDir = f.transform.position - transform.position;
            total += Mathf.Pow(rawDir.magnitude, 2) * (rawDir).normalized;
        }
        return total.normalized;
    }

    // get Seperation compontnet for the flock behavior 
    private Vector3 GetSeperation(List<FlockMember> neighbors, List<GameObject> avoid)
    {
        Vector3 total = Vector3.zero;
        foreach (FlockMember f in neighbors)
        {
            // sum the direction vectors from the current element
            Vector3 rawDir = transform.position - f.transform.position;
            total += (Mathf.Pow(rawDir.magnitude + 2, 2)) * (rawDir).normalized;
           
        }
        foreach (GameObject a in avoid)
        {
            // sum the direction vectors from the current element

            // total += a.transform.position - transform.position;
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