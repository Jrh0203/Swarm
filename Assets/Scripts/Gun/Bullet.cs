using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [Tooltip("The lifetime in ms of a bullet")]
    [SerializeField] private float lifeTime = 1000;

    [Tooltip("How fast the bullet shoots")]
    [SerializeField] private float shootSpeed = 10;

    [Tooltip("Does the bullet die on collision?")]
    [SerializeField] private bool dieOnCol = true;

    private float currentTime;
    private Rigidbody rB;

    // used when object is awoken
    private void Awake()
    {
        rB = GetComponent<Rigidbody>();
        // use awake instead of start in case we want to use a bullet pool
        currentTime = 0;

        rB.velocity = transform.forward * shootSpeed;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // check lifetime
        currentTime += Time.deltaTime;
        if (currentTime >= lifeTime) Seppuku();

        // 

	}

    // Method that destroys the bullet
    void Seppuku()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (dieOnCol) Seppuku();
    }
}
