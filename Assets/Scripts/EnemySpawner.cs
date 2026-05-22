using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Base Spawn Settings")]
    public float baseSpawnRate = 2.0f;
    public float minSpawnRate = 0.4f;
    public float difficultyScalePeriod = 30f;

    private float currentSpawnRate;
    private float nextSpawnTime = 0f;

    [Header("Spawn Area")]
    public float minSpawnDistance = 8f;
    public float maxSpawnDistance = 12f;

    private Transform playerTransform;
    private float startTime;

    private GameObject regularPrefab;
    private GameObject internPrefab;
    private GameObject hrManagerPrefab;
    private GameObject micromanagerPrefab;

    void Start()
    {
        startTime = Time.time;
        currentSpawnRate = baseSpawnRate;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        regularPrefab = Resources.Load<GameObject>("Enemy");
        internPrefab = Resources.Load<GameObject>("Enemy_Intern");
        hrManagerPrefab = Resources.Load<GameObject>("Enemy_HR");
        micromanagerPrefab = Resources.Load<GameObject>("Enemy_Micromanager");
        if (regularPrefab == null) Debug.LogWarning("[SPAWNER] 'Enemy' prefab bulunamadı!");
    }

    void Update()
    {
        if (playerTransform == null) return;
        CalculateDynamicDifficulty();
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + currentSpawnRate;
        }
    }
    void CalculateDynamicDifficulty()
    {
        float timeElapsed = Time.time - startTime;
        float difficultyLevel = Mathf.Floor(timeElapsed / difficultyScalePeriod);

        currentSpawnRate = baseSpawnRate - (difficultyLevel * 0.2f);
        currentSpawnRate = Mathf.Max(currentSpawnRate, minSpawnRate);
    }
    void SpawnEnemy()
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 spawnPosition = playerTransform.position + new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0) * randomDistance;

        float timeElapsed = Time.time - startTime;
        float regularWeight = 100f;
        float internWeight = timeElapsed < 120f ? 80f : 30f;
        float hrWeight = timeElapsed > 60f ? 25f : 5f;

        float microWeight = timeElapsed > 30f ? 35f : 0f;

        float totalWeight = regularWeight + internWeight + hrWeight + microWeight;
        float roll = Random.Range(0f, totalWeight);

        GameObject prefabToSpawn = regularPrefab;
        if (roll < regularWeight)
        {
            prefabToSpawn = regularPrefab;
        }
        else if (roll < regularWeight + internWeight)
        {
            if (internPrefab != null) prefabToSpawn = internPrefab;
        }
        else if (roll < regularWeight + internWeight + hrWeight)
        {
            if (hrManagerPrefab != null) prefabToSpawn = hrManagerPrefab;
        }
        else
        {
            if (micromanagerPrefab != null) prefabToSpawn = micromanagerPrefab;
        }
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}