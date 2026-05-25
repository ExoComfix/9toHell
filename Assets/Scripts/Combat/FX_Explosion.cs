using UnityEngine;

public class FX_Explosion : MonoBehaviour
{
    public float maxRadius = 2.5f;
    public float duration = 0.3f;

    private SpriteRenderer spriteRenderer;
    private float elapsedTime = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        elapsedTime = 0f;
        transform.localScale = Vector3.zero;

        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = 0.6f;
            spriteRenderer.color = color;
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / duration;

        if (progress <= 1f)
        {
            float currentScale = Mathf.Lerp(0f, maxRadius * 2f, progress);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(0.6f, 0f, progress);
                spriteRenderer.color = color;
            }
        }
        else
        {
            if (ObjectPooler.Instance != null)
            {
                ObjectPooler.Instance.ReturnToPool(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}