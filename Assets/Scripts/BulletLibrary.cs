using System;
using UnityEngine;

public static class BulletLibrary
{
    // angles here should always be passed in as degrees!
    public static BulletController SpawnBulletFixedSpeedAngle(GameObject bulletPrefab, Vector3 position, Vector2 thetaPhi, float speed, float lifespan, bool friendly = false)
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, position, Quaternion.LookRotation(thetaPhi));
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

    public static BulletController SpawnBulletFixedSpeedDirection(GameObject bulletPrefab, Vector3 position, Vector3 direction, float speed, float lifespan, bool friendly = false)
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, position, Quaternion.LookRotation(direction));
        BulletController bc = bullet.GetComponent<BulletController>();
        bc.setSphericalRotation(CartesianToSpherical(direction));
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
        GameObject bullet = GameObject.Instantiate(bulletPrefab, position, Quaternion.LookRotation(thetaPhi));
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

    // converts spherical coordinates (theta, phi, radius) in degrees to cartesian (x, y, z)
    public static Vector3 SphericalToCartesian(Vector3 spherical)
    {
        float theta = spherical.x * Mathf.Deg2Rad;
        float phi = spherical.y * Mathf.Deg2Rad;
        return new Vector3(
            Mathf.Cos(phi) * Mathf.Cos(theta),
            Mathf.Sin(theta),
            Mathf.Sin(phi) * Mathf.Cos(theta));
    }

    public static Vector3 SphericalToCartesian(float thetaDeg, float phiDeg, float radius=1f)
    {
        float theta = thetaDeg * Mathf.Deg2Rad;
        float phi = phiDeg * Mathf.Deg2Rad;
        return new Vector3(
            Mathf.Cos(phi) * Mathf.Cos(theta),
            Mathf.Sin(theta),
            Mathf.Sin(phi) * Mathf.Cos(theta));
    }

    // converts cartesian coordinates (x, y, z) to spherical direction (theta, phi)
    public static Vector2 CartesianToSpherical(Vector3 cartesian)
    {
        cartesian.Normalize();
        float theta = Mathf.Asin(cartesian.y) * Mathf.Rad2Deg;
        float phi = Mathf.Atan2(cartesian.z, cartesian.x) * Mathf.Rad2Deg;
        return new Vector2(theta, phi);
    }

    // credits: https://stackoverflow.com/questions/55464852/how-to-find-a-randomic-vector-orthogonal-to-a-given-vector
    public static Vector3 RandomOrthogonalVector(Vector3 v)
    {
        Vector3 normal = v.normalized;
        Vector3 tangent = Vector3.Cross(normal, new Vector3(-normal.z, normal.x, normal.y));
        Vector3 bitangent = Vector3.Cross(normal, tangent);
        float randAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
        return tangent * Mathf.Cos(randAngle) + bitangent * Mathf.Sin(randAngle);

    }

    // credits: https://web.archive.org/web/20210808051027/http://ringofblades.com/Blades/Code/PredictiveAim.cs 
    public static Vector3 PredictiveAim(Vector3 bulletInitialPosition, Vector3 targetInitialPosition, Vector3 targetVelocity, float bulletSpeed)
    {
        // the magnitude of the vector from shooter to target's final position is equal to the speed of the bullet times time until impact
        // so we can isolate for time using the equation: (bulletInitialPosition + targetVelocity * time - targetInitialPosition).magnitude = bulletSpeed * time
        // pretty complicated equation but you can figure out the length of the left side through the law of cosines with:
        // A = (targetInitialPosition - bulletInitialPosition).magnitude
        // B = targetVelocity.magnitude * time
        // C = bulletSpeed * time
        // use dot product to solve for the relevant angle:

        float cosTheta = targetVelocity.magnitude > 0 ? Vector3.Dot((targetInitialPosition - bulletInitialPosition).normalized, targetVelocity.normalized) : 1.0f;

        // awesome. applying the law of cosines and rearranging, i'll save you the algebra, we get a quadratic equation solving for t where:
        float a = (bulletSpeed * bulletSpeed) - (targetVelocity.sqrMagnitude);
        float b = 2f * (targetInitialPosition - bulletInitialPosition).magnitude * targetVelocity.magnitude * cosTheta;
        float c = -1f * (targetInitialPosition - bulletInitialPosition).sqrMagnitude;

        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f || Mathf.Abs(a) < 0.001f)
        {
            // no way for your bullets to actually hit the target (how the hell did we get here)
            // just aim right at them and hope for the best
            return (targetInitialPosition - bulletInitialPosition).normalized * bulletSpeed;
        }
        float timeToImpact1 = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
        float timeToImpact2 = (-b - Mathf.Sqrt(discriminant)) / (2f * a);
        float timeToImpact = Mathf.Min(timeToImpact1, timeToImpact2);
        if (timeToImpact < 0f) timeToImpact = Mathf.Max(timeToImpact1, timeToImpact2);
        if (timeToImpact < 0f) timeToImpact = 0f;

        return targetVelocity + (targetInitialPosition - bulletInitialPosition) / timeToImpact; // the vector to aim at, its magnitude being the speed
    }
}
