using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MainMenuUpgradeUI: MonoBehaviour
{
    [Header ("UI Text Elements")]
    public TextMeshProUGUI totalPointsText;
    public TextMeshProUGUI damageLevelText;
    public TextMeshProUGUI speedLevelText;
    public TextMeshProUGUI healthLevelText;

    [Header("Cost Settings")]
    public int baseUpgradeCost = 50;
void Start()
    {
        UpdateUIElements();
    }
    void UpdateUIElements()
    {
        if(DataManager.Instance == null) return;

        totalPointsText.text = $"Kıdem Puanı: {DataManager.Instance.GetTotalPoints()}KP";

        int dmg = DataManager.Instance.GetUpgradeLevel("Damage");
        int spd = DataManager.Instance.GetUpgradeLevel("Speed");
        int hp = DataManager.Instance.GetUpgradeLevel("Health");

        damageLevelText.text = $"Seviye: {dmg} (Maliyet: {(dmg + 1) * baseUpgradeCost} KP)";
        speedLevelText.text = $"Seviye: {spd} (Maliyet: {(spd + 1) * baseUpgradeCost} KP)";
        healthLevelText.text = $"Seviye: {hp} (Maliyet: {(hp + 1) * baseUpgradeCost} KP)";
    }
    public void UpgradeDamage() => AttemptUpgrade("Damage");
    public void UpgradeSpeed() => AttemptUpgrade("Speed");
    public void UpgradeHealth() => AttemptUpgrade("Health");

    void AttemptUpgrade(string upgradeType)
    {
        if (DataManager.Instance == null) return;
        int currentLevel = DataManager.Instance.GetUpgradeLevel(upgradeType);
        int cost = (currentLevel + 1) * baseUpgradeCost;
        if (DataManager.Instance.SpendPoints(cost))
        {
            DataManager.Instance.IncreaseUpgradeLevel(upgradeType);
            UpdateUIElements();
        }
        else
        {
            Debug.LogWarning($"Yetersiz Kıdem Puanı! {upgradeType} yükseltmesi için {cost} KP gerekiyor.");
        }
    }
}
