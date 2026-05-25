using UnityEngine;
public sealed class ProjectileHitProcessor
{
    private readonly float damage;
    private readonly float explosionRadius;
    private readonly LayerMask enemyLayer;
    private readonly string hitFXPoolTag;
    private readonly string explosionFXPoolTag;
    private readonly ICombatSynergyQuery synergyQuery;
    private readonly Transform playerTransform;
    private readonly Vector3 projectilePosition;

    public ProjectileHitProcessor(
        float damage,
        float explosionRadius,
        LayerMask enemyLayer,
        string hitFXPoolTag,
        string explosionFXPoolTag,
        ICombatSynergyQuery synergyQuery,
        Transform playerTransform,
        Vector3 projectilePosition)
    {
        this.damage = damage;
        this.explosionRadius = explosionRadius;
        this.enemyLayer = enemyLayer;
        this.hitFXPoolTag = hitFXPoolTag;
        this.explosionFXPoolTag = explosionFXPoolTag;
        this.synergyQuery = synergyQuery;
        this.playerTransform = playerTransform;
        this.projectilePosition = projectilePosition;
    }

    public void ProcessHit(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        SpawnHitVisuals(collision.transform.position);

        BossCEO boss = collision.GetComponent<BossCEO>();
        if (boss != null)
        {
            CombatDamageApplicator.ApplyToBoss(boss, damage);
            return;
        }

        EnemyAI enemy = collision.GetComponent<EnemyAI>();
        if (enemy == null) return;

        if (synergyQuery != null && synergyQuery.IsSynergyActive(CombatSynergyIds.Overtime))
        {
            ProcessOvertimeExplosion();
        }
        else if (synergyQuery != null)
        {
            ProjectileSynergyEffects.ApplyAfterHit(enemy, damage, synergyQuery, playerTransform);
        }
        else
        {
            CombatDamageApplicator.ApplyToEnemy(enemy, damage);
        }
    }

    private void ProcessOvertimeExplosion()
    {
        Collider2D[] surroundedEnemies = Physics2D.OverlapCircleAll(projectilePosition, explosionRadius, enemyLayer);
        foreach (Collider2D enemyCollider in surroundedEnemies)
        {
            BossCEO boss = enemyCollider.GetComponent<BossCEO>();
            if (boss != null)
            {
                CombatDamageApplicator.ApplyToBoss(boss, damage);
                continue;
            }

            EnemyAI currentEnemy = enemyCollider.GetComponent<EnemyAI>();
            if (currentEnemy != null && synergyQuery != null)
            {
                ProjectileSynergyEffects.ApplyAfterHit(currentEnemy, damage, synergyQuery, playerTransform);
            }
        }

        SpawnExplosionVisuals();
    }

    private void SpawnHitVisuals(Vector3 position)
    {
        if (ObjectPooler.Instance == null) return;
        ObjectPooler.Instance.SpawnFromPool(hitFXPoolTag, position, Quaternion.identity);
    }

    private void SpawnExplosionVisuals()
    {
        if (ObjectPooler.Instance != null)
        {
            GameObject expFX = ObjectPooler.Instance.SpawnFromPool(explosionFXPoolTag, projectilePosition, Quaternion.identity);
            if (expFX != null)
            {
                FX_Explosion fxScript = expFX.GetComponent<FX_Explosion>();
                if (fxScript != null) fxScript.maxRadius = explosionRadius;
            }
        }

        if (Camera.main != null)
        {
            CameraShake shaker = Camera.main.GetComponent<CameraShake>();
            if (shaker != null) shaker.Shake(0.2f, 0.25f);
        }
    }
}
