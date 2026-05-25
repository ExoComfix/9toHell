using UnityEngine;
public static class EnemyDeathHandler
{
    public static void ExecuteDeath(Transform enemyTransform, EnemyData enemyData, string xpPoolTag)
    {
        if (ObjectPooler.Instance == null)
        {
            enemyTransform.gameObject.SetActive(false);
            return;
        }

        ObjectPooler.Instance.SpawnFromPool(xpPoolTag, enemyTransform.position, Quaternion.identity);

        if (enemyData != null && enemyData.potentialDropItems != null && enemyData.potentialDropItems.Length > 0)
        {
            if (Random.value <= enemyData.dropChance)
            {
                string randomItemPoolTag = enemyData.potentialDropItems[Random.Range(0, enemyData.potentialDropItems.Length)];
                ObjectPooler.Instance.SpawnFromPool(randomItemPoolTag, enemyTransform.position, Quaternion.identity);
            }
        }

        enemyTransform.gameObject.SetActive(false);
    }
}
