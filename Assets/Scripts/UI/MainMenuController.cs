using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject upgradePanel;

    void Start()
    {
        if(mainPanel != null) mainPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }
    public void OpenUpgradePanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(true);
    }
    public void CloseUpgradePanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("Oyun kapatılıyor...");
        Application.Quit();
    }
}
