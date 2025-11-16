using UnityEngine;

public static class BulletLibrary
{
    public static BulletController SpawnBulletFixedSpeedAngle(GameObject bulletPrefab, Vector3 position, Vector2 thetaPhi, float speed, float lifespan, bool friendly = false)
    {
        GameObject bullet = Object.Instantiate(bulletPrefab, position, Quaternion.LookRotation(thetaPhi));
        BulletController bc = bullet.GetComponent<BulletController>();
        bc.setSphericalRotation(thetaPhi);
        bc.setSpeed(speed);
        bc.setLifespan(lifespan);
        if (lifespan != -1f) {
            bc.setExpiry(true);
        }
        bc.setDeltaSpeed(0f);
        bc.setSphericalRotationAccel(Vector2.zero);
        bc.updateVelocity();
        bc.updateAccel();
        return bc;
    }

    // careful with this one, accelDirection implies angular velocity
    public static BulletController SpawnBulletAccel(GameObject bulletPrefab, Vector3 position, Vector2 thetaPhi, float speed, Vector2 accelThetaPhi, float deltaSpeed, float lifespan, bool friendly = false)
    {
        GameObject bullet = Object.Instantiate(bulletPrefab, position, Quaternion.LookRotation(thetaPhi));
        BulletController bc = bullet.GetComponent<BulletController>();
        bc.setSphericalRotation(thetaPhi);
        bc.setSpeed(speed);
        bc.setSphericalRotationAccel(accelThetaPhi);
        bc.setDeltaSpeed(deltaSpeed);
        bc.setLifespan(lifespan);
        if (lifespan != -1f)
        {
            bc.setExpiry(true);
        }
        bc.updateVelocity();
        bc.updateAccel();
        bc.setFriendly(friendly);
        return bc;
    }

    public static Vector3 SphericalToCartesian(float thetaDeg, float phiDeg, float radius=1)
    {
        float thetaRad = Mathf.Deg2Rad * thetaDeg;
        float phiRad = Mathf.Deg2Rad * phiDeg;

        float x = radius * Mathf.Cos(phiRad) * Mathf.Cos(thetaRad);
        float y = radius * Mathf.Sin(phiRad);
        float z = radius * Mathf.Cos(phiRad) * Mathf.Sin(thetaRad);

        return new Vector3(x, y, z);
    }

    // credits: https://stackoverflow.com/questions/55464852/how-to-find-a-randomic-vector-orthogonal-to-a-given-vector
    public static Vector3 RandomOrthogonalVector(Vector3 v)
    {
        Vector3 normal = v.normalized;
        Vector3 tangent = Vector3.Cross(normal, new Vector3(-normal.z, normal.x, normal.y));
        Vector3 bitangent = Vector3.Cross(normal, tangent);
        float randAngle = Random.Range(-Mathf.PI, Mathf.PI);
        return tangent * Mathf.Cos(randAngle) + bitangent * Mathf.Sin(randAngle);

    }
}
