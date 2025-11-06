using UnityEngine;
using System.Collections;
using System.Collections.Generic; // 1. (เพิ่ม) ต้องใช้ List

public class PlayerMelee : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public BoxCollider meleeHitbox;

    [Header("Stats")]
    public float meleeDamage = 25f;
    public float attackDelay = 0.2f;
    public float activeTime = 0.3f;
    public float attackCooldown = 1.0f;

    private bool isAttacking = false;

    // 2. (เพิ่ม) สร้าง List เพื่อจดจำศัตรูที่โดนตีแล้ว
    private List<Collider> hitEnemies = new List<Collider>();

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
        hitEnemies.Clear(); // 3. (เพิ่ม) "ล้าง" รายชื่อเก่าทุกครั้งที่เริ่มฟันใหม่

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
        // 4. (แก้ไข) เช็ค 2 อย่าง: 
        //    1. เป็น "Enemy" หรือไม่
        //    2. "ยังไม่เคย" อยู่ใน List ที่เราจดไว้ ใช่หรือไม่
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other))
        {
            EnemyHealthBar hp = other.GetComponent<EnemyHealthBar>();
            if (hp != null)
            {
                hp.TakeDamage(meleeDamage);
                hitEnemies.Add(other); // (สำคัญ) เพิ่มศัตรูตัวนี้ลง List
            }
        }
    }
}