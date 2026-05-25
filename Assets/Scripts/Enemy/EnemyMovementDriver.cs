using UnityEngine;
public static class EnemyMovementDriver
{
    public static void Chase(Rigidbody2D rb, Vector2 direction, float moveSpeed)
    {
        if (rb == null) return;
        rb.linearVelocity = direction * moveSpeed;
    }

    public static void Stop(Rigidbody2D rb)
    {
        if (rb == null) return;
        rb.linearVelocity = Vector2.zero;
    }
}
