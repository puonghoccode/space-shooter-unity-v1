using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float speed;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance?.OnAsteroidHitPlayer();
            Destroy(gameObject);
        }
    }
}
