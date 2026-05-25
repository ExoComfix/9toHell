using UnityEngine;
public static class ProjectileLauncher
{
    public static void Fire(
        WeaponData weaponData,
        float damageModifier,
        Vector3 spawnPosition,
        Vector3 direction,
        PlayerController playerController,
        ICombatSynergyQuery synergyQuery)
    {
        if (ObjectPooler.Instance == null || weaponData == null) return;

        GameObject projObj = ObjectPooler.Instance.SpawnFromPool(weaponData.poolTag, spawnPosition, Quaternion.identity);
        if (projObj == null) return;

        Projectile projectile = projObj.GetComponent<Projectile>();
        if (projectile == null) return;

        projectile.Setup(
            direction,
            WeaponDamageCalculator.GetDamage(weaponData.baseDamage, damageModifier),
            weaponData.projectileSpeed,
            weaponData.lifetime,
            playerController,
            synergyQuery);
    }
}
