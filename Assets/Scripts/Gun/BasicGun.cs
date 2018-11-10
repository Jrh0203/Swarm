using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : MonoBehaviour {

    //how much extra space from mesh should it spawn
    private const float BULLET_OVERHEAD = 0.25f;

    [SerializeField]private Bullet bullet;
    private Collider c;

    [SerializeField] private const float bulletCooldown = 1;
    private float bulletCooldownTimer;
         
	// Use this for initialization
	void Start () {
        c = GetComponent<BoxCollider>();
        bulletCooldownTimer = bulletCooldown;
	}
	
	// Update is called once per frame
	void Update () {
        bulletCooldownTimer += Time.deltaTime;

	    if (Input.GetAxis("Fire1") != 0 && bulletCooldownTimer >= bulletCooldown)
        {
            Debug.Log("SHOT BOI");
            Instantiate(bullet, transform.position + new Vector3(0, 0,c.transform.localScale.z / 2 + bullet.transform.localScale.z / 2 + BULLET_OVERHEAD), transform.rotation);
            bulletCooldownTimer = 0;
        }
	}
}
