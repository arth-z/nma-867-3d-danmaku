using Unity.Cinemachine;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    // movement
    float delta_speed;
    float NORMAL_SPEED = 450f;
    Vector3 velocity = new Vector3(0, 0, 0);
    Vector3 accel;
    float defenseiveMotionTimer = 0f;
    float boundsMotionTimer = 0f;
    float phaseTimer = 0f;
    bool aggressive = false;

    public BulletSpawner bulletSpawner;

    // gameplay systems
    int health = 5;
    float iFrameTimer = 0f;

    float drag;
    float AIR_RES = 5f;

    // you!
    public GameObject playerObject;
    PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        delta_speed = NORMAL_SPEED;
        drag = AIR_RES;

        player = playerObject.GetComponent<PlayerController>();

        bulletSpawner.setPlayer(player);
        bulletSpawner.setController(this);
        bulletSpawner.SetPatternToFire(bulletSpawner.FireBackBlast);
    }

    void MotionControl()
    {
        transform.position +=  Time.deltaTime * velocity + (0.5f * Time.deltaTime * Time.deltaTime * accel);
        // change velocity by acceleration
        velocity += accel * Time.deltaTime;
        // air resistance or something
        velocity /= 1 + drag * Time.deltaTime;
    }

    void Phase1()
    {
        Vector3 toPlayer = player.getPosition() - transform.position;

        phaseTimer += Time.deltaTime;

        if (phaseTimer > 9f && toPlayer.magnitude > 200f)
        {
            aggressive = true;
            phaseTimer += Time.deltaTime * Random.Range(0.0f, 4f);
        } else if (phaseTimer > 15f)
        {
            aggressive = false;
            phaseTimer = 0f;
        } 
        
        if (aggressive)
        {
            Offensive();
            SlowDown();
        } else
        {
            Defensive();   
            MotionControl();
        }
    }

    void SlowDown()
    {
        accel = new Vector3(0f, 0f, 0f);
    }

    void Defensive()
    {
        defenseiveMotionTimer += Time.deltaTime;
        // fire the backblast pattern if you're not doing it
        if (bulletSpawner.getFirePattern() != bulletSpawner.FireBackBlast)
        {
            bulletSpawner.SetPatternToFire(bulletSpawner.FireBackBlast);
        }
        Vector3 playerPosition = player.getPosition();

        Vector3 toPlayer = (playerPosition - transform.position).normalized;
        float distanceToPlayer = toPlayer.magnitude;

        // basic idea - at all times, fly away from the player, but occasionally broadside/strafe out orthogonally to them
        // read their dashcoeff and immediately react by dashing orthogonally if they dash towards you
        if (defenseiveMotionTimer > 1.5f && distanceToPlayer < 300f || accel == Vector3.zero)
        {
            accel = -toPlayer * delta_speed;
        } else if (defenseiveMotionTimer > 1.5f)
        {
            accel = BulletLibrary.RandomOrthogonalVector(toPlayer).normalized * delta_speed;
        }

        if (player.getDashCoeff() > 1.5f && distanceToPlayer < 300f)
        {
            accel = BulletLibrary.RandomOrthogonalVector(toPlayer).normalized * delta_speed * 1.5f;
        }

        if (defenseiveMotionTimer > 3f)
        {
            accel += BulletLibrary.RandomOrthogonalVector(toPlayer).normalized * delta_speed;
            defenseiveMotionTimer = 0f;
        }
            
    }

    void Offensive()
    {
        // just shoot that shit straight at them
        if (bulletSpawner.getFirePattern() != bulletSpawner.FireAimedStream)
        {
            bulletSpawner.SetPatternToFire(bulletSpawner.FireAimedStream);
        }
    }

    void IFrameUpdate()
    {
        if (iFrameTimer > 0f)
        {
            iFrameTimer -= Time.deltaTime;
            if (iFrameTimer < 0f) iFrameTimer = 0f;
        }
    }

    public void TakeDamage()
    {
        print("yowch");
        health -= 1;
        bulletSpawner.setIntensity(5-health);
        aggressive = false;
        phaseTimer = 0;
        if (health <= 0) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Phase1();
        MotionControl();
        IFrameUpdate();
        DontHitTheGround();
        DontEscapeBounds();
    }

    // update is called after all Update functions have been called
    void LateUpdate()
    {

    }

    void FixedUpdate()
    {
    }

    public Vector3 getVelocity()
    {
        return velocity;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (iFrameTimer == 0f)
        {
            BulletController bulletOther = other.GetComponent<BulletController>();
            if (bulletOther == null) return;
            if (!bulletOther.isFriendly()) return;
            health -= 1;
            print(health);
            if (health <= 0) Destroy(gameObject);
            iFrameTimer = 2f;
        }
    }

    public void DontHitTheGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward.normalized, out hit, delta_speed * Time.deltaTime + 0.2f))
        {
            if (hit.collider.GetComponent<Terrain>() != null)
            {
                accel = BulletLibrary.RandomOrthogonalVector(transform.forward).normalized * delta_speed;

            }
        }
    }

    public void DontEscapeBounds()
    {
        boundsMotionTimer += Time.deltaTime;
        if (boundsMotionTimer < 1.5f) return;

        Vector3 toOrigin = (Vector3.zero - transform.position).normalized;
        // for now, a 3200x3200x3200 cube centered at origin
        if (transform.position.x > 1600f || transform.position.x < -1600f ||
            transform.position.y > 1600f || transform.position.y < -1600f ||
            transform.position.z > 1600f || transform.position.z < -1600f)
        {
            accel = toOrigin * delta_speed;
        }
    }
}