using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    float lifeSpan;
    float maxScale;
    float timer = 0f;
    Vector3 direction = new Vector3(0, 0, 0);
    GameObject followObject = null;
    

    public void setLifespan(float newLifespan)
    {
        lifeSpan = newLifespan;
    }

    public void setMaxScale(float newMaxScale)
    {
        maxScale = newMaxScale;
    }

    public void setFollow(GameObject newFollow)
    {
        followObject = newFollow;
    }

    public void setDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(maxScale, maxScale, maxScale);
    }

    // while alive, expand until max scale is reached then die
    void Update()
    {
        timer += Time.deltaTime;

        transform.position = followObject.transform.position;
        transform.up = -direction;

        if (timer >= lifeSpan)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other == null) return;  
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(); 
        }

        BulletController bullet = other.GetComponent<BulletController>();
        if (bullet != null)
        {
            Destroy(bullet.gameObject);
        }
            
    }

}
