public sealed class PlayerWeaponUpgradeApplicator
{
    private readonly AutoAttack autoAttack;

    public PlayerWeaponUpgradeApplicator(AutoAttack autoAttack)
    {
        this.autoAttack = autoAttack;
    }

    public void Apply(string upgradeType, float value)
    {
        if (autoAttack == null) return;

        switch (upgradeType)
        {
            case "Damage": autoAttack.UpdateDamage(value); break;
            case "Range": autoAttack.UpdateRange(value); break;
            case "FireRate": autoAttack.UpdateFireRate(value); break;
        }
    }
}
