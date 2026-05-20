using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 3f;
    private Vector3 moveDirection;

    public void Setup(Vector3 direction)
    {
        this.moveDirection = direction;
        // Havada asılı kalmasın diye 3 saniye sonra kendini yok eder
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Çarptığı nesnenin Tag'i "Enemy" ise
        if (collision.CompareTag("Enemy"))
        {
            // MVP kuralı: Şimdilik düşmanı ve mermiyi direkt yok et
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}