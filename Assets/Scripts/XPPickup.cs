using UnityEngine;

public class XPPickup : MonoBehaviour
{
    private Transform playerTransform;
    private bool isFlying = false;
    private float flySpeed = 8f;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (isFlying && playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, flySpeed * Time.deltaTime);
            flySpeed += Time.deltaTime * 5f;
        }
    }
    public void TriggerMagnet()
    {
        isFlying = true;
    }
}