using UnityEngine;
[DisallowMultipleComponent]
public class PlayerInput : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    private SpriteRenderer spriteRenderer;
    private Animator anim;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        ReadMovementAxes();
        ApplySpriteFacing();
        UpdateWalkAnimation();
    }

    private void ReadMovementAxes()
    {
        Vector2 rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        MoveInput = rawInput.normalized;
    }

    private void ApplySpriteFacing()
    {
        if (spriteRenderer == null) return;

        if (MoveInput.x > 0f) spriteRenderer.flipX = false;
        else if (MoveInput.x < 0f) spriteRenderer.flipX = true;
    }

    private void UpdateWalkAnimation()
    {
        if (anim == null) return;

        anim.SetBool("isWalking", MoveInput.sqrMagnitude > 0f);
    }
}
