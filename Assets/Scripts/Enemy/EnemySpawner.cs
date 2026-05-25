using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private bool floorAllowsSpawning = true;
    private bool uiPauseActive;
    [Header("Base Spawn Settings")]
    public float baseSpawnRate = 2.0f;
    public float minSpawnRate = 0.4f;
    public float difficultyScalePeriod = 30f;

    [Header("Spawn Area")]
    public float minSpawnDistance = 8f;
    public float maxSpawnDistance = 12f;

    private float currentSpawnRate;
    private float nextSpawnTime;
    private float startTime;
    private Transform playerTransform;

    private GameObject regularPrefab;
    private GameObject internPrefab;
    private GameObject hrManagerPrefab;
    private GameObject micromanagerPrefab;

    private void OnEnable()
    {
        FloorManager.OnFloorStateChanged += HandleFloorStateChanged;
        ApplySpawnState();
    }

    private void OnDisable()
    {
        FloorManager.OnFloorStateChanged -= HandleFloorStateChanged;
    }

    private void Start()
    {
        startTime = Time.time;
        currentSpawnRate = baseSpawnRate;
        playerTransform = EnemyPlayerTarget.Transform;

        LoadEnemyPrefabs();
        ApplySpawnState();
    }

    private void HandleFloorStateChanged(FloorManager.FloorState newState)
    {
        floorAllowsSpawning = newState == FloorManager.FloorState.Exploration
            || newState == FloorManager.FloorState.CorporateCollapse;
        ApplySpawnState();
    }

    public void SetUIPause(bool paused)
    {
        uiPauseActive = paused;
        ApplySpawnState();
    }

    private void ApplySpawnState()
    {
        enabled = floorAllowsSpawning && !uiPauseActive;
    }

    private void Update()
    {
        if (!enabled || playerTransform == null) return;

        CalculateDynamicDifficulty();

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + currentSpawnRate;
        }
    }

    private void LoadEnemyPrefabs()
    {
        regularPrefab = Resources.Load<GameObject>("Enemy");
        internPrefab = Resources.Load<GameObject>("Prefabs/Enemy_Intern");
        hrManagerPrefab = Resources.Load<GameObject>("Prefabs/Enemy_HR");
        micromanagerPrefab = Resources.Load<GameObject>("Prefabs/Enemy_MicroManager");

        if (regularPrefab == null) Debug.LogWarning("[SPAWNER] 'Enemy' prefab bulunamadı!");
    }

    private void CalculateDynamicDifficulty()
    {
        float timeElapsed = Time.time - startTime;
        float difficultyLevel = Mathf.Floor(timeElapsed / difficultyScalePeriod);

        currentSpawnRate = baseSpawnRate - (difficultyLevel * 0.2f);
        currentSpawnRate = Mathf.Max(currentSpawnRate, minSpawnRate);
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomSpawnPositionAroundPlayer();
        GameObject prefabToSpawn = SelectWeightedPrefab();

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GetRandomSpawnPositionAroundPlayer()
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 offset = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * randomDistance;
        return playerTransform.position + offset;
    }

    private GameObject SelectWeightedPrefab()
    {
        float timeElapsed = Time.time - startTime;
        float regularWeight = 100f;
        float internWeight = timeElapsed < 120f ? 80f : 30f;
        float hrWeight = timeElapsed > 60f ? 25f : 5f;
        float microWeight = timeElapsed > 30f ? 35f : 0f;

        float totalWeight = regularWeight + internWeight + hrWeight + microWeight;
        float roll = Random.Range(0f, totalWeight);

        if (roll < regularWeight)
        {
            return regularPrefab;
        }

        if (roll < regularWeight + internWeight)
        {
            return internPrefab != null ? internPrefab : regularPrefab;
        }

        if (roll < regularWeight + internWeight + hrWeight)
        {
            return hrManagerPrefab != null ? hrManagerPrefab : regularPrefab;
        }

        return micromanagerPrefab != null ? micromanagerPrefab : regularPrefab;
    }
}
