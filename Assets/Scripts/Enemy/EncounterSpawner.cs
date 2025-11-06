// ต้องเพิ่มบรรทัดนี้เพื่อใช้งาน List
using System.Collections.Generic;
using UnityEngine;

public class EncounterSpawner : MonoBehaviour
{
    // (เหมือนเดิม) รายการ Prefab ศัตรู (ใส่กี่แบบก็ได้)
    public GameObject[] enemyPrefabs;

    // (ใหม่) จำนวนที่จะสร้างในแต่ละรอบ
    public int spawnAmount = 10;

    // (เหมือนเดิม) รัศมีที่จะสุ่มเกิด
    public float spawnRadius = 15.0f;

    // (ใหม่) รายการสำหรับติดตามศัตรูที่ยังมีชีวิตอยู่
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    // (ใหม่) ตัวแปรสถานะ: true = ยังมีศัตรูในรอบนี้เหลืออยู่
    private bool isWaveActive = false;

    // (ใหม่) ใช้สำหรับแสดง Gizmos ในหน้า Editor (วงกลมสีแดง)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    // --- ส่วนที่ 1: การตรวจจับผู้เล่น (Trigger) ---
    private void OnTriggerEnter(Collider other)
    {
        // 1. ตรวจสอบว่าเป็น Player
        // 2. ตรวจสอบว่า "isWaveActive" เป็น false (หมายความว่ารอบที่แล้วถูกฆ่าหมดแล้ว)
        if (other.CompareTag("Player") && !isWaveActive)
        {
            // เริ่มสร้างศัตรูรอบใหม่
            SpawnWave();
        }
    }

    // --- ส่วนที่ 2: การสร้างศัตรูเป็นกลุ่ม ---
    void SpawnWave()
    {
        Debug.Log("เริ่ม Wave ใหม่! สร้างศัตรู " + spawnAmount + " ตัว");
        isWaveActive = true; // ตั้งค่าสถานะว่า "รอบนี้เริ่มแล้ว"
        spawnedEnemies.Clear(); // เคลียร์รายการเก่า (เผื่อมี)

        // วนลูปสร้างศัตรู 10 ครั้ง
        for (int i = 0; i < spawnAmount; i++)
        {
            // สุ่มประเภทศัตรู (เหมือนโค้ดก่อนหน้า)
            int prefabIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[prefabIndex];

            // สุ่มตำแหน่ง (เหมือนโค้ดก่อนหน้า)
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            // สร้างศัตรู
            GameObject newEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);

            // !! สำคัญ: เพิ่มศัตรูตัวนี้ลงในรายการติดตาม
            spawnedEnemies.Add(newEnemy);
        }
    }

    // --- ส่วนที่ 3: การตรวจสอบว่า "ยิงหมด" หรือยัง ---
    // Update จะทำงานทุกเฟรม
    void Update()
    {
        // ถ้า Wave ยังไม่เริ่ม (isWaveActive เป็น false) ก็ไม่ต้องทำอะไร
        if (!isWaveActive)
        {
            return;
        }

        // (สำคัญ) ตรวจสอบรายการศัตรู
        // เราจะวนลูปแบบย้อนกลับ (เพื่อไม่ให้ List เพี้ยนตอนลบ)
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            // ถ้าศัตรูในรายการกลายเป็น 'null' (หมายความว่ามันถูก Destroy() ไปแล้ว)
            if (spawnedEnemies[i] == null)
            {
                // ลบออกจากรายการติดตาม
                spawnedEnemies.RemoveAt(i);
            }
        }

        // หลังจากตรวจสอบทั้ง List แล้ว...
        // ถ้าจำนวนศัตรูในรายการ = 0 (ตายหมดแล้ว)
        if (spawnedEnemies.Count == 0)
        {
            Debug.Log("Wave เคลียร์แล้ว! Spawner พร้อมทำงานรอบต่อไป");
            isWaveActive = false; // รีเซ็ตสถานะ -> Spawner พร้อมให้ Trigger ทำงานใหม่
        }
    }
}