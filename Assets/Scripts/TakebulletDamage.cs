using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public float damage = 25f;  // �ӹǹ damage
    public string targetTag = "Enemy"; // tag �ͧ�ѵ��

    private void OnCollisionEnter(Collision collision)
    {
        // ����� object ��誹�� tag �����ҵ�ͧ����������
        if (collision.gameObject.CompareTag(targetTag))
        {
            // �� EnemyHealthBar �� object ���
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
