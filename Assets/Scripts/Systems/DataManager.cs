using UnityEngine;

public class DataManager : MonoBehaviour 
{
    public static DataManager Instance { get; private set; }

    private const string TOTAL_XP_KEY = "TotalSeniorityPoints";
    private const string UPGRADE_DAMAGE_KEY = "Upgrade_DamageLevel";
    private const string UPGRADE_SPEED_KEY = "Upgrade_SpeedLevel";
    private const string UPGRADE_HEALTH_KEY = "Upgrade_HealthLevel";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    public int GetTotalPoints() => PlayerPrefs.GetInt(TOTAL_XP_KEY, 0);

    public void AddPoints(int amount)
    {
        int current = GetTotalPoints();
        PlayerPrefs.SetInt(TOTAL_XP_KEY, current + amount);
        PlayerPrefs.Save();
    }
    public bool SpendPoints(int amount)
    {
        int current = GetTotalPoints();
        if (current >= amount)
        {
            PlayerPrefs.SetInt(TOTAL_XP_KEY, current - amount);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
    public int GetUpgradeLevel(string UpgradeType) 
    {
        return UpgradeType switch
        {
            "Damage" => PlayerPrefs.GetInt(UPGRADE_DAMAGE_KEY, 0),
            "Speed" => PlayerPrefs.GetInt(UPGRADE_SPEED_KEY, 0),
            "Health" => PlayerPrefs.GetInt(UPGRADE_HEALTH_KEY, 0),
            _ => 0,
        };
    }
    public void IncreaseUpgradeLevel(string upgradeType)
    {
        int currentLevel = GetUpgradeLevel(upgradeType);
        string key = upgradeType switch
        {
            "Damage" => UPGRADE_DAMAGE_KEY,
            "Speed" => UPGRADE_SPEED_KEY,
            "Health" => UPGRADE_HEALTH_KEY,
            _ => "",
        };
        if (!string.IsNullOrEmpty(key))
        {
            PlayerPrefs.SetInt(key, currentLevel + 1);
            PlayerPrefs.Save();
        }
    }
    // FOR TEST

    [ContextMenu("Reset All Data")]
    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("[DATA MANAGER] Tüm veriler sýfýrlandý.");
    }
}
