using UnityEngine;

public class BulletController : MonoBehaviour
{

    public Vector3 angles; // this is not a direction vector!!! these are direction cosine angles!!!
    public float speed;
    Vector3 velocity;
    public Vector3 accelAngles;
    public float delta_speed;
    Vector3 accel;

    public float lifespan = 20f;
    public bool willExpire = true;

    Collider col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateVelocity();
        updateAccel();
        col = GetComponent<Collider>();
    }

    void Move()
    {
        transform.forward = velocity.normalized; // face the direction you're going
        velocity += accel * Time.deltaTime; // go at your speed
        transform.position += velocity * Time.deltaTime; // accelerate if required
    }

    void Lifespan()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0f && willExpire)
        {
            Destroy(gameObject);
        }
    }

    void updateVelocity()
    {
        Vector3 radAngles = new Vector3(angles.x * Mathf.Deg2Rad, angles.y * Mathf.Deg2Rad, angles.z * Mathf.Deg2Rad);
        velocity = new Vector3(
            Mathf.Cos(radAngles.x) * speed,
            Mathf.Sin(radAngles.y) * speed,
            Mathf.Cos(radAngles.z) * speed
        );
    }

    void updateAccel()
    {
        Vector3 radAccelAngles = new Vector3(accelAngles.x * Mathf.Deg2Rad, accelAngles.y * Mathf.Deg2Rad, accelAngles.z * Mathf.Deg2Rad);
        accel = new Vector3(
            Mathf.Cos(radAccelAngles.x) * delta_speed,
            Mathf.Sin(radAccelAngles.y) * delta_speed,
            Mathf.Cos(radAccelAngles.z) * delta_speed
        );
    }

    // note: this accepts 3-tuple of ANGLES in degrees
    public void setAngles(Vector3 newAngles)
    {
        angles = newAngles;
        updateVelocity();
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
        updateVelocity();
    }

    public void setAngleAccel(Vector3 newDirection)
    {
        accelAngles = newDirection;
        updateAccel();
    }

    public void setDeltaSpeed(float newDeltaSpeed)
    {
        delta_speed = newDeltaSpeed;
        updateAccel();
    }

    public void setLifespan(float newLifespan)
    {
        lifespan = newLifespan;
    }

    public void setExpiry(bool doesExpire)
    {
        willExpire = doesExpire;
    }   

    // Update is called once per frame
    void Update()
    {
        Move();
        Lifespan();
    }
}
