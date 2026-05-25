public sealed class PlayerWeaponStatApplicator
{
    private readonly AutoAttack autoAttack;

    public PlayerWeaponStatApplicator(AutoAttack autoAttack)
    {
        this.autoAttack = autoAttack;
    }

    public void ApplyDepartmentStats(DepartmentData departmentData)
    {
        if (departmentData == null || autoAttack == null) return;

        autoAttack.UpdateFireRate(departmentData.attackCooldownModifier);
        if (departmentData.damageMultiplier > 1f) autoAttack.UpdateDamage(departmentData.damageMultiplier);
        if (departmentData.rangeBonus > 0f) autoAttack.UpdateRange(departmentData.rangeBonus);
    }
}
