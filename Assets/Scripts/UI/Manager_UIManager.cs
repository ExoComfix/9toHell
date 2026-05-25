using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Manager_UIManager : MonoBehaviour
{
    [Header("Core UI Elements")]
    public Slider burnoutSlider;
    public Slider xpSlider;
    public TextMeshProUGUI levelText;

    [Header("Character Selection UI")]
    public GameObject characterSelectionPanel;
    public List<DepartmentData> departmentDataPool = new List<DepartmentData>();
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

    [Header("Objective UI Elements")]
    public GameObject objectivePanel;
    public TextMeshProUGUI objectiveNameText;
    public TextMeshProUGUI objectiveDescText;
    public Slider objectiveProgressSlider;
    public Image sliderFillImage;

    [Header("Victory Screen Settings")]
    public GameObject victoryPanel;

    [Header("Synergy Notification")]
    public TextMeshProUGUI synergyNotificationText;

    [Header("Upgrade Pool")]
    public List<UpgradeCard> allUpgradesPool = new List<UpgradeCard>();
    private List<UpgradeCard> currentSelectedCards = new List<UpgradeCard>();
    private PlayerController player;
    private EnemySpawner enemySpawner;

    private int upgradeQueueCount = 0;
    [HideInInspector] public int currentLevelXP = 0;

    private void Awake()
    {
        SetupButtonListeners();
    }

    private void Start()
    {
        FindPlayerReference();

        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (objectiveProgressSlider != null) objectiveProgressSlider.gameObject.SetActive(false);

        if (characterSelectionPanel != null)
        {
            characterSelectionPanel.SetActive(true);
            Time.timeScale = 0f;
            SetEnemySpawnPause(true);
        }
    }

    private void SetupButtonListeners()
    {
        if (card1Button != null) card1Button.onClick.AddListener(() => OnCardClicked(0));
        if (card2Button != null) card2Button.onClick.AddListener(() => OnCardClicked(1));
        if (card3Button != null) card3Button.onClick.AddListener(() => OnCardClicked(2));

        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);

        if (selectDevButton != null) selectDevButton.onClick.AddListener(() => SelectDepartment(0));
        if (selectMarketingButton != null) selectMarketingButton.onClick.AddListener(() => SelectDepartment(1));
        if (selectAccountingButton != null) selectAccountingButton.onClick.AddListener(() => SelectDepartment(2));
    }

    private void FindPlayerReference()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }

        if (enemySpawner == null)
        {
            enemySpawner = FindAnyObjectByType<EnemySpawner>();
        }
    }

    private void SetEnemySpawnPause(bool paused)
    {
        if (enemySpawner == null)
        {
            enemySpawner = FindAnyObjectByType<EnemySpawner>();
        }

        if (enemySpawner != null)
        {
            enemySpawner.SetUIPause(paused);
        }
    }

    private void OnEnable()
    {
        PlayerController.OnBurnoutChanged += UpdateBurnoutUI;
        PlayerController.OnXPChanged += UpdateXPUI;
        PlayerController.OnLevelChanged += OnPlayerLevelUp;
        PlayerController.OnPlayerDeath += TriggerGameOverScreen;
        FloorManager.OnFloorStateChanged += HandleFloorStateChanged;
        FloorManager.OnCollapseProgressUpdated += HandleCollapseProgressUpdated;
    }
    private void OnDisable()
    {
        PlayerController.OnBurnoutChanged -= UpdateBurnoutUI;
        PlayerController.OnXPChanged -= UpdateXPUI;
        PlayerController.OnLevelChanged -= OnPlayerLevelUp;
        PlayerController.OnPlayerDeath -= TriggerGameOverScreen;
        FloorManager.OnFloorStateChanged -= HandleFloorStateChanged;
        FloorManager.OnCollapseProgressUpdated -= HandleCollapseProgressUpdated;
    }

    #region CORE UI UPDATES
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
    private void OnPlayerLevelUp(int currentLevel)
    {
        currentLevelXP++;
        UpdateLevelUI(currentLevel);
        if (Time.timeSinceLevelLoad > 0.1f)
        {
            upgradeQueueCount++;
            if (upgradePanel == null || !upgradePanel.activeSelf)
            {
                ShowUpgradeScreen();
            }
        }
    }

    private void UpdateLevelUI(int currentLevel)
    {
        if (levelText != null)
        {
            string deptName = (player != null && player.currentDepartmentData != null)
                ? player.currentDepartmentData.departmentName
                : "Çalışan";

            levelText.text = $"Unvan: {deptName} (Lv. {currentLevel})";
        }
    }
    #endregion

    #region DEPARTMENT & PROGRESSION SELECTION
    private void SelectDepartment(int deptIndex)
    {
        if (player == null) FindPlayerReference();
        if (player == null) return;

        if (departmentDataPool != null && deptIndex < departmentDataPool.Count)
        {
            player.currentDepartmentData = departmentDataPool[deptIndex];
        }
        else
        {
            Debug.LogError($"[UI MANAGER] departmentDataPool içinde {deptIndex}. indexte departman verisi bulunamadı!");
            return;
        }

        player.ApplyDepartmentStats();

        if (burnoutSlider != null)
        {
            burnoutSlider.maxValue = player.maxBurnout;
            burnoutSlider.value = 0f;
        }

        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);

        Time.timeScale = 1f;
        SetEnemySpawnPause(false);
        Debug.Log($"[SYSTEM] Oyun {player.currentDepartmentData.departmentName} olarak başladı!");
        UpdateLevelUI(player.currentLevel);
    }
    #endregion

    #region MODULAR UPGRADE & SYNERGY LOGIC
    private void ShowUpgradeScreen()
    {
        if (allUpgradesPool.Count < 3)
        {
            Debug.LogError("[UPGRADE] Geliştirme havuzunda en az 3 adet UpgradeCard olmalı!");
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

        if (upgradePanel != null) upgradePanel.SetActive(true);
        Time.timeScale = 0f;
        SetEnemySpawnPause(true);
    }
    private void SetButtonTexts(Button button, UpgradeCard cardData)
    {
        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = $"{cardData.cardTitle}\n\n<size=80%>{cardData.cardDescription}</size>";
        }
    }

    private void OnCardClicked(int cardIndex)
    {
        if (player == null || cardIndex >= currentSelectedCards.Count) return;

        string chosenID = currentSelectedCards[cardIndex].upgradeID;

        if (!player.chosenUpgradeIDs.Contains(chosenID))
        {
            player.chosenUpgradeIDs.Add(chosenID);
        }
        ApplyStatUpgrade(chosenID);
        CheckAndActivateSynergies();

        upgradeQueueCount--;
        if (upgradeQueueCount > 0)
        {
            ShowUpgradeScreen();
        }
        else
        {
            if (upgradePanel != null) upgradePanel.SetActive(false);
            Time.timeScale = 1f;
            SetEnemySpawnPause(false);
        }
    }

    private void ApplyStatUpgrade(string upgradeID)
    {
        if (player == null) return;

        switch (upgradeID)
        {
            case "AttackSpeed": player.ApplyUpgradeToWeapon("FireRate", 0.75f); break;
            case "Damage": player.ApplyUpgradeToWeapon("Damage", 1.5f); break;
            case "Range": player.ApplyUpgradeToWeapon("Range", 2.0f); break;
            case "MoveSpeed": player.ModifyMoveSpeed(1.5f); break;
            case "MaxHealth": player.ResetBurnout(); break;
            default:
                Debug.LogWarning($"[UI] '{upgradeID}' ID'si ile eşleşen geliştirme bulunamadı! Teselli olarak stres sıfırlandı.");
                player.ResetBurnout();
                break;
        }
    }
    private void CheckAndActivateSynergies()
    {
        if (player == null) return;

        var chosen = player.chosenUpgradeIDs;

        // Mesai Patlaması Sinerjisi (Overtime)
        if (chosen.Contains("AttackSpeed") && chosen.Contains("Damage"))
        {
            player.ActivateSynergy("Overtime");
            ShowSynergyNotification("<color=#FFCC00><b>SİNERJİ AKTİF: MESAİ PATLAMASI</b></color>\nKahve bardakları artık alan hasarı veriyor!");
        }

        // Deadline Paneli Sinerjisi (Deadline)
        if (chosen.Contains("Range") && chosen.Contains("Damage"))
        {
            player.ActivateSynergy("Deadline");
            ShowSynergyNotification("<color=#FF3333><b>🚨 SİNERJİ: DEADLINE PANELİ</b></color>\nAtışlar çarptığı yerde düşmanları %50 yavaşlatan kırmızı teslim bölgeleri oluşturuyor!");
        }

        // Home Office Lüksü Sinerjisi (HomeOffice)
        if (chosen.Contains("MoveSpeed") && chosen.Contains("MaxHealth"))
        {
            player.ActivateSynergy("HomeOffice");
            ShowSynergyNotification("<color=#33FF33><b>🏡 SİNERJİ: HOME OFFICE LÜKSÜ</b></color>\nKahve bardakları artık stajyerleri ve müdürleri kurumsal bir güçle geriye fırlatıyor!");
        }

        // Brainstorming Sinerjisi (Brainstorm)
        if (chosen.Contains("AttackSpeed") && chosen.Contains("Range"))
        {
            player.ActivateSynergy("Brainstorm");
            ShowSynergyNotification("<color=#3399FF><b>💡 SİNERJİ: BRAINSTORMING</b></color>\nArtık tek bir bardak değil, plaza diliyle harmanlanmış çoklu kahve dalgası fırlatıyorsun!");
        }
    }
    #endregion

    #region NOTIFICATIONS & GAME OVER / VICTORY SCREENS
    public void ShowSynergyNotification(string message)
    {
        if (synergyNotificationText != null)
        {
            synergyNotificationText.text = message;
            CancelInvoke(nameof(ClearSynergyNotification));
            Invoke(nameof(ClearSynergyNotification), 2.5f);
        }
    }
    private void ClearSynergyNotification()
    {
        if (synergyNotificationText != null) synergyNotificationText.text = "";
    }
    public void TriggerGameOverScreen()
    {
        if (gameOverPanel == null) return;

        Time.timeScale = 0f;
        SetEnemySpawnPause(true);

        float totalSeconds = Time.timeSinceLevelLoad;
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);

        if (surviveTimeText != null)
        {
            surviveTimeText.text = $"Toplam Mesai Süresi: <color=#FFCC00>{minutes:00}:{seconds:00}</color>";
        }

        if (finalLevelText != null && player != null)
        {
            string finalTitle = levelText != null ? levelText.text.Split('(')[0] : "Çalışan";
            finalLevelText.text = $"{finalTitle}\n(<color=#00FFCC>Seviye {player.currentLevel}</color> konumunda istifa edildi)";
        }

        gameOverPanel.SetActive(true);
        ConvertXPToSeniorityPoints();
    }
    private void HandleFloorStateChanged(FloorManager.FloorState newState)
    {
        switch (newState)
        {
            case FloorManager.FloorState.Exploration:
                if (objectivePanel != null) objectivePanel.SetActive(true);
                break;
            case FloorManager.FloorState.CorporateCollapse:
                if (objectiveDescText != null)
                    objectiveDescText.text = "<color=red><b>⚠️ ŞİRKET ÇÖKÜYOR! BOSS ÇAĞIRILMAYA HAZIR!</b></color>";
                break;
            case FloorManager.FloorState.BossEncounter:
                if (objectiveNameText != null) objectiveNameText.text = "[👹 EXECUTIVE ALERT]";
                break;
            case FloorManager.FloorState.FloorCleared:
                if (objectivePanel != null) objectivePanel.SetActive(false);
                ShowVictoryScreen();
                break;
        }
    }
    public void InitializeObjectiveUI(string name, string desc, float maxProgress)
    {
        if (objectivePanel != null) objectivePanel.SetActive(true);
        if (objectiveProgressSlider != null)
        {
            objectiveProgressSlider.gameObject.SetActive(true);
            objectiveProgressSlider.maxValue = maxProgress;
            objectiveProgressSlider.value = 0f;
        }
        if (objectiveNameText != null) objectiveNameText.text = name;
        if (objectiveDescText != null) objectiveDescText.text = desc;
    }
    public void UpdateObjectiveProgress(float currentProgress, bool isPlayerInside)
    {
        if (objectiveProgressSlider == null) return;

        objectiveProgressSlider.value = currentProgress;
        if (sliderFillImage != null)
        {
            sliderFillImage.color = isPlayerInside ? Color.cyan : Color.red;
        }

        if (objectiveDescText != null)
        {
            float percentage = (currentProgress / objectiveProgressSlider.maxValue) * 100f;
            objectiveDescText.text = isPlayerInside
                ? $"Reboot İşlemi: %{percentage:F0} (Sistem Odasındasınız)"
                : $"<color=red>⚠️ ALANDAN AYRILDINIZ! (Progress Azalıyor: %{percentage:F0})</color>";
        }
    }
    public void ClearObjectiveUI(string completionMessage)
    {
        if (objectiveDescText != null) objectiveDescText.text = $"<color=green><b>{completionMessage}</b></color>";
        StartCoroutine(HideSliderRoutine());
    }

    private System.Collections.IEnumerator HideSliderRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (objectiveProgressSlider != null) objectiveProgressSlider.gameObject.SetActive(false);
    }
    public void ShowVictoryScreen()
    {
        SetEnemySpawnPause(true);

        if (victoryPanel != null)
        {
            Time.timeScale = 0f;
            victoryPanel.SetActive(true);
            Debug.Log("[VICTORY] Zafer ekranı gösteriliyor!");
        }

        if (DataManager.Instance != null && player != null)
        {
            int pointsEarned = player.currentLevel * 10;
            DataManager.Instance.AddPoints(pointsEarned);
        }

        ConvertXPToSeniorityPoints();
    }
    public void ContinueOvertime()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
            Time.timeScale = 1f;
        }

        SetEnemySpawnPause(false);

        if (FloorManager.Instance != null)
        {
            FloorManager.Instance.RequestContinueAfterVictory();
        }
    }
    private void ConvertXPToSeniorityPoints()
    {
        if (DataManager.Instance != null && currentLevelXP > 0)
        {
            DataManager.Instance.AddPoints(currentLevelXP);
            Debug.Log($"[CONVERSION] {currentLevelXP} XP kıdem puanına dönüştürüldü!");
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Button_RestartGame() => RestartGame();

    public void Button_ReturnToMainMenu() => ReturnToMainMenu();

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    private void HandleCollapseProgressUpdated(float percentage)
    {
    }
    #endregion
}