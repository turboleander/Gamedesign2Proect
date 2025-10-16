using UnityEngine;

public class Door : MonoBehaviour
{
    public int doorID;           // ประตูหมายเลขอะไร
    public bool isFinalDoor;     // ถ้า true = ประตูนี้จะจบเกม

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.HasKey(doorID))
            {
                Debug.Log("เปิดประตูหมายเลข " + doorID + " สำเร็จ!");

                // ถ้าเป็นประตูสุดท้าย = จบเกม
                if (isFinalDoor)
                {
                    Debug.Log("จบเกม!");

#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
                else
                {
                    // ถ้าไม่ใช่ประตูจบเกม = แค่ลบประตูออกไป
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.Log("ต้องใช้กุญแจหมายเลข " + doorID + " เพื่อเปิดประตูนี้!");
            }
        }
    }
}
