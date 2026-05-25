using System.Collections.Generic;
public sealed class PlayerSynergyTracker
{
    private readonly HashSet<string> activeSynergies = new HashSet<string>();

    public void Activate(string synergyID)
    {
        if (activeSynergies.Contains(synergyID)) return;

        activeSynergies.Add(synergyID);
        UnityEngine.Debug.LogWarning($"[SYNERGY ACTIVATED] {synergyID} sinerjisi başarıyla devreye alındı!");
    }

    public bool IsActive(string synergyID)
    {
        return activeSynergies.Contains(synergyID);
    }
}
