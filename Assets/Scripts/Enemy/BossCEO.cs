using UnityEngine;
using System.Collections;

public class BossCEO : MonoBehaviour
{
    public enum BossPhase { Phase1, Phase2, Phase3 }

    [Header("Data Framework")]
    public EnemyData bossData;

    [Header("Phase Settings")]
    public BossPhase currentPhase = BossPhase.Phase1;

    private float currentHealth;
    private float nextAttackTime = 0f;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool phase2Triggered = false;
    private bool phase3Triggered = false;
    private float activeMoveSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        if (bossData != null)
        {
            currentHealth = bossData.maxHealth;
            activeMoveSpeed = bossData.moveSpeed;
        }
    }
    void FixedUpdate()
    {
        if (playerTransform == null || rb == null || bossData == null) return;
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                BasicChaseAndAttack();
                break;
            case BossPhase.Phase2:
                KPIStationaryAttack();
                break;
            case BossPhase.Phase3:
                EnragedChaseAndAttack();
                break;
        }
    }
    void BasicChaseAndAttack()
    {
        Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
        rb.linearVelocity = direction * activeMoveSpeed;

        if (direction.x > 0) spriteRenderer.flipX = false;
        else if (direction.x < 0) spriteRenderer.flipX = true;

        if (Time.time >= nextAttackTime)
        {
            LaunchCorporateProjectiles(1);
            nextAttackTime = Time.time + bossData.fireRate;
        }
    }
    void KPIStationaryAttack()
    {
        rb.linearVelocity = Vector2.zero;

        if (Time.time >= nextAttackTime)
        {
            LaunchCorporateProjectiles(4);
            nextAttackTime = Time.time + (bossData.fireRate * 0.8f);
        }
    }
    void EnragedChaseAndAttack()
    {
        Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
        rb.linearVelocity = direction * activeMoveSpeed;

        if (Time.time >= nextAttackTime)
        {
            LaunchCorporateProjectiles(8);
            nextAttackTime = Time.time + (bossData.fireRate * 0.5f);
        }
    }
    void LaunchCorporateProjectiles(int count)
    {
        if (ObjectPooler.Instance == null || bossData == null || playerTransform == null) return;

        Vector3 baseDirection = (playerTransform.position - transform.position).normalized;
        float angleStep = 360f / count;
        float angle = 0f;

        for (int i = 0; i < count; i++)
        {
            Vector3 finalDirection = baseDirection;
            if (count > 1)
            {
                float angleRad = angle * Mathf.Deg2Rad;
                finalDirection = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0).normalized;
            }

            Vector3 spawnOffset = finalDirection * 1f;
            GameObject proj = ObjectPooler.Instance.SpawnFromPool(bossData.poolTag, transform.position + spawnOffset, Quaternion.identity);

            if (proj != null)
            {
                EnemyProjectile projScript = proj.GetComponent<EnemyProjectile>();
                if (projScript == null) projScript = proj.AddComponent<EnemyProjectile>();
                float finalDamage = currentPhase == BossPhase.Phase3 ? bossData.burnoutDamage * 1.5f : bossData.burnoutDamage;
                projScript.Setup(finalDirection, finalDamage);
            }

            angle += angleStep;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        float healthPercentage = currentHealth / bossData.maxHealth;

        Debug.Log($"[BOSS] CEO Hasar Aldı! Kalan Can: %{healthPercentage * 100}");

        if (healthPercentage <= 0.6f && healthPercentage > 0.3f && !phase2Triggered)
        {
            TriggerPhase2();
        }
        else if (healthPercentage <= 0.3f && healthPercentage > 0f && !phase3Triggered)
        {
            TriggerPhase3();
        }

        if (currentHealth <= 0) Die();
    }

    void TriggerPhase2()
    {
        phase2Triggered = true;
        currentPhase = BossPhase.Phase2;
        GetComponent<SpriteRenderer>().color = Color.orange;
        Debug.LogWarning("[BOSS ALERT] CEO Evre 2'ye Geçti: Performans Değerlendirmesi Başlıyor! KPI Çemberlerinden Kaçın!");
    }
    void TriggerPhase3()
    {
        phase3Triggered = true;
        currentPhase = BossPhase.Phase3;
        activeMoveSpeed = bossData.moveSpeed * 1.5f;
        GetComponent<SpriteRenderer>().color = Color.red;
        Debug.LogError("[BOSS ALERT] CEO Evre 3'ye Geçti: Şirket Küçülmeye Gidiyor! Agresif İşten Çıkarmalar Başladı!");
    }
    void Die()
    {
        Debug.LogWarning("[VICTORY] CEO İstifasını Verdi! Plaza Tamamen Temizlendi.");
        Manager_UIManager uiManager = FindAnyObjectByType<Manager_UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowVictoryScreen();
        }
        else
        {
            Debug.LogError("Manager_UIManager bulunamadı, zafer ekranı açılamıyor!");
        }
        gameObject.SetActive(false);
    }
}