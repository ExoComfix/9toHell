using UnityEngine;

public class Item_LeaveRequest : MonoBehaviour
{
    public float stressReliefAmount = 30f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.IncreaseBurnout(-stressReliefAmount);

                if (player.currentBurnout < 0f) player.currentBurnout = 0f;
            }

            Manager_UIManager uiManager = FindAnyObjectByType<Manager_UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowSynergyNotification("<color=#00FF00><b>📄 İZİN DİLEKÇESİ ONAYLANDI! <b></color>\nTükenmişlik seviyesi azaldı, derin bir nefes al!");
            }

            Destroy(gameObject);
        }
    }
}