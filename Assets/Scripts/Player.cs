using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public float playerSpeed;
	public Vector3 playerVelocity;
	private CharacterController controller;

	private float smoothSpeed = .4f;
    //gun stuff
    private BasicGun gun;
	private float movementMultiplier = 1.0f;
    private bool shot = false;
	public float hp = 100;
	public float maxHp = 100;

	private float range = 18;
	private bool hit;
	public float Range {
		get {
			return range;
		}

		set {
			range = value;
		}
	}

	public float Hp {
		get {
			return hp;
		}

		set {
			hp = value;
		}
	}

	// Use this for initialization
	void Start () {
		hit = false;
		controller = GetComponent<CharacterController>();
		playerVelocity = new Vector3(0, 0, 0);
        gun = GetComponentInChildren<BasicGun>();
	}
	
	// Update is called once per frame
	void Update () {
		hit = false;
		Vector3 moveDir = GetMoveDirection();
		Vector3 moveVelocity = moveDir * Time.deltaTime * playerSpeed * movementMultiplier;
		
		//set velocity in x and z plane
		playerVelocity.x = moveVelocity.x;
		playerVelocity.z = moveVelocity.z;

		//set velocity in the y direction
		playerVelocity += Physics.gravity * Time.deltaTime;
		if(controller.isGrounded) {
			playerVelocity.y = 0;
		}

		controller.Move(playerVelocity);
		Vector3 look = GetLookDirection();
		if(look != Vector3.zero) {
			Vector3 smoothedLook = Vector3.Lerp(transform.forward, look, smoothSpeed);
			transform.forward = smoothedLook;
		}
        // shoot bullet
        if (!shot && Input.GetAxis("Fire1") != 0)
        {
            shot = true;
            gun.Shoot();
        }
        else { shot = false; }
	}

	Vector3 GetMoveDirection() {
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		moveInput = Vector3.ClampMagnitude(moveInput, 1);
		return moveInput;
	}

	Vector3 GetLookDirection() {
		Vector3 newDir = new Vector3(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2, 0);
		return Vector3.Normalize(new Vector3(newDir.x, 0, newDir.y));
	}

	public void hurt(float damage) {
		if(!hit) {
			hit = true;
			hp = Mathf.Clamp(hp - damage, 0, hp);
		}
	}

	public void SetMovementModifier(float modifier) {
		movementMultiplier = modifier;
	}

	void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.layer == LayerMask.NameToLayer("Bullet")) {
			Bullet b = collider.gameObject.GetComponent<Bullet>();
			hurt(b.getDamage());
			b.Seppuku();	
		}
    }
}
