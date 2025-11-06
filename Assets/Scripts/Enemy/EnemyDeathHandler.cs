using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    public EnemySpawner spawner;

    // เรียกตอน enemy ตาย
    public void Die()
    {
        if (spawner != null)
            spawner.OnEnemyDeath();

        Destroy(gameObject);
    }
}
