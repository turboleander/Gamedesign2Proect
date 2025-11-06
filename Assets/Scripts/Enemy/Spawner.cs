using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;      // Prefab ของ Enemy
    public int maxEnemies = 5;          // จำนวนสูงสุดที่ Spawn ได้พร้อมกัน
    public float spawnInterval = 3f;    // เวลาระหว่างการเกิดแต่ละตัว

    [Header("Spawn Area / Points")]
    public Transform[] spawnPoints;     // จุดเกิด (ลากใส่ใน Inspector)

    private float lastSpawnTime;
    private int currentEnemyCount;

    void Update()
    {
        // ถ้าเวลา spawn ถึง และจำนวน enemy ยังไม่เต็ม
        if (Time.time >= lastSpawnTime + spawnInterval && currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null) return;

        // สุ่มจุดเกิด
        int index = Random.Range(0, spawnPoints.Length);
        Transform point = spawnPoints[index];

        // สร้าง enemy
        GameObject enemy = Instantiate(enemyPrefab, point.position, point.rotation);

        // นับจำนวน enemy ที่เกิด
        currentEnemyCount++;

        // ลดจำนวนเมื่อ enemy ตาย (ใช้ callback)
        EnemyDeathHandler deathHandler = enemy.AddComponent<EnemyDeathHandler>();
        deathHandler.spawner = this;
    }

    // เรียกเมื่อศัตรูตัวใดตัวหนึ่งตาย
    public void OnEnemyDeath()
    {
        currentEnemyCount--;
    }

    // แสดง Gizmos ให้เห็นจุด spawn
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var p in spawnPoints)
        {
            if (p != null)
                Gizmos.DrawWireSphere(p.position, 0.5f);
        }
    }
}
