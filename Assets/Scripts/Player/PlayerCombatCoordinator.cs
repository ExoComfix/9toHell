using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AutoAttack))]

[DisallowMultipleComponent]

public class PlayerCombatCoordinator : MonoBehaviour, ICombatSynergyQuery

{
    public List<string> chosenUpgradeIDs = new List<string>();
    private PlayerSynergyTracker synergyTracker;
    private PlayerWeaponStatApplicator weaponStatApplicator;
    private PlayerWeaponUpgradeApplicator weaponUpgradeApplicator;

    private void Awake()
    {
        AutoAttack autoAttack = GetComponent<AutoAttack>();
        synergyTracker = new PlayerSynergyTracker();
        weaponStatApplicator = new PlayerWeaponStatApplicator(autoAttack);
        weaponUpgradeApplicator = new PlayerWeaponUpgradeApplicator(autoAttack);
    }

    public void ApplyDepartmentWeaponStats(DepartmentData departmentData)

    {

        weaponStatApplicator.ApplyDepartmentStats(departmentData);

    }

    public void ApplyUpgradeToWeapon(string upgradeType, float value)

    {
        weaponUpgradeApplicator.Apply(upgradeType, value);
    }
    public void ActivateSynergy(string synergyID)
    {
        synergyTracker.Activate(synergyID);
    }
    public bool IsSynergyActive(string synergyID)
    {
        return synergyTracker.IsActive(synergyID);
    }
}


