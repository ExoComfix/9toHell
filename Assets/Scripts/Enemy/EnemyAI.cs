using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyRole { Regular, Intern, HR_Manager, SeniorManager, Micromanager }

    [Header("Data Driven Setup")]
    public EnemyData enemyData;

    [SerializeField] private float speedModifier = 1f;
    public float MoveSpeed => enemyData != null ? enemyData.moveSpeed * speedModifier : 2.5f;
    public float MaxHealth => enemyData != null ? enemyData.maxHealth : 20f;
    public float BurnoutDamage => enemyData != null ? enemyData.burnoutDamage : 10f;

    [Header("Pool Config (Corporate Drops)")]
    [SerializeField] private string xpPoolTag = "XP_Signature";

    private float currentHealth;
    private float nextFireTime = 0f;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Coroutine slowCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ConfigureRigidbody();
        FindPlayerReference();
    }

    private void OnEnable()
    {
        ResetEnemyStats();
    }

    private void ConfigureRigidbody()
    {
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    private void FindPlayerReference()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
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

        if (enemyData.isRanged && sqrDistanceToPlayer <= sqrAttackRange)
        {
            rb.linearVelocity = Vector2.zero;

            if (Time.time >= nextFireTime)
            {
                FireReport();
                nextFireTime = Time.time + enemyData.fireRate;
            }
        }
        else
        {
            Vector2 direction = toPlayer.normalized;
            rb.linearVelocity = direction * MoveSpeed;
        }
    }

    private void FireReport()
    {
        if (playerTransform == null || ObjectPooler.Instance == null || enemyData == null) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Vector3 spawnOffset = direction * 0.8f;
        GameObject proj = ObjectPooler.Instance.SpawnFromPool(enemyData.poolTag, transform.position + spawnOffset, Quaternion.identity);

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && enemyData != null)
        {
            PlayerController playerScript = collision.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.IncreaseBurnout(enemyData.burnoutDamage);
                gameObject.SetActive(false);
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0) Die();
    }

    #region DEBUFFS (SINERJI ETKILERI)
    public void ApplySlow(float slowAmount, float duration = 2f)
    {
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
        if (ObjectPooler.Instance == null)
        {
            gameObject.SetActive(false);
            return;
        }
        ObjectPooler.Instance.SpawnFromPool(xpPoolTag, transform.position, Quaternion.identity);
        if (enemyData != null && enemyData.potentialDropItems != null && enemyData.potentialDropItems.Length > 0)
        {
            if (Random.value <= enemyData.dropChance)
            {
                string randomItemPoolTag = enemyData.potentialDropItems[Random.Range(0, enemyData.potentialDropItems.Length)];
                ObjectPooler.Instance.SpawnFromPool(randomItemPoolTag, transform.position, Quaternion.identity);
            }
        }

        gameObject.SetActive(false);
    }
}