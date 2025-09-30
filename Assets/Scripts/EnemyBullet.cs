using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 10; // กำหนดดาเมจของกระสุน

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // หา PlayerHealth จาก object ที่โดน
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // ไม่ว่าชนอะไร กระสุนหายเสมอ
        Destroy(gameObject);
    }
}
