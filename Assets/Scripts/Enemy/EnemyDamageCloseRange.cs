using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyDamageCloseRange : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 10f;        // ดาเมจต่อครั้ง
    public float attackCooldown = 1.5f;     // เวลาระหว่างการโจมตี

    [Header("Tag Settings")]
    public string playerTag = "Player";     // Tag ของ Player

    private float lastAttackTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        // ถ้า Collider ที่เข้ามาคือ Player
        if (other.CompareTag(playerTag))
        {
            // ตรวจสอบ cooldown ไม่ให้ตีถี่เกินไป
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // หา component PlayerHealth แล้วทำดาเมจ
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

                if (playerHealth != null && !playerHealth.IsDead())
                {
                    playerHealth.TakeDamage(attackDamage);
                    Debug.Log("Enemy hit player for " + attackDamage + " damage!");
                }

                // อัพเดตเวลาโจมตีล่าสุด
                lastAttackTime = Time.time;
            }
        }
    }

    // แสดงขอบเขต collider ใน Scene เพื่อดูง่ายขึ้น
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}
