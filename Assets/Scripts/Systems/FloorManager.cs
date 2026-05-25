using System;
using UnityEngine;
public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance { get; private set; }

    public enum FloorState { Exploration, CorporateCollapse, BossEncounter, FloorCleared }

    [Header("Current Floor Metrics")]
    public int currentFloor = -1;
    public FloorState currentFloorState = FloorState.Exploration;

    [Header("Collapse Settings")]
    public float collapseTimer = 0f;
    public float timeBeforeCollapse = 120f;
    [Range(0f, 100f)] public float collapsePercentage = 0f;

    [Header("Objective Progress")]
    public int totalObjectivesOnFloor = 1;
    private int completedObjectivesCount = 0;

    [Header("Boss Spawn (sole spawn configuration)")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    private bool bossSummonAuthorized;
    private bool bossSpawned;

    public static event Action<FloorState> OnFloorStateChanged;
    public static event Action<float> OnCollapseProgressUpdated;
    public static event Action OnBossSummonReady;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        if (currentFloor < 0) currentFloor = 0;
        EnterFloorExploration();
    }

    private void Update()
    {
        if (currentFloorState != FloorState.Exploration) return;

        collapseTimer += Time.deltaTime;
        collapsePercentage = Mathf.Clamp01(collapseTimer / timeBeforeCollapse) * 100f;
        OnCollapseProgressUpdated?.Invoke(collapsePercentage);

        if (collapseTimer >= timeBeforeCollapse)
        {
            EnterCorporateCollapse();
        }
    }

    public void OnObjectiveCompleted()
    {
        if (currentFloorState != FloorState.Exploration) return;

        completedObjectivesCount++;
        Debug.Log($"[CORP] {currentFloor}. Görev tamamlandı! ({completedObjectivesCount}/{totalObjectivesOnFloor})");

        if (completedObjectivesCount >= totalObjectivesOnFloor)
        {
            EnterBossEncounterFromObjectives();
        }
    }
    public void RequestBossSummon()
    {
        if (currentFloorState != FloorState.CorporateCollapse) return;
        EnterBossEncounter();
    }

    public void NotifyBossDefeated()
    {
        if (currentFloorState != FloorState.BossEncounter) return;
        EnterFloorCleared();
    }

    public void RequestContinueAfterVictory()
    {
        if (currentFloorState != FloorState.FloorCleared) return;
        AdvanceToNextFloor();
    }

    private void EnterFloorExploration()
    {
        currentFloorState = FloorState.Exploration;
        bossSummonAuthorized = false;
        bossSpawned = false;
        collapseTimer = 0f;
        collapsePercentage = 0f;
        OnFloorStateChanged?.Invoke(currentFloorState);
        Debug.Log($"[CORP] {currentFloor}. Kata giriş yapıldı. Görevler bekleniyor...");
    }

    private void EnterCorporateCollapse()
    {
        if (currentFloorState != FloorState.Exploration) return;

        currentFloorState = FloorState.CorporateCollapse;
        bossSummonAuthorized = true;
        OnFloorStateChanged?.Invoke(currentFloorState);
        OnBossSummonReady?.Invoke();
        Debug.LogWarning("[⚠️ ALERT] SÜRE BİTTİ! Ofis çöküş moduna geçiyor. Acil durum ışıkları devrede!");
    }
    private void EnterBossEncounterFromObjectives()
    {
        if (currentFloorState != FloorState.Exploration) return;

        bossSummonAuthorized = true;
        Debug.Log($"[CORP] {currentFloor}. Tüm görevler tamamlandı! Patronun ortaya çıkması için hazırlık yapılıyor...");
        EnterBossEncounter();
    }
    private void EnterBossEncounter()
    {
        if (!bossSummonAuthorized || bossSpawned) return;

        currentFloorState = FloorState.BossEncounter;
        OnFloorStateChanged?.Invoke(currentFloorState);
        SpawnBoss();
    }

    private void SpawnBoss()
    {
        if (bossSpawned) return;

        if (bossPrefab == null || bossSpawnPoint == null)
        {
            Debug.LogError("[CORP] FloorManager bossPrefab veya bossSpawnPoint atanmamış! Boss spawn edilemedi.");
            return;
        }

        GameObject bossInstance = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        bossInstance.name = "Boss_CEO";
        bossSpawned = true;
        Debug.LogError($"[CORP] {currentFloor}. Kat — CEO sahneye indi!");
    }

    private void EnterFloorCleared()
    {
        currentFloorState = FloorState.FloorCleared;
        OnFloorStateChanged?.Invoke(currentFloorState);
        Debug.Log($"[CORP] {currentFloor}. Patron yenildi! Kat temizlendi, bir sonraki kata geçmeye hazırlanın...");
    }

    private void AdvanceToNextFloor()
    {
        currentFloor++;
        completedObjectivesCount = 0;

        BaseObjective[] objectives = FindObjectsByType<BaseObjective>();
        if (objectives != null)
        {
            foreach (BaseObjective objective in objectives)
            {
                if (objective != null)
                {
                    objective.ResetForNewFloor();
                }
            }
        }

        EnterFloorExploration();
        Debug.Log($"[CORP] Asansör indi — {currentFloor}. kata hoş geldiniz.");
    }
}
