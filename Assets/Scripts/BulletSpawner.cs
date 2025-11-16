using System;
using UnityEngine;
using static BulletLibrary;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    Vector2 bulletDirection = new Vector2(0f, 0f);
    float bulletSpeed = 50f;
    Vector2 bulletAccelDirection = new Vector2(0f, 0f);
    float bulletDeltaSpeed = 0f;
    float fireInterval = 0.000001f; // time between bullets
    float fireTimer = 0f;
    bool active = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bulletPrefab == null) Debug.LogError("BulletSpawner: No bullet prefab assigned");
        if (bulletPrefab.scene.IsValid()) Debug.LogError("BulletSpawner: Bullet prefab is in scene, not project assets");


    }

    void InitializeWith(GameObject newBulletPrefab, Vector2 newBulletDirection, float newBulletSpeed, float newFireInterval)
    {
        bulletPrefab = newBulletPrefab;
        bulletDirection = newBulletDirection;
        bulletSpeed = newBulletSpeed;
        fireInterval = newFireInterval;
    }

    public void setActivation(bool newActive)
    {
        active = newActive;
    }

    void FirePattern1()
    {
        if (!active) return;
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            bulletDirection += new Vector2(11f, 7f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, bulletDirection, bulletSpeed, 10f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, -bulletDirection, bulletSpeed, 10f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, new Vector2(bulletDirection.x + 45, bulletDirection.y + 45), bulletSpeed, 10f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, new Vector2(bulletDirection.x - 45, bulletDirection.y - 45), bulletSpeed, 10f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, new Vector2(bulletDirection.x + 90, bulletDirection.y + 90), bulletSpeed, 10f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, new Vector2(bulletDirection.x - 90, bulletDirection.y - 90), bulletSpeed, 10f);
            
            fireTimer = 0f;
        }
    }


    // Update is called once per frame
    void Update()
    {
        FirePattern1();
    }
}
