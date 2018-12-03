using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : MonoBehaviour {

    //how much extra space from mesh should it spawn
    private const float BULLET_OVERHEAD = 0.25f;

    [Tooltip("Type of bullet we will use")]
    [SerializeField]private GameObject bullet;
    private Collider c;

    [Tooltip("Are you using a cooldown between shots?")]
    [SerializeField] private bool usingCooldown = true;

    [Tooltip("How many secounds between each shot")]
    [SerializeField] private float coolDown = .1f;

    private float shotTimer;
         
	// Use this for initialization
	void Start () {
        c = GetComponent<BoxCollider>();
        shotTimer = coolDown;
	}
	
	// Update is called once per frame
	void Update () {
        shotTimer += Time.deltaTime;
	}

    // method to shoot a bullet
    // returns true if sucessfully shot, false otherwise
    public bool Shoot () {
        if (!usingCooldown || shotTimer >= coolDown)
        {
            Vector3 barrelOffset = new Vector3(0, 0, c.transform.localScale.z / 2 + bullet.transform.localScale.z / 2 + BULLET_OVERHEAD);
            GameObject b = Instantiate(bullet, transform.position + transform.rotation * barrelOffset, transform.rotation);
            shotTimer = 0;
            return true;
        } 
        else
        {
            return false;
        }
    }
}
