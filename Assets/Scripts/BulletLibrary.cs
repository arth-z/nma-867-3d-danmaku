using UnityEngine;

public static class BulletLibrary
{
    public static BulletController SpawnBulletFixedSpeed(GameObject bulletPrefab, Vector3 position, Vector3 rotation, float speed, float lifespan = 20f)
    {
        GameObject bullet = Object.Instantiate(bulletPrefab, position, Quaternion.Euler(rotation));
        BulletController bc = bullet.GetComponent<BulletController>();
        bc.setAngles(rotation);
        bc.setSpeed(speed);
        bc.setLifespan(lifespan);
        return bc;
    }

    // careful with this one, accelDirection implies angular velocity
    public static BulletController SpawnBulletAccel(GameObject bulletPrefab, Vector3 position, Vector3 rotation, float speed, Vector3 accelDirection, float deltaSpeed, float lifespan = 20f)
    {
        GameObject bullet = Object.Instantiate(bulletPrefab, position, Quaternion.Euler(rotation));
        BulletController bc = bullet.GetComponent<BulletController>();
        bc.setAngles(rotation);
        bc.setSpeed(speed);
        bc.setAngleAccel(accelDirection);
        bc.setDeltaSpeed(deltaSpeed);
        bc.setLifespan(lifespan);
        return bc;
    }
}
