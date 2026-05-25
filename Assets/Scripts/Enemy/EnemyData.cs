using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "9toHell/Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public EnemyRole role;
    public float moveSpeed = 2.5f;
    public float maxHealth = 20f;
    public float burnoutDamage = 10f;

    [Header("Ranged Settings (Micromanager+)")]
    public bool isRanged = false;
    public float attackRange = 5f;
    public float fireRate = 2f;
    public string poolTag = "EnemyReport";

    [Header("Drop Settings")]
    public float dropChance = 0.2f;
    public string[] potentialDropItems;
}