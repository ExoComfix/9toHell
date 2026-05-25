using UnityEngine;
public sealed class EnemyTargetScanner
{
    private readonly Transform origin;
    private readonly LayerMask enemyLayer;
    private readonly System.Func<float> getRange;

    public EnemyTargetScanner(Transform origin, LayerMask enemyLayer, System.Func<float> getRange)
    {
        this.origin = origin;
        this.enemyLayer = enemyLayer;
        this.getRange = getRange;
    }

    public Transform FindNearest()
    {
        float range = getRange();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(origin.position, range, enemyLayer);
        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in hitEnemies)
        {
            float distanceToEnemy = (origin.position - enemy.transform.position).sqrMagnitude;
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }
}
