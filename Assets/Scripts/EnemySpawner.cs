using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnRate = 2.0f;
    private float nextSpawnTime = 0f;

    [Header("Spawn Area")]
    public float minSpawnDistance = 8f;
    public float maxSpawnDistance = 12f;
    
    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    void Update()
    {
        if (playerTransform == null) return;
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnRate;
        }
    }
    void SpawnEnemy()
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

        float spawnX = playerTransform.position.x + Mathf.Cos(randomAngle) * randomDistance;
        float spawnY = playerTransform.position.y + Mathf.Sin(randomAngle) * randomDistance;
        
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
