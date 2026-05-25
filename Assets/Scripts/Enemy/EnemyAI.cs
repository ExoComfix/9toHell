using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Data Driven Setup")]
    public EnemyData enemyData;

    [SerializeField] private float speedModifier = 1f;
    public float MoveSpeed => enemyData != null ? enemyData.moveSpeed * speedModifier : 2.5f;
    public float MaxHealth => enemyData != null ? enemyData.maxHealth : 20f;
    public float BurnoutDamage => enemyData != null ? enemyData.burnoutDamage : 10f;

    [Header("Pool Config (Corporate Drops)")]
    [SerializeField] private string xpPoolTag = "XP_Signature";

    private float currentHealth;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Coroutine slowCoroutine;
    private EnemyRangedAttackDriver rangedAttack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rangedAttack = new EnemyRangedAttackDriver();
    }

    private void Start()
    {
        ConfigureRigidbody();
        playerTransform = EnemyPlayerTarget.Transform;
    }

    private void OnEnable()
    {
        ResetEnemyStats();
    }

    private void ConfigureRigidbody()
    {
        if (rb == null) return;

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void ResetEnemyStats()
    {
        speedModifier = 1f;
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);

        if (enemyData != null)
        {
            currentHealth = enemyData.maxHealth;
        }
    }

    private void FixedUpdate()
    {
        if (playerTransform == null || rb == null || enemyData == null) return;

        Vector2 toPlayer = (Vector2)playerTransform.position - rb.position;
        float sqrDistanceToPlayer = toPlayer.sqrMagnitude;
        float sqrAttackRange = enemyData.attackRange * enemyData.attackRange;

        if (IsInRangedAttackState(sqrDistanceToPlayer, sqrAttackRange))
        {
            TickRangedAttack(toPlayer);
        }
        else
        {
            TickChase(toPlayer);
        }
    }

    private bool IsInRangedAttackState(float sqrDistanceToPlayer, float sqrAttackRange)
    {
        return enemyData.isRanged && sqrDistanceToPlayer <= sqrAttackRange;
    }

    private void TickRangedAttack(Vector2 toPlayer)
    {
        EnemyMovementDriver.Stop(rb);
        rangedAttack.TryFire(transform, playerTransform, enemyData);
    }

    private void TickChase(Vector2 toPlayer)
    {
        Vector2 direction = toPlayer.normalized;
        EnemyMovementDriver.Chase(rb, direction, MoveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || enemyData == null) return;

        PlayerController playerScript = collision.GetComponent<PlayerController>();
        if (playerScript == null) return;

        playerScript.IncreaseBurnout(enemyData.burnoutDamage);
        gameObject.SetActive(false);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0) Die();
    }

    #region DEBUFFS (SINERJI ETKILERI)

    public void ApplySlow(float slowAmount, float duration = 2f)
    {
        if (!gameObject.activeInHierarchy) return;
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowRoutine(slowAmount, duration));
    }

    private IEnumerator SlowRoutine(float slowAmount, float duration)
    {
        speedModifier = Mathf.Max(slowAmount, 0.2f);
        yield return new WaitForSeconds(duration);
        speedModifier = 1f;
    }

    #endregion

    private void Die()
    {
        EnemyDeathHandler.ExecuteDeath(transform, enemyData, xpPoolTag);
    }
}
