using UnityEngine;

public class StarController : MonoBehaviour
{
    public float speed;
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance?.OnStarCollected();
            Destroy(gameObject);
        }
    }
}
