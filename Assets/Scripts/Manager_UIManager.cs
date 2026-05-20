using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Manager_UIManager : MonoBehaviour
{
    [Header("Core UI Elements")]
    public Slider burnoutSlider;
    public Slider xpSlider;
    public TextMeshProUGUI levelText;

    [Header("Upgrade System UI")]
    public GameObject upgradePanel;
    public Button card1Button;
    public Button card2Button;
    public Button card3Button;

    [Header("Upgrade Pool (Geliştirme Havuzu)")]
    public List<UpgradeCard> allUpgradesPool = new List<UpgradeCard>();

    private List<UpgradeCard> currentSelectedCards = new List<UpgradeCard>();
    private PlayerController player;
    private int lastCheckedLevel = 1;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.GetComponent<PlayerController>();

        if (upgradePanel != null) upgradePanel.SetActive(false);

        card1Button.onClick.AddListener(() => OnCardClicked(0));
        card2Button.onClick.AddListener(() => OnCardClicked(1));
        card3Button.onClick.AddListener(() => OnCardClicked(2));
    }

    void Update()
    {
        if (player == null) return;

        burnoutSlider.value = player.currentBurnout;
        xpSlider.maxValue = player.xpToNextLevel;
        xpSlider.value = player.currentXP;

        string title = "Stajyer";
        if (player.currentLevel >= 2 && player.currentLevel < 4) title = "Junior Uzman";
        else if (player.currentLevel >= 4 && player.currentLevel < 6) title = "Senior Uzman";
        else if (player.currentLevel >= 6) title = "Müdür Yardımcısı";

        levelText.text = $"Unvan: {title} (Lv. {player.currentLevel})";

        if (player.currentLevel > lastCheckedLevel)
        {
            lastCheckedLevel = player.currentLevel;
            ShowUpgradeScreen();
        }
    }

    void ShowUpgradeScreen()
    {
        if (allUpgradesPool.Count < 3)
        {
            Debug.LogError("Havuzda en az 3 kart olmalı dostum!");
            return;
        }

        List<UpgradeCard> tempList = new List<UpgradeCard>(allUpgradesPool);
        currentSelectedCards.Clear();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            currentSelectedCards.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }
        SetButtonTexts(card1Button, currentSelectedCards[0]);
        SetButtonTexts(card2Button, currentSelectedCards[1]);
        SetButtonTexts(card3Button, currentSelectedCards[2]);

        upgradePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void SetButtonTexts(Button button, UpgradeCard cardData)
    {
        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = $"<b>{cardData.cardTitle}</b>\n\n{cardData.cardDescription}";
        }
    }

    void OnCardClicked(int cardIndex)
    {
        if (player == null || cardIndex >= currentSelectedCards.Count) return;

        string chosenID = currentSelectedCards[cardIndex].upgradeID;
        bool upgradeApplied = false;

        if (chosenID == "AttackSpeed") { player.UpgradeAttackSpeed(); upgradeApplied = true; }
        else if (chosenID == "MoveSpeed") { player.UpgradeMoveSpeed(); upgradeApplied = true; }
        else if (chosenID == "MaxHealth") { player.UpgradeMaxHealth(); upgradeApplied = true; }
        else if (chosenID == "Damage") { player.UpgradeDamage(); upgradeApplied = true; }
        else if (chosenID == "Range") { player.UpgradeRange(); upgradeApplied = true; }

        if (!upgradeApplied)
        {
            Debug.LogError($"[CRITICAL] '{chosenID}' adında bir Upgrade ID kod tarafında bulunamadı! Oyuncunun kilitlenmemesi için teselli ödülü olarak stres sıfırlandı.");
            player.UpgradeMaxHealth();
        }

        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}