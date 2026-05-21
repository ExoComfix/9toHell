using UnityEngine;

public class Item_Magnet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectAllXPOnScreen();
            Destroy(gameObject);
        }
    }

    void CollectAllXPOnScreen()
    {
        XPPickup[] allXP = FindObjectsByType<XPPickup>();

        foreach (XPPickup xp in allXP)
        {
            xp.TriggerMagnet();
        }
        Manager_UIManager uiManager = FindAnyObjectByType<Manager_UIManager>();

        if (uiManager != null)
        {
            uiManager.ShowSynergyNotification(
                "<color=#00FFCC><b>🧲 GLOBAL MIKNATIS AKTİF!</b></color>\nTüm kurumsal hedefler toplanıyor!"
            );
        }
    }
}