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

    [Header("Character Selection UI")]
    public GameObject characterSelectionPanel;
    public Button selectDevButton;
    public Button selectMarketingButton;
    public Button selectAccountingButton;

    [Header("Upgrade System UI")]
    public GameObject upgradePanel;
    public Button card1Button;
    public Button card2Button;
    public Button card3Button;


    [Header("Game Over UI Elements")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI surviveTimeText;
    public TextMeshProUGUI finalLevelText;
    public Button restartButton;

    [Header("Synergy Notification")]
    public TMPro.TextMeshProUGUI synergyNotificationText;

    [Header("Upgrade Pool")]
    public List<UpgradeCard> allUpgradesPool = new List<UpgradeCard>();
    private List<UpgradeCard> currentSelectedCards = new List<UpgradeCard>();
    private PlayerController player;

    private int upgradeQueueCount = 0;

    void Start()
    {
        PlayerController playerObj = FindAnyObjectByType<PlayerController>();
        if (playerObj != null) player = playerObj;

        if (upgradePanel != null) upgradePanel.SetActive(false);

        card1Button.onClick.AddListener(() => OnCardClicked(0));
        card2Button.onClick.AddListener(() => OnCardClicked(1));
        card3Button.onClick.AddListener(() => OnCardClicked(2));

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (restartButton != null) restartButton.onClick.AddListener(() => RestartGame()); if (selectDevButton != null) selectDevButton.onClick.AddListener(() => SelectDepartment(0));
        if (selectMarketingButton != null) selectMarketingButton.onClick.AddListener(() => SelectDepartment(1));
        if (selectAccountingButton != null) selectAccountingButton.onClick.AddListener(() => SelectDepartment(2));

        if (characterSelectionPanel != null)
        {
            characterSelectionPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    void SelectDepartment(int deptIndex)
    {
        if (player == null) return;
        player.selectedDepartment = (PlayerController.DepartmentType)deptIndex;
        player.ApplyDepartmentStats();
        if (burnoutSlider != null)
        {
            burnoutSlider.maxValue = player.maxBurnout;
            burnoutSlider.value = 0f;
        }
        if (characterSelectionPanel != null)
        {
            characterSelectionPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        Debug.Log($"[SYSTEM] Oyun {player.selectedDepartment} olarak başladı!");
    }
    private void OnEnable()
    {
        PlayerController.OnBurnoutChanged += UpdateBurnoutUI;
        PlayerController.OnXPChanged += UpdateXPUI;
        PlayerController.OnLevelChanged += UpdateLevelUI;
    }
    private void OnDisable()
    {
        PlayerController.OnBurnoutChanged -= UpdateBurnoutUI;
        PlayerController.OnXPChanged -= UpdateXPUI;
        PlayerController.OnLevelChanged -= UpdateLevelUI;
    }
    private void UpdateBurnoutUI(float currentBurnout, float maxBurnout)
    {
        if (burnoutSlider != null)
        {
            burnoutSlider.maxValue = maxBurnout;
            burnoutSlider.value = currentBurnout;
        }
    }
    private void UpdateXPUI(float currentXP, float xpToNextLevel)
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = xpToNextLevel;
            xpSlider.value = currentXP;
        }
    }
    private void UpdateLevelUI(int currentLevel)
    {
        if (levelText != null)
        {
            levelText.text = $"Unvan: Developer (Lv. {currentLevel})";
            upgradeQueueCount++;
            if (!upgradePanel.activeSelf)
            {
                ShowUpgradeScreen();
            }
        }
    }

    public void TriggerGameOverScreen()
    {
        if (gameOverPanel == null) return;

        Time.timeScale = 0f;

        float totalSeconds = Time.timeSinceLevelLoad;
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);

        if (surviveTimeText != null)
        {
            surviveTimeText.text = $"Toplam Mesai Süresi: <color=#FFCC00>{minutes:00}:{seconds:00}</color>";
        }

        if (finalLevelText != null)
        {
            string finalTitle = levelText.text.Split('(')[0];
            finalLevelText.text = $"{finalTitle}\n(<color=#00FFCC>Seviye {player.currentLevel}</color> konumunda istifa edildi)";
        }

        gameOverPanel.SetActive(true);
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void ShowUpgradeScreen()
    {
        if (allUpgradesPool.Count < 3)
        {
            Debug.LogError("[UPGRADE] Havuzda en az 3 adet UpgradeCard scriptable/veri nesnesi olmalı kral!");
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
            tmpText.text = $"{cardData.cardTitle}\n\n<size=80%>{cardData.cardDescription}</size>";
        }
    }

    void OnCardClicked(int cardIndex)
    {
        if (player == null || cardIndex >= currentSelectedCards.Count) return;

        string chosenID = currentSelectedCards[cardIndex].upgradeID;

        if (!player.chosenUpgradeIDs.Contains(chosenID))
        {
            player.chosenUpgradeIDs.Add(chosenID);
        }

        bool upgradeApplied = false;
        if (chosenID == "AttackSpeed") { player.UpgradeAttackSpeed(); upgradeApplied = true; }
        else if (chosenID == "MoveSpeed") { player.UpgradeMoveSpeed(); upgradeApplied = true; }
        else if (chosenID == "MaxHealth") { player.UpgradeMaxHealth(); upgradeApplied = true; }
        else if (chosenID == "Damage") { player.UpgradeDamage(); upgradeApplied = true; }
        else if (chosenID == "Range") { player.UpgradeRange(); upgradeApplied = true; }

        if (!upgradeApplied)
        {
            Debug.LogError($"[CRITICAL] '{chosenID}' adında bir ID eşleşmedi! Teselli ödülü olarak stres sıfırlandı.");
            player.UpgradeMaxHealth();
        }

        if (player.chosenUpgradeIDs.Contains("AttackSpeed") && player.chosenUpgradeIDs.Contains("Damage"))
        {
            player.ActivateOvertimeSynergy();
            ShowSynergyNotification("<color=#FFCC00><b>SİNERJİ AKTİF: MESAİ PATLAMASI</b></color>\nKahve bardakları artık alan hasarı veriyor!");
        }

        if (player.chosenUpgradeIDs.Contains("Range") && player.chosenUpgradeIDs.Contains("Damage"))
        {
            player.ActivateDeadlineSynergy();
            ShowSynergyNotification("<color=#FF3333><b>🚨 SİNERJİ: DEADLINE PANELİ</b></color>\nAtışlar çarptığı yerde düşmanları %50 yavaşlatan kırmızı teslim bölgeleri oluşturuyor!");
        }

        if (player.chosenUpgradeIDs.Contains("MoveSpeed") && player.chosenUpgradeIDs.Contains("MaxHealth"))
        {
            player.ActivateHomeOfficeSynergy();
            ShowSynergyNotification("<color=#33FF33><b>🏡 SİNERJİ: HOME OFFICE LÜKSÜ</b></color>\nKahve bardakları artık stajyerleri ve müdürleri kurumsal bir güçle geriye fırlatıyor!");
        }

        if (player.chosenUpgradeIDs.Contains("AttackSpeed") && player.chosenUpgradeIDs.Contains("Range"))
        {
            player.ActivateBrainstormSynergy();
            ShowSynergyNotification("<color=#3399FF><b>💡 SİNERJİ: BRAINSTORMING</b></color>\nArtık tek bir bardak değil, plaza diliyle harmanlanmış çoklu kahve dalgası fırlatıyorsun!");
        }
        upgradeQueueCount--;
        if (upgradeQueueCount > 0) ShowUpgradeScreen();
        else
        {
            upgradePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void ShowSynergyNotification(string message)
    {
        if (synergyNotificationText != null)
        {
            synergyNotificationText.text = message;
            CancelInvoke("ClearSynergyNotification");
            Invoke("ClearSynergyNotification", 2.5f);
        }
    }

    void ClearSynergyNotification()
    {
        if (synergyNotificationText != null)
        {
            synergyNotificationText.text = "";
        }
    }
}