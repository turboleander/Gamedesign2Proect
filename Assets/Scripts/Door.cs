using UnityEngine;

public class Door : MonoBehaviour
{
    public int doorID;
    public bool isFinalDoor;

    [Header("Door Object")]
    public GameObject actualDoorObject; // กำแพงประตู

    [Header("UI Settings")]
    // (เพิ่ม) ช่องสำหรับลากหน้าจอ Win Screen มาใส่
    public GameObject winScreenUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.HasKey(doorID))
            {
                Debug.Log("เปิดประตูหมายเลข " + doorID + " สำเร็จ!");

                if (isFinalDoor)
                {
                    // === ส่วนที่แก้ไข: เรียกหน้าจอชนะ ===
                    WinGame();
                }
                else
                {
                    // เปิดประตูธรรมดา
                    if (actualDoorObject != null)
                    {
                        Destroy(actualDoorObject);
                    }
                    Destroy(gameObject); // ทำลาย Trigger ทิ้ง
                }
            }
            else
            {
                Debug.Log("ต้องใช้กุญแจหมายเลข " + doorID + " เพื่อเปิดประตูนี้!");
            }
        }
    }

    void WinGame()
    {
        Debug.Log("YOU WIN!");

        if (winScreenUI != null)
        {
            // 1. เปิดหน้าจอชนะ
            winScreenUI.SetActive(true);

            // 2. หยุดเวลาในเกม (ศัตรูจะหยุดเดิน, ปืนจะยิงไม่ได้)
            Time.timeScale = 0f;

            // 3. ปลดล็อกเมาส์ (สำคัญมากสำหรับเกม FPS)
            // เพื่อให้ผู้เล่นเอาเมาส์ไปกดปุ่ม Restart หรือ Menu ได้
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.LogError("อย่าลืมลาก Win Screen UI มาใส่ใน Inspector ของประตูด้วยนะครับ!");
        }
    }
}