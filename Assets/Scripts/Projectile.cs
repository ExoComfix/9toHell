using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 3f;
    private Vector3 moveDirection;

    private GameObject xpPrefab;

    public void Setup(Vector3 direction)
    {
        this.moveDirection = direction;
        xpPrefab = Resources.Load<GameObject>("XP_Signature");
        Destroy(gameObject, lifetime);
    }
    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if(xpPrefab != null) 
            {
                Instantiate(xpPrefab,collision.transform.position,Quaternion.identity);
            }
            else 
            {
                GameObject loadedXP = GameObject.Find("XP_Signature");
                if(loadedXP != null) Instantiate(loadedXP,collision.transform.position,Quaternion.identity);
            }
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}