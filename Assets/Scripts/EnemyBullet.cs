using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 10; // ��˹�������ͧ����ع

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �� PlayerHealth �ҡ object ���ⴹ
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // �����Ҫ����� ����ع�������
        Destroy(gameObject);
    }
}
