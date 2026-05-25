using UnityEngine;
public class BossSummonPad : MonoBehaviour
{
    [Header("Interact Settings")]
    public float activationRadius = 2.5f;
    public LayerMask playerLayer;

    [Header("Visual Feedback")]
    public SpriteRenderer padSprite;
    public Color lockedColor = Color.gray;
    public Color readyColor = Color.yellow;
    public Color activeColor = Color.red;

    private bool padInteractEnabled;
    private Manager_UIManager uiManager;

    private void OnEnable()
    {
        FloorManager.OnBossSummonReady += EnablePadForCollapse;
        FloorManager.OnFloorStateChanged += HandleFloorStateChanged;
    }

    private void OnDisable()
    {
        FloorManager.OnBossSummonReady -= EnablePadForCollapse;
        FloorManager.OnFloorStateChanged -= HandleFloorStateChanged;
    }

    private void Start()
    {
        uiManager = FindAnyObjectByType<Manager_UIManager>();
        SetPadColor(lockedColor);
    }

    private void EnablePadForCollapse()
    {
        if (FloorManager.Instance != null &&
            FloorManager.Instance.currentFloorState != FloorManager.FloorState.CorporateCollapse)
        {
            return;
        }

        padInteractEnabled = true;
        SetPadColor(readyColor);
        Debug.LogWarning("[ASANSÖR] Süre doldu — Boss çağırma paneli aktif! Asansör başına gidin ve [E] basın.");
    }

    private void HandleFloorStateChanged(FloorManager.FloorState state)
    {
        switch (state)
        {
            case FloorManager.FloorState.Exploration:
                padInteractEnabled = false;
                SetPadColor(lockedColor);
                break;
            case FloorManager.FloorState.BossEncounter:
                padInteractEnabled = false;
                SetPadColor(activeColor);
                break;
            case FloorManager.FloorState.FloorCleared:
                padInteractEnabled = false;
                SetPadColor(lockedColor);
                break;
        }
    }

    private void Update()
    {
        if (!padInteractEnabled) return;

        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, activationRadius, playerLayer);
        if (playerCollider == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            RequestBossViaFloorManager();
        }
    }

    private void RequestBossViaFloorManager()
    {
        padInteractEnabled = false;
        SetPadColor(activeColor);

        if (FloorManager.Instance != null)
        {
            FloorManager.Instance.RequestBossSummon();
        }

        if (uiManager != null)
        {
            uiManager.ShowSynergyNotification(
                "<color=red><size=130%><b>🚨 EXECUTIVE ALERT 🚨</b></size>\nÜST DÜZEY YÖNETİCİ SAHNEYE İNDİ! KAÇIŞ YOK!</color>");
        }
    }

    private void SetPadColor(Color color)
    {
        if (padSprite != null) padSprite.color = color;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}
