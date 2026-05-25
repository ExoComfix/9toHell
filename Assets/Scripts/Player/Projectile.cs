using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement & Combat")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 3f;
    private Vector3 moveDirection;

    [Header("Synergy Settings")]
    public float explosionRadius = 2.5f;
    public LayerMask enemyLayer;

    [Header("Pool Tags for Effects")]
    [SerializeField] private string hitFXPoolTag = "FX_EmailDestroy";
    [SerializeField] private string explosionFXPoolTag = "FX_Explosion";

    private PlayerController playerRef;
    private ICombatSynergyQuery synergyQuery;

    public void Setup(
        Vector3 direction,
        float damageAmount,
        float projectileSpeed,
        float projectileLifetime,
        PlayerController player = null,
        ICombatSynergyQuery synergy = null)
    {
        moveDirection = direction;
        damage = damageAmount;
        speed = projectileSpeed;
        lifetime = projectileLifetime;
        playerRef = player;
        synergyQuery = synergy;

        CancelInvoke(nameof(DeactivateProjectile));
        Invoke(nameof(DeactivateProjectile), lifetime);
    }
    public void Setup(Vector3 direction, PlayerController player = null)
    {
        playerRef = player;
        synergyQuery = player != null ? player.Combat : null;
        moveDirection = direction;

        CancelInvoke(nameof(DeactivateProjectile));
        Invoke(nameof(DeactivateProjectile), lifetime);
    }

    private void DeactivateProjectile()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProjectileHitProcessor processor = new ProjectileHitProcessor(
            damage,
            explosionRadius,
            enemyLayer,
            hitFXPoolTag,
            explosionFXPoolTag,
            synergyQuery,
            playerRef != null ? playerRef.transform : null,
            transform.position);

        processor.ProcessHit(collision);

        if (collision.CompareTag("Enemy"))
        {
            DeactivateProjectile();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
