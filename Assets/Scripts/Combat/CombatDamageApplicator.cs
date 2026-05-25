using UnityEngine;
public static class CombatDamageApplicator
{
    public static void ApplyToEnemy(EnemyAI enemy, float damage)
    {
        if (enemy == null) return;
        enemy.TakeDamage(damage);
    }

    public static void ApplyToBoss(BossCEO boss, float damage)
    {
        if (boss == null) return;
        boss.TakeDamage(damage);
    }
}
