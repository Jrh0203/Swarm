using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShoot : MonoBehaviour {

    LinkedList<Vector3> prevVelcities;
    private const int PREV_VEL_NUM = 10;

    //how much extra space from mesh should it spawn
    private const float BULLET_OVERHEAD = 0.25f;

    [Tooltip("Type of bullet we will use")]
    [SerializeField]private Bullet bullet;
    private Collider c;

    [Tooltip("Are you using a cooldown between shots?")]
    [SerializeField] private bool usingCooldown = true;

    [Tooltip("How many secounds between each shot")]
    [SerializeField] private float coolDown = .1f;

    public bool doShoot = true;
    private float shotTimer;

    AudioSource gunSound;
         
	// Use this for initialization
	void Start () {
        //c = GetComponent<BoxCollider>();
        prevVelcities = new LinkedList<Vector3>();
        shotTimer = coolDown;
        for(int i = 0; i < PREV_VEL_NUM; i ++)
        prevVelcities.AddLast(Vector3.zero);

        gunSound = GetComponent<AudioSource>();
    }

    public static Vector3 FirstOrderIntercept
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
    )  {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return targetPosition + t*(targetRelativeVelocity);
    }

    public static float FirstOrderInterceptTime
    (
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity
    ) {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if(velocitySquared < 0.001f)
            return 0f;
     
        float a = velocitySquared - shotSpeed*shotSpeed;
     
        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude/
            (
                2f*Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }
     
        float b = 2f*Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b*b - 4f*a*c;
     
        if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
            float   t1 = (-b + Mathf.Sqrt(determinant))/(2f*a),
                    t2 = (-b - Mathf.Sqrt(determinant))/(2f*a);
            if (t1 > 0f) {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            } else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        } else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b/(2f*a), 0f); //don't shoot back in time
    }
	
	// Update is called once per frame
	void Update () {
        Player player = GameManager.Instance.PlayerObj;

        Vector3 fromPosition = transform.position;
        Vector3 toPosition = player.transform.position;
        Vector3 direction = toPosition - fromPosition;

        // get average velocity based on previos activity
        prevVelcities.RemoveFirst();
        prevVelcities.AddLast(GameManager.Instance.PlayerObj.playerVelocity);
        Vector3 total = Vector3.zero;
        foreach (Vector3 v in prevVelcities)
        {
            total += v;
        }
        total /= prevVelcities.Count;

        Vector3 interceptPoint = FirstOrderIntercept
        (
            transform.position,
            new Vector3(0,0,0),
            20,
            GameManager.Instance.PlayerObj.transform.position,
            //GameManager.Instance.PlayerObj.playerVelocity * 50
            total * 50
        );

        transform.LookAt(interceptPoint);
        transform.eulerAngles = new Vector3(
            0,
            transform.eulerAngles.y,
            0
        );

        Grid grid = GameManager.Instance.GridObj;
        Node n = grid.NodeFromWorldPos(fromPosition);
        shotTimer += Time.deltaTime;
        if (doShoot && shotTimer>coolDown && n.isCover == false){
        	Shoot();
        }
	}

    // method to shoot a bullet
    // returns true if sucessfully shot, false otherwise
    public bool Shoot () {
        if (!usingCooldown || shotTimer >= coolDown)
        {
            Vector3 barrelOffset = new Vector3(0, 0, transform.localScale.z / 2 + bullet.transform.localScale.z / 2 + BULLET_OVERHEAD);
            Bullet b = Instantiate(bullet, transform.position + transform.rotation * barrelOffset, transform.rotation);
            shotTimer = 0;
            gunSound.Play();
            return true;
        } 
        else
        {
            return false;
        }
    }
}
