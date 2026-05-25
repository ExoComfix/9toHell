using UnityEngine;

public abstract class BaseObjective : MonoBehaviour
{
    [Header("Objective Corporate Info")]
    public string objectiveName = "Genel Görev";
    [TextArea] public string objectiveDescription = "Bu katın kurumsal işlerini tamamla.";

    public bool IsCompleted { get; protected set; } = false;

    protected virtual void Start()
    {
        IsCompleted = false;
    }
    protected void CompleteObjective()
    {
        if (IsCompleted) return;

        IsCompleted = true;
        Debug.Log($"[SUCCESS] Görev Başarılı: {objectiveName}");

        if (FloorManager.Instance != null)
        {
            FloorManager.Instance.OnObjectiveCompleted();
        }

        OnObjectiveCompleteVisuals();
    }
    protected virtual void OnObjectiveCompleteVisuals()
    {
        // İleride görsel feedback için ezilebilir (override)
    }

    public virtual void ResetForNewFloor()
    {
        IsCompleted = false;
    }
}