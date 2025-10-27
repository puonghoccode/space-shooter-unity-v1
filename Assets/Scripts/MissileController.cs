using UnityEngine;

public class MissileController : MonoBehaviour
{
    public float missileSpeed = 20f;
    void Update()
    {
        transform.Translate(Vector3.up * missileSpeed * Time.deltaTime);
    }
}
