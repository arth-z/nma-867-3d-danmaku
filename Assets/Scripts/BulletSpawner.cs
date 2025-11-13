using UnityEngine;
using static BulletLibrary;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    Vector2 bulletDirection = new Vector2(0f, 0f);
    float bulletSpeed = 25f;
    Vector2 bulletAccelDirection = new Vector2(0f, 0f);
    float bulletDeltaSpeed = 0f;
    float fireInterval = 0.00011f; // time between bullets
    float fireTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bulletPrefab == null) Debug.LogError("BulletSpawner: No bullet prefab assigned");
        if (bulletPrefab.scene.IsValid()) Debug.LogError("BulletSpawner: Bullet prefab is in scene, not project assets");
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            bulletDirection += new Vector2(11f, 7f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, bulletDirection, bulletSpeed, 5f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, -bulletDirection, bulletSpeed, 5f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, new Vector2(bulletDirection.x + 90, bulletDirection.y + 90), bulletSpeed, 5f);
            SpawnBulletFixedSpeedAngle(bulletPrefab, transform.position, new Vector2(bulletDirection.x - 90, bulletDirection.y - 90), bulletSpeed, 5f);
            
            fireTimer = 0f;
        }
    }
}
