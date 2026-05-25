using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private const string PointsKey = "SeniorityPoints";
    private const string DamageLevelKey = "Upgrade_Damage";
    private const string SpeedLevelKey = "Upgrade_Speed";
    private const string HealthLevelKey = "Upgrade_Health";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public int GetTotalPoints()
    {
        return PlayerPrefs.GetInt(PointsKey, 0);
    }

    public void AddPoints(int amount)
    {
        if (amount <= 0) return;

        int total = GetTotalPoints() + amount;
        PlayerPrefs.SetInt(PointsKey, total);
        PlayerPrefs.Save();
    }

    public bool SpendPoints(int cost)
    {
        if (cost <= 0) return true;

        int total = GetTotalPoints();
        if (total < cost) return false;

        PlayerPrefs.SetInt(PointsKey, total - cost);
        PlayerPrefs.Save();
        return true;
    }

    public int GetUpgradeLevel(string upgradeType)
    {
        switch (upgradeType)
        {
            case "Damage":
                return PlayerPrefs.GetInt(DamageLevelKey, 0);
            case "Speed":
                return PlayerPrefs.GetInt(SpeedLevelKey, 0);
            case "Health":
                return PlayerPrefs.GetInt(HealthLevelKey, 0);
            default:
                return 0;
        }
    }

    public void IncreaseUpgradeLevel(string upgradeType)
    {
        int level = GetUpgradeLevel(upgradeType) + 1;

        switch (upgradeType)
        {
            case "Damage":
                PlayerPrefs.SetInt(DamageLevelKey, level);
                break;
            case "Speed":
                PlayerPrefs.SetInt(SpeedLevelKey, level);
                break;
            case "Health":
                PlayerPrefs.SetInt(HealthLevelKey, level);
                break;
            default:
                return;
        }

        PlayerPrefs.Save();
    }
}
