using System;
using UnityEngine;
using static BulletLibrary;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    Vector2 bulletDirection = new Vector2(0f, 0f);
    PlayerController player;
    EnemyController controller;
    float angleOffset = 0f;
    float bulletSpeed = 50f;
    float fireInterval = 0.00001f; // time between bullets
    float fireTimer = 0f;
    bool active = true;

    Action FirePattern;

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
        angleOffset = 0f;
        bulletSpeed = newBulletSpeed;
        fireInterval = newFireInterval;

    }

    public void setPlayer(PlayerController newPlayer)
    {
        player = newPlayer;
    }

    public void setController(EnemyController newController)
    {
        controller = newController;
    }

    public void setActivation(bool newActive)
    {
        active = newActive;
    }

    public void SetPatternToFire(Action newPattern)
    {
        FirePattern = newPattern;
    }

    public void FireBackBlast()
    {
        if (!active) return;
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            bulletDirection += new Vector2(11f, 7f);

            // fire six "arms" of bullets more-or-less evenly distributed around itself, then increment their angles to make a spiral
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, bulletDirection, bulletSpeed, 10f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, -bulletDirection, bulletSpeed, 10f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, new Vector2(bulletDirection.x + 45, bulletDirection.y + 45), bulletSpeed, 10f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, new Vector2(bulletDirection.x - 45, bulletDirection.y - 45), bulletSpeed, 10f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, new Vector2(bulletDirection.x + 90, bulletDirection.y + 90), bulletSpeed, 10f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, new Vector2(bulletDirection.x - 90, bulletDirection.y - 90), bulletSpeed, 10f);
            
            fireTimer = 0f;
        }
    }

    public void FireLaser()
    {
        if (!active) return;
        fireInterval = 0.001f;
        bulletSpeed = 500f;
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval/10)
        {
            float radius = 3f;

            // also let's not account for acceleration i dont want to make a polynomial factorer in C#
            Vector3 playerVelocity = player.getVelocity();
            Vector3 playerPosition = player.getPosition();
            Vector3 toPlayer = (playerPosition - transform.position).normalized;

            // trollface predictive aim
            Vector3 toAim = BulletLibrary.PredictiveAim(transform.position, playerPosition, playerVelocity, bulletSpeed);
            Vector2 direction = CartesianToSpherical(toAim.normalized);
            
            // Plane formed by normal vector and origin
            Vector3 normal = SphericalToCartesian(direction.x, direction.y, 1f).normalized;
            Vector3 tangent1 = Vector3.Cross(normal, Vector3.left).normalized;
            Vector3 tangent2 = Vector3.Cross(normal, tangent1).normalized;
            
            // Four quadrants on the plane at a given radius
            Vector3 pointOnPlane1 = tangent1 * (radius * Mathf.Cos(angleOffset)) + tangent2 * (radius * Mathf.Sin(angleOffset));
            Vector3 pointOnPlane2 = -tangent1 * (radius * Mathf.Cos(angleOffset)) + tangent2 * (radius * Mathf.Sin(angleOffset));
            Vector3 pointOnPlane3 = tangent1 * (radius * Mathf.Cos(angleOffset)) - tangent2 * (radius * Mathf.Sin(angleOffset));
            Vector3 pointOnPlane4 = -tangent1 * (radius * Mathf.Cos(angleOffset)) - tangent2 * (radius * Mathf.Sin(angleOffset));

            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, direction, bulletSpeed, 20f);

            // outer ring
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position + pointOnPlane1, direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position + pointOnPlane2, direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position + pointOnPlane3, direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position + pointOnPlane4, direction, bulletSpeed, 20f);

            // inner ring
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position + (pointOnPlane1 * 0.5f), direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position + (pointOnPlane2 * 0.5f), direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position + (pointOnPlane3 * 0.5f), direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position + (pointOnPlane4 * 0.5f), direction, bulletSpeed, 20f);

            angleOffset += 7f;
            fireTimer = 0f;
        }
    }


    // Update is called once per frame
    void Update()
    {
        FirePattern();
    }
}
