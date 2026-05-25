using UnityEngine;
using System.Collections;

public class Objective_ServerReboot : BaseObjective
{
    [Header("Server Reboot Settings")]
    public float requiredRebootTime = 15f;
    public float currentProgress = 0f;
    public float captureRadius = 3.5f;
    public LayerMask playerLayer;

    [Header("Visual Feedback (Optional)")]
    public SpriteRenderer zoneIndicatorSprite;
    public Color activeColor = Color.cyan;
    public Color completedColor = Color.green;

    private bool isPlayerInside = false;
    private Manager_UIManager uiManager;
    protected override void Start()
    {
        base.Start();
        objectiveName = "IT Sunucu Odası Operasyonu";
        objectiveDescription = "Arızalı ana sunucu kasasına yakın durarak sistemi yeniden başlat (Reboot). Alandan ayrılma!";

        uiManager = FindAnyObjectByType<Manager_UIManager>();
        if (uiManager != null)
        {
            uiManager.InitializeObjectiveUI(objectiveName, objectiveDescription, requiredRebootTime);
        }
        if (zoneIndicatorSprite != null)
        {
            zoneIndicatorSprite.color = Color.red;
            zoneIndicatorSprite.transform.localScale = new Vector3(captureRadius * 2, captureRadius * 2, 1f);
        }
    }

    void Update()
    {
        if (IsCompleted) return;
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, captureRadius, playerLayer);
        isPlayerInside = playerCollider != null;

        if (isPlayerInside)
        {
            if (zoneIndicatorSprite != null && zoneIndicatorSprite.color != activeColor)
                zoneIndicatorSprite.color = activeColor;

            currentProgress += Time.deltaTime;
            float progressPercentage = (currentProgress / requiredRebootTime) * 100f;

            if (currentProgress >= requiredRebootTime)
            {
                CompleteObjective();
            }
        }
        else
        {
            if (zoneIndicatorSprite != null && zoneIndicatorSprite.color != Color.red)
                zoneIndicatorSprite.color = Color.red;

            if (currentProgress > 0f)
            {
                currentProgress -= Time.deltaTime * 0.5f;
            }
        }
        if (uiManager != null)
        {
            uiManager.UpdateObjectiveProgress(currentProgress, isPlayerInside);
        }
    }
    protected override void OnObjectiveCompleteVisuals()
    {
        base.OnObjectiveCompleteVisuals();
        if (uiManager != null)
        {
            uiManager.ClearObjectiveUI("✓ BAŞARILI: Sunucu ayağa kaldırıldı. Plaza interneti aktif!");
        }
    }

    public override void ResetForNewFloor()
    {
        base.ResetForNewFloor();
        currentProgress = 0f;
        isPlayerInside = false;

        if (uiManager != null)
        {
            uiManager.InitializeObjectiveUI(objectiveName, objectiveDescription, requiredRebootTime);
        }

        if (zoneIndicatorSprite != null)
        {
            zoneIndicatorSprite.color = Color.red;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, captureRadius);
    }
}