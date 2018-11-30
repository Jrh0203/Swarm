using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : MonoBehaviour {

    //how much extra space from mesh should it spawn
    private const float BULLET_OVERHEAD = 0.25f;

    [Tooltip("Type of bullet we will use")]
    [SerializeField]private Bullet bullet;
    private Collider c;

    [Tooltip("Are you using a cooldown between shots?")]
    [SerializeField] private bool usingCooldown = true;

    [Tooltip("How many secounds between each shot")]
    [SerializeField] private float coolDown = .1f;

    enum GunType {BasicGun, Shotgun};
    [SerializeField] GunType startingGun = GunType.BasicGun;
    private GunType gunType;
    private float shotTimer;


    // Shotgun Fields
    int shotgunBulletCount;
    //Bullet spread in degrees from center line
    float shotgunBulletSpread;
         
	// Use this for initialization
	void Start () {
        c = GetComponent<BoxCollider>();
        shotTimer = coolDown;
        gunType = startingGun;
        shotgunBulletCount = 8;
        shotgunBulletSpread = 45.0f;
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
            switch (gunType) {
                case GunType.Shotgun:
                    ShootShotgun();
                    break;
                case GunType.BasicGun:
                default:
                    ShootBasicGun();
                    break;
            }
            shotTimer = 0;
            return true;
        } 
        else
        {
            return false;
        }
    }

    public void ShootBasicGun() {
        Vector3 barrelOffset = new Vector3(0, 0, c.transform.localScale.z / 2 + bullet.transform.localScale.z / 2 + BULLET_OVERHEAD);
        Bullet b = Instantiate(bullet, transform.position + transform.rotation * barrelOffset, transform.rotation);
    }

    public void ShootShotgun() {
        Debug.Log("Shooting Shotgun");
        Vector3 barrelOffset = new Vector3(0, 0, c.transform.localScale.z / 2 + bullet.transform.localScale.z / 2 + BULLET_OVERHEAD);

        float degreesBetweenBullets = (shotgunBulletSpread / (shotgunBulletCount - 1)) * 2;
        if (shotgunBulletCount == 1) degreesBetweenBullets = 0.0f;
        for (int i=0; i < shotgunBulletCount; i++) {
            float bulletDegrees = -shotgunBulletSpread + i * degreesBetweenBullets;
            if (bulletDegrees <= 0) bulletDegrees += 360.0f; 
            Quaternion direction = transform.rotation * Quaternion.AngleAxis(bulletDegrees, Vector3.up);
            Bullet b = Instantiate(bullet, transform.position + direction * barrelOffset * 2, direction);
            Debug.Log("Bullet" + i + " rotation = " + b.transform.forward);
        }
    }
}
