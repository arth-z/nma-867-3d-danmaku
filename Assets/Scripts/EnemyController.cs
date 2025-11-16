using Unity.Cinemachine;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    // movement
    float delta_speed;
    float NORMAL_SPEED = 500f;
    Vector3 velocity = new Vector3(0, 0, 0);
    Vector3 accel;
    float dashTimer = 0.3f;
    float dashCoeff = 1f;
    float behaviourTimer = 0f;
    bool aggressive = false;
    bool dashing = false;


    Rigidbody rb;
    Collider col;

    // gameplay systems
    int health = 50;
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

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        player = playerObject.GetComponent<PlayerController>();
    }

    void MotionControl()
    {
        transform.position += dashCoeff * Time.deltaTime * velocity + (0.5f * Time.deltaTime * Time.deltaTime * accel);
        // change velocity by acceleration
        velocity += accel * Time.deltaTime;
        // air resistance or something
        velocity /= 1 + drag * Time.deltaTime;
    }

    void ReactToPlayerPhase1()
    {
        behaviourTimer += Time.deltaTime;
        if (!aggressive)
        {
            Vector3 playerPosition = player.getPosition();
            Vector3 playerForward = player.getForward();
            Vector3 playerVelocity = player.getVelocity();
            Vector3 playerAccel = player.getAccel();
            float playerDashCoeff = player.getDashCoeff();

            Vector3 toPlayer = (playerPosition - transform.position).normalized;
            float distanceToPlayer = toPlayer.magnitude;

            if (behaviourTimer > 1.5f)
            {
                accel = -toPlayer * delta_speed;
            }

            if (player.getDashCoeff() > 1.5f)
            {
                accel = BulletLibrary.RandomOrthogonalVector(toPlayer).normalized * delta_speed * 1.5f;
            }

            if (behaviourTimer > 3f)
            {
                accel += BulletLibrary.RandomOrthogonalVector(toPlayer).normalized * delta_speed;
                behaviourTimer = 0f;
            }
            
        }
    }

    void Focus()
    {
        drag = AIR_RES*4f;
        delta_speed = NORMAL_SPEED * 0.7f;
    }

    void Unfocus()
    {
        drag = AIR_RES;
        delta_speed = NORMAL_SPEED;
    }

    void Dash()
    {
        if (dashTimer >= 0.3f) dashTimer = 0f;
        dashing = true;
    }

    void StopDash()
    {
        dashing = false;
    }

    void DashUpdate()
    {
        if (dashTimer < 0.3f)
        {
            dashTimer += Time.deltaTime;
            dashCoeff = dashCoeff >= 3f ? 3f : dashCoeff + dashTimer * 1500f * Time.deltaTime;
        }
        else
        {
            if (dashCoeff > 1.5f) dashCoeff -= Time.deltaTime;
            if (dashing) {dashCoeff = 1.5f;} else {dashCoeff = 1f;}
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

    // Update is called once per frame
    void Update()
    {
        ReactToPlayerPhase1();
        DashUpdate();
        MotionControl();
        IFrameUpdate();
    }

    // update is called after all Update functions have been called
    void LateUpdate()
    {

    }

    void FixedUpdate()
    {
    }

    void Attack()
    {
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
}