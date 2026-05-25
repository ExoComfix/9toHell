using UnityEngine;
public static class ProjectileSynergyEffects
{
    public static void ApplyAfterHit(
        EnemyAI enemy,
        float damage,
        ICombatSynergyQuery synergyQuery,
        Transform playerTransform)
    {
        if (enemy == null || synergyQuery == null) return;

        CombatDamageApplicator.ApplyToEnemy(enemy, damage);
        if (!enemy.gameObject.activeInHierarchy) return;

        if (synergyQuery.IsSynergyActive(CombatSynergyIds.Deadline))
        {
            enemy.ApplySlow(0.5f);
        }

        if (synergyQuery.IsSynergyActive(CombatSynergyIds.HomeOffice) && playerTransform != null)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockbackDirection = (enemy.transform.position - playerTransform.position).normalized;
                enemyRb.AddForce(knockbackDirection * 15f, ForceMode2D.Impulse);
            }
        }
    }
}
