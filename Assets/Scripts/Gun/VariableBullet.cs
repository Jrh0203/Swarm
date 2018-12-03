using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableBullet : MonoBehaviour
{
    [Tooltip("The lifetime in ms of a bullet")]
    [SerializeField] private float lifeTime = 100;

    [Tooltip("How fast the bullet shoots")]
    [SerializeField] private float shootSpeed = 20;

    [Tooltip("Does the bullet die on collision?")]
    [SerializeField] private bool dieOnCol = true;

    [Tooltip("Function of velocity")]
    [SerializeField] private AnimationCurve c;

    private float currentTime;
    private Rigidbody rB;

    private float bulletDamage;

    // used when object is awoken
    private void Awake()
    {
        rB = GetComponent<Rigidbody>();
        // use awake instead of start in case we want to use a bullet pool
        currentTime = 0;

        bulletDamage = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        rB.velocity = transform.forward * shootSpeed * c.Evaluate(currentTime / lifeTime);
        // check lifetime
        currentTime += Time.deltaTime;
        if (currentTime >= lifeTime) Seppuku();

        // 

    }

    // Method that destroys the bullet
    public void Seppuku()
    {
        Debug.Log("sepuk");
        Destroy(gameObject);
    }

    public float getDamage()
    {
        return bulletDamage;
    }
    void OnTriggerEnter(Collider collider)
    {
        if (dieOnCol) Seppuku();
    }
}
