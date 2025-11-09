using UnityEngine;

public class MissileController : MonoBehaviour
{
    public float missileSpeed;
    void Update()
    {
        transform.Translate(Vector3.up * missileSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (GameManager.instance != null && GameManager.instance.explosion != null)
            {
                var pos = other.transform.position;         // <<< quan trọng
                var vfx = Instantiate(GameManager.instance.explosion, pos, Quaternion.identity);

                Destroy(vfx, 2f);
            }
            GameManager.instance?.AddScore(5);

            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
