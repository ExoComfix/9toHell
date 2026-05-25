using UnityEngine;
public sealed class EnemyRangedAttackDriver
{
    private const float ProjectileSpawnOffset = 0.8f;

    private float nextFireTime;

    public void TryFire(Transform enemyTransform, Transform playerTransform, EnemyData enemyData)
    {
        if (enemyData == null || playerTransform == null || enemyTransform == null) return;
        if (Time.time < nextFireTime) return;
        if (ObjectPooler.Instance == null) return;

        Vector3 direction = (playerTransform.position - enemyTransform.position).normalized;
        Vector3 spawnOffset = direction * ProjectileSpawnOffset;
        GameObject proj = ObjectPooler.Instance.SpawnFromPool(
            enemyData.poolTag,
            enemyTransform.position + spawnOffset,
            Quaternion.identity);

        if (proj != null)
        {
            EnemyProjectile projScript = proj.GetComponent<EnemyProjectile>();
            if (projScript != null)
            {
                projScript.Setup(direction, enemyData.burnoutDamage);
            }
            else
            {
                Debug.LogError($"[⚠️ OP] {enemyData.poolTag} prefab'ında EnemyProjectile script'i eksik!");
            }
        }

        nextFireTime = Time.time + enemyData.fireRate;
    }
}
