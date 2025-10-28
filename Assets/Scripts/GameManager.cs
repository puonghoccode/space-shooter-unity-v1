using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float minInstantiateValue;
    public float maxInstantiateValue;

    private void Start()
    {
        InvokeRepeating("InstantiateEnemy", 2f, 3f);
    }

    void InstantiateEnemy()
    {
        Vector3 enemypos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6f);
        Instantiate(enemyPrefab, enemypos, Quaternion.identity);
    }
}
