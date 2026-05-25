using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;

    public float MoveSpeed => moveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ConfigureRigidbody()
    {
        if (rb == null) return;

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void AddMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

    public void ApplyVelocity(Vector2 moveInput)
    {
        if (rb == null) return;

        rb.linearVelocity = moveInput * moveSpeed;
    }
}
