using System;
using UnityEngine;
using static BulletLibrary;

public class BulletSpawner : MonoBehaviour
{
    public GameObject[] bulletPrefabs;
    Vector2 bulletDirection = new Vector2(0f, 0f);
    PlayerController player;
    EnemyController controller;
    float angleOffset = 0f;
    float fireTimer = 0f;
    int intensity = 0;
    bool active = true;

    Action FirePattern;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bulletPrefabs == null || bulletPrefabs.Length == 0) Debug.LogError("BulletSpawner: No bullet prefabs assigned");
        foreach (var prefab in bulletPrefabs)
        {
            if (prefab.scene.IsValid()) Debug.LogError("BulletSpawner: Bullet prefab is in scene, not project assets");
        }
    }

    public void setIntensity(int newIntensity)
    {
        intensity = newIntensity;
    }

    public int getIntensity()
    {
        return intensity;
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
        float bulletSpeed = 50f;
        float fireInterval = 0.00001f; 
        float bulletLifespan = 5f;
        if (!active) return;
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            bulletDirection += new Vector2(11f, 7f);

            // fire "arms" of bullets more-or-less evenly distributed around itself, then increment their angles to make a spiral
            // well, i have a really bad intuition for 3d space so this is vaguely symmetrical and evenly distributed
            // i think i see the classic 2D danmaku spiral here somewhere so good enough for me
            if (intensity > 0)
            {
                SpawnBulletFixedSpeedAngle(bulletPrefabs[0], transform.position, bulletDirection, bulletSpeed, 10f);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[0], transform.position, -bulletDirection, bulletSpeed, 10f);

                SpawnBulletFixedSpeedAngle(bulletPrefabs[0], transform.position, new Vector2(bulletDirection.x + 45, bulletDirection.y + 45), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[0], transform.position, new Vector2(bulletDirection.x - 45, bulletDirection.y - 45), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[0], transform.position, new Vector2(bulletDirection.x + 90, bulletDirection.y + 90), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[0], transform.position, new Vector2(bulletDirection.x - 90, bulletDirection.y - 90), bulletSpeed, bulletLifespan);   
            }

            if (intensity > 1)
            {
                SpawnBulletFixedSpeedAngle(bulletPrefabs[0], transform.position, new Vector2(bulletDirection.x + 45, -bulletDirection.y + 45), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[0], transform.position, new Vector2(bulletDirection.x - 45, -bulletDirection.y - 45), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[0], transform.position, new Vector2(bulletDirection.x + 90, -bulletDirection.y + 90), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[0], transform.position, new Vector2(bulletDirection.x - 90, -bulletDirection.y - 90), bulletSpeed, bulletLifespan);
            }

            if (intensity > 2)
            {
                SpawnBulletFixedSpeedAngle(bulletPrefabs[1], transform.position, new Vector2(-bulletDirection.x + 45, bulletDirection.y + 45), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[1], transform.position, new Vector2(-bulletDirection.x - 45, bulletDirection.y - 45), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[1], transform.position, new Vector2(-bulletDirection.x + 90, bulletDirection.y + 90), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[1], transform.position, new Vector2(-bulletDirection.x - 90, bulletDirection.y - 90), bulletSpeed, bulletLifespan);
            }

            if (intensity > 3)
            {
                SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position, new Vector2(-bulletDirection.x + 45, -bulletDirection.y + 45), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position, new Vector2(-bulletDirection.x - 45, -bulletDirection.y - 45), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position, new Vector2(-bulletDirection.x + 90, -bulletDirection.y + 90), bulletSpeed, bulletLifespan);
                SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position, new Vector2(-bulletDirection.x - 90, -bulletDirection.y - 90), bulletSpeed, bulletLifespan);
            }


            fireTimer = 0f;
        }
    }

    public void FireAimedStream()
    {
        if (intensity == 0) return;
        float fireInterval = 0.0001f;
        float bulletSpeed = 500f * (1 + (intensity / 6f));
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
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

            SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position, direction, bulletSpeed, 20f);

            // outer ring
            SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position + pointOnPlane1, direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position + pointOnPlane2, direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position + pointOnPlane3, direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position + pointOnPlane4, direction, bulletSpeed, 20f);

            // inner ring
            SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position + (pointOnPlane1 * 0.5f), direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position + (pointOnPlane2 * 0.5f), direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position + (pointOnPlane3 * 0.5f), direction, bulletSpeed, 20f);
            SpawnBulletFixedSpeedAngle(bulletPrefabs[2], transform.position + (pointOnPlane4 * 0.5f), direction, bulletSpeed, 20f);

            angleOffset += 7f;
            fireTimer = 0f;
        }
    }

    public Action getFirePattern()
    {
        return FirePattern;
    }

    // Update is called once per frame
    void Update()
    {
        FirePattern();
    }
}
