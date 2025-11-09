using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed;

    [Header("Missile")]
    public GameObject missile;
    public Transform missileSpawnPosition;
    public Transform muzzleSpawnPosition;
    public float destroyTime = 5f;

    [Header("Hit Feedback")]
    public float invulnDuration = 2f;      
    public float blinkHz = 10f;               // tần số
    public GameObject spinMissileFXOptional;
    bool _invuln;
    GameObject fx = null;

    void Update()
    {
        PlayerMovement();
        PlayerShoot();
    }

    void PlayerMovement()
    {
        float xPos = Input.GetAxis("Horizontal");
        float yPos = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(xPos, yPos, 0) * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    void PlayerShoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnMissile();
            SpawnMuzzleFlash();
        }
    }

    void SpawnMissile()
    {
        GameObject gm = Instantiate(missile, missileSpawnPosition.position, missileSpawnPosition.rotation);

        Destroy(gm, destroyTime);
    }
    
    void SpawnMuzzleFlash()
    {
        if (GameManager.instance && GameManager.instance.muzzleFlash)
        {
            var muzzle = Instantiate(GameManager.instance.muzzleFlash,
                                     muzzleSpawnPosition.position, muzzleSpawnPosition.rotation);
            Destroy(muzzle, 0.25f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (_invuln) return;

        if (other.CompareTag("Enemy") || other.CompareTag("Asteroid"))
        {
            StartCoroutine(HitEffectThenReduceLife());
        }
    }

    IEnumerator HitEffectThenReduceLife()
    {
        _invuln = true;

        GameManager.instance?.TakeHit();

        if (GameManager.instance != null && GameManager.instance.currentLives <= 0)
        {
            yield break;
        }

        if (spinMissileFXOptional)
        {
            fx = Instantiate(spinMissileFXOptional, transform.position, Quaternion.identity);
            fx.transform.SetParent(transform, false);

            var my = GetComponentInChildren<SpriteRenderer>();
            var ring = fx.GetComponentInChildren<SpriteRenderer>();
            if (my && ring) { ring.sortingLayerID = my.sortingLayerID; ring.sortingOrder = my.sortingOrder + 10; }
        }

        // bất tử tạm
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // nhấp nháy
        var srs = GetComponentsInChildren<SpriteRenderer>();
        float t = 0f;
        while (t < invulnDuration)
        {
            float phase = Mathf.PingPong(t * blinkHz, 1f);
            float alpha = Mathf.Lerp(0.3f, 1f, phase);
            foreach (var sr in srs)
            {
                var c = sr.color; c.a = alpha; sr.color = c;
            }

            t += Time.deltaTime;
            yield return null;
        }

        // khôi phục
        foreach (var sr in srs)
        { 
            var c = sr.color; c.a = 1f; sr.color = c; 
        }
        if (fx) Destroy(fx);
        if (col) col.enabled = true;

        _invuln = false;
    }
}