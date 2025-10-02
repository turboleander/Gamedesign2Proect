using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public float damage = 25f;  // จำนวน damage
    public string targetTag = "Enemy"; // tag ของศัตรู

    private void OnCollisionEnter(Collision collision)
    {
        // เช็คว่า object ที่ชนมี tag ที่เราต้องการหรือไม่
        if (collision.gameObject.CompareTag(targetTag))
        {
            // หา EnemyHealthBar บน object นั้น
            EnemyHealthBar enemy = collision.gameObject.GetComponent<EnemyHealthBar>();
            if (enemy == null)
            {
                enemy = collision.gameObject.GetComponentInParent<EnemyHealthBar>();
            }

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"{collision.gameObject.name} took {damage} damage");
            }
        }
    }
}
