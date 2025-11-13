using UnityEngine;

public class BulletController : MonoBehaviour
{

    Vector2 anglesThetaPhi;
    float speed;
    Vector3 velocity;

    Vector2 accelAnglesThetaPhi;
    float delta_speed;
    Vector3 accel;

    float lifespan;
    bool willExpire;

    Collider col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.forward = velocity.normalized; // face the direction you're going
        col = GetComponent<Collider>();
    }

    void Move()
    {
        transform.forward = velocity.normalized; // face the direction you're going
        transform.position += velocity * Time.deltaTime + accel * (0.5f * Time.deltaTime * Time.deltaTime); // go in that direction according to that one kinematics eqn
        velocity += accel * Time.deltaTime; // update velocity
    }

    void Lifespan()
    {
        lifespan -= Time.deltaTime;
        if (lifespan < 0f && willExpire)
        {
            Destroy(gameObject);
        }
    }

    public void updateVelocity()
    {
        // Only X and Y angles are used for direction; Z is ignored
        var radAngles = Mathf.Deg2Rad * anglesThetaPhi;
        // Convert spherical angles to Cartesian coordinates
        velocity = new Vector3(
            Mathf.Cos(radAngles.y) * Mathf.Cos(radAngles.x),
            Mathf.Sin(radAngles.x),
            Mathf.Sin(radAngles.y) * Mathf.Cos(radAngles.x)
        ) * speed;
    }

    public void updateAccel()
    {
        // Only X and Y angles are used for direction; Z is ignored
        var radAngles = Mathf.Deg2Rad * accelAnglesThetaPhi;
        // Convert spherical angles to Cartesian coordinates
        accel = new Vector3(
            Mathf.Cos(radAngles.y) * Mathf.Cos(radAngles.x),
            Mathf.Sin(radAngles.x),
            Mathf.Sin(radAngles.y) * Mathf.Cos(radAngles.x)
        ) * delta_speed;
    }

    // note: this accepts 2-tuples of angles (spherical coords: theta, phi)
    public void setSphericalRotation(Vector2 newThetaPhi)
    {
        anglesThetaPhi = newThetaPhi;
    }

    public void setDirection(Vector3 newDirection)
    {
        velocity = newDirection.normalized * speed;
    }

    public void setSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void setSphericalRotationAccel(Vector2 newThetaPhi)
    {
        accelAnglesThetaPhi = newThetaPhi;
    }

    public void setDeltaSpeed(float newDeltaSpeed)
    {
        delta_speed = newDeltaSpeed;
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
