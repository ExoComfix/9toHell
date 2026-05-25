using System;
using UnityEngine;
[DisallowMultipleComponent]
public class PlayerXPProgression : MonoBehaviour
{
    public float currentXP = 0f;
    public int currentLevel = 1;
    public float xpToNextLevel = 100f;

    private Action<float, float> onXpChanged;
    private Action<int> onLevelChanged;

    public void BindEvents(Action<float, float> xpChanged, Action<int> levelChanged)
    {
        onXpChanged = xpChanged;
        onLevelChanged = levelChanged;
    }

    public void NotifyInitialState()
    {
        onXpChanged?.Invoke(currentXP, xpToNextLevel);
        onLevelChanged?.Invoke(currentLevel);
    }

    public void AddXP(float amount)
    {
        currentXP += amount;
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
        onXpChanged?.Invoke(currentXP, xpToNextLevel);
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = Mathf.Round(xpToNextLevel * 1.2f);
        onLevelChanged?.Invoke(currentLevel);
        onXpChanged?.Invoke(currentXP, xpToNextLevel);
    }
}
