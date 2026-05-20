using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("Stats & Progression")]
    public float currentBurnout = 0f;
    public float maxBurnout = 100f;

    [Space]
    public int currentLevel = 1;
    public float currentXP = 0f;
    public float xpToNextLevel = 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("Untagged") && collision.name.Contains("XP_Signature"))
        {
            GainXP(25f);
            Destroy(collision.gameObject);
        }
    }

    public void GainXP(float amount)
    {
        currentXP += amount;
        Debug.Log($"Kariyer Puanı Kazandın! XP: {currentXP}/{xpToNextLevel}");

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel *= 1.2f;
        moveSpeed += 0.5f;

        Debug.LogWarning($"TERFİ ALDIN! Yeni Unvan Seviyen: {currentLevel}. Artık daha hızlı koşuyorsun!");
    }

    public void IncreaseBurnout(float amount)
    {
        currentBurnout += amount;
        Debug.Log($"Stres Seviyesi: %{currentBurnout}");

        if (currentBurnout >= maxBurnout)
        {
            currentBurnout = maxBurnout;
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("İSTİFA ETTİN! Game Over.");
        Time.timeScale = 0f;
    }
}