using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyDamageCloseRange : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackDamage = 10f;        // �������ͤ���
    public float attackCooldown = 1.5f;     // ���������ҧ�������

    [Header("Tag Settings")]
    public string playerTag = "Player";     // Tag �ͧ Player

    private float lastAttackTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        // ��� Collider �������Ҥ�� Player
        if (other.CompareTag(playerTag))
        {
            // ��Ǩ�ͺ cooldown ������ն���Թ�
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // �� component PlayerHealth ���ǷӴ����
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

                if (playerHealth != null && !playerHealth.IsDead())
                {
                    playerHealth.TakeDamage(attackDamage);
                    Debug.Log("Enemy hit player for " + attackDamage + " damage!");
                }

                // �Ѿവ������������ش
                lastAttackTime = Time.time;
            }
        }
    }

    // �ʴ��ͺࢵ collider � Scene ���ʹ٧��¢��
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}
