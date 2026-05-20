using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manager_UIManager : MonoBehaviour 
{
    public Slider burnoutSlider;
    public Slider xpSlider;
    public TextMeshProUGUI levelText;

    private PlayerController player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
        {
            player = playerObj.GetComponent<PlayerController>();
        }
}
    
    void Update()
    {
        if (player == null) return;

        burnoutSlider.value = player.currentBurnout;
        xpSlider.maxValue = player.xpToNextLevel;
        xpSlider.value = player.currentXP;

        string title = "Stajyer";
        if (player.currentLevel == 2) title = "Junior";
        else if (player.currentLevel == 3) title = "Mid-Level";
        else if (player.currentLevel == 4) title = "Senior";
        else if (player.currentLevel == 5) title = "Lead (Burnout Yakın!)";

        levelText.text = $"Unvan: {title} (Lv. {player.currentLevel})";
    }
}
