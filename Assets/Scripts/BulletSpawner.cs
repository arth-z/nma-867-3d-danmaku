using UnityEngine;
using static BulletLibrary;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Vector3 bulletDirection = new Vector3(0f, 0f, 0f);
    public float bulletSpeed = 1f;
    public Vector3 bulletAccelDirection = new Vector3(0f, 0f, 0f);
    public float bulletDeltaSpeed = 0f;
    public float fireInterval = 0.1f; // time between bullets
    public float fireTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BulletController bc = bulletPrefab.GetComponent<BulletController>();
        }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            SpawnBulletFixedSpeed(bulletPrefab, transform.position, bulletDirection, bulletSpeed, 20f);
            SpawnBulletFixedSpeed(bulletPrefab, transform.position, bulletDirection + new Vector3(90, 0, 0), bulletSpeed, 20f);
            SpawnBulletFixedSpeed(bulletPrefab, transform.position, -bulletDirection, bulletSpeed, 20f);
            SpawnBulletFixedSpeed(bulletPrefab, transform.position, -bulletDirection + new Vector3(90, 0, 0), bulletSpeed, 20f);

            bulletDirection += new Vector3(0f, 10f, 10f); // rotate bullet direction for next shot
            fireTimer = 0f;
        }
    }
}
