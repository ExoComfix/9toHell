using UnityEngine;
using UnityEngine.UI;

public class Manager_UIManager : MonoBehaviour 
{
    public Slider burnoutSlider;
    public Slider xpSlider;

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
    }
}
