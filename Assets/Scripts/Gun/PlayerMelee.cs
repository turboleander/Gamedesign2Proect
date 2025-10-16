using UnityEngine;
using System.Collections;

public class PlayerMelee : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public BoxCollider meleeHitbox;

    [Header("Stats")]
    public float meleeDamage = 25f;
    public float attackDelay = 0.2f; // ดีเลย์ก่อน hitbox เปิด
    public float activeTime = 0.3f;  // ระยะเวลาที่ hitbox เปิด
    public float attackCooldown = 1.0f; // เวลาระหว่างตีได้อีกครั้ง

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !isAttacking)
        {
            StartCoroutine(MeleeAttack());
        }
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;

        // เล่น animation (ถ้ามี)
        if (animator)
            animator.SetTrigger("Melee");

        
        yield return new WaitForSeconds(attackDelay);
        meleeHitbox.enabled = true;

        
        yield return new WaitForSeconds(activeTime);
        meleeHitbox.enabled = false;

        
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            
            EnemyHealthBar hp = other.GetComponent<EnemyHealthBar>();
            if (hp != null)
            {
                hp.TakeDamage(meleeDamage);
            }
        }
    }
}
