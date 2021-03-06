﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : MonoBehaviour {

    //how much extra space from mesh should it spawn
    private const float BULLET_OVERHEAD = 0.25f;

    [Tooltip("Type of bullet we will use")]
    [SerializeField]private GameObject bullet;
    private Collider c;

    private LineRenderer lRend;

    [Tooltip("Are you using a cooldown between shots?")]
    [SerializeField] private bool usingCooldown = true;

    [Tooltip("How many secounds between each shot")]
    [SerializeField] private float initialCoolDown = .8f;
    public float coolDown;

    public enum GunType {BasicGun, Shotgun, Sniper};
    [SerializeField] GunType startingGun = GunType.BasicGun;
    private GunType gunType;
    public float shotTimer;


    // Shotgun Fields
    [Range(2, 20)]
    [SerializeField] int shotgunBulletCount = 4;
    //Bullet spread in degrees from center line
    [Range(0.0f, 180.0f)]
    [SerializeField] float shotgunBulletSpread = 45.0f;

    // Audio
    AudioSource shotgunSound;
    AudioSource sniperSound;
    AudioSource basicGunSound;
         
	// Use this for initialization
	void Start () {
        c = GetComponent<BoxCollider>();

        lRend = GetComponent<LineRenderer>();
        lRend.material = new Material (Shader.Find("Particles/Additive"));

        shotTimer = 0;
        coolDown = initialCoolDown;
        setGunType(startingGun);
        setGunType(GunType.BasicGun);

        sniperSound = GetComponents<AudioSource>()[0];
        shotgunSound = GetComponents<AudioSource>()[1];
        basicGunSound = GetComponents<AudioSource>()[2];
	}
	
	// Update is called once per frame
	void Update () {
        Color shotColor = lRend.startColor;
        shotColor.a -= .03f;
        lRend.startColor = shotColor;
        lRend.endColor = shotColor;
        shotTimer += Time.deltaTime;


        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            setGunType(GunType.BasicGun);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            setGunType(GunType.Shotgun);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            setGunType(GunType.Sniper);
        }

        // Fading out sniper shots
        
	}

    // method to shoot a bullet
    // returns true if sucessfully shot, false otherwise
    public bool Shoot () {
        if (shotTimer >= coolDown)
        {
            shotTimer = 0.0f;
            float addTo = .0f;
            if (gunType == GunType.Sniper){
                shotTimer+=.5f;
            }
            switch (gunType) {
                case GunType.Shotgun:
                    ShootShotgun();
                    break;
                case GunType.Sniper:
                    ShootSniper();
                    break;
                case GunType.BasicGun:
                default:
                    ShootBasicGun();
                    break;
            }
            return true;
        } 
        else
        {
            return false;
        }
    }

    public void ShootBasicGun() {
        Vector3 barrelOffset = new Vector3(0, 0, c.transform.localScale.z / 2 + bullet.transform.localScale.z / 2 + BULLET_OVERHEAD);
        GameObject b = Instantiate(bullet, transform.position + transform.rotation * barrelOffset*2, transform.rotation);
        basicGunSound.Play();
    }

    public void ShootShotgun() {
        float shotgunBulletLifetime = 0.5f;
        Vector3 barrelOffset = new Vector3(0, 0, c.transform.localScale.z / 2 + bullet.transform.localScale.z / 2 + BULLET_OVERHEAD);

        float degreesBetweenBullets = (shotgunBulletSpread / (shotgunBulletCount - 1)) * 2;
        if (shotgunBulletCount == 1) degreesBetweenBullets = 0.0f;

        for (int i=0; i < shotgunBulletCount; i++) {
            float bulletDegrees = -shotgunBulletSpread + i * degreesBetweenBullets;
            if (bulletDegrees <= 0) bulletDegrees += 360.0f; 
            Quaternion direction = transform.rotation * Quaternion.AngleAxis(bulletDegrees, Vector3.up);
            GameObject b = Instantiate(bullet, transform.position + direction * barrelOffset * 2.0f, direction);
            b.GetComponent<Bullet>().SetLifeTime(shotgunBulletLifetime);
        }
        shotgunSound.Play();
    }

    public void ShootSniper() {
        float sniperDamage = 70.0f;
        
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Enemy") | LayerMask.GetMask("Wall");
        Vector3 forwardVector = transform.TransformDirection(Vector3.forward);
        forwardVector.y = 0.0f;
        Vector3 posVector = transform.position;
        posVector.y = .8f;
        Debug.Log(transform.position);
        if (Physics.Raycast(posVector, forwardVector, out hit, Mathf.Infinity, mask)) {
            if (hit.collider.gameObject.tag == "Enemy") {
                Enemy e = hit.collider.gameObject.GetComponent<Enemy>();
                e.TakeDamage(sniperDamage);
            }
        }
        float rayDistance = (hit.distance == 0) ? 1000 : hit.distance;
        lRend.SetPosition(0, transform.position);
        lRend.SetPosition(1, transform.position + transform.TransformDirection(Vector3.forward) * rayDistance);
        Color lineColor = Color.white;
        lineColor.a = 255.0f;
        lRend.startColor = lineColor;
        lRend.endColor = lineColor;
        sniperSound.Play();
    }

    public void setGunType(GunType gun) {
        Camera cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        cam.SetSniperView(false);
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();

        gunType = gun;
        switch (gunType) {
            case GunType.Shotgun:
                coolDown = initialCoolDown * 5;
                player.SetMovementModifier(1.0f);
                break;
            case GunType.Sniper:
                coolDown = initialCoolDown * 8;
                cam.SetSniperView(true);
                player.SetMovementModifier(0.25f);
                break;
            case GunType.BasicGun:
                player.SetMovementModifier(1.1f);
                coolDown = initialCoolDown;
                break;
            default:
                player.SetMovementModifier(1.0f);
                coolDown = initialCoolDown;
                break;
        }
    }
}
