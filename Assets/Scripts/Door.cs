using UnityEngine;

public class Door : MonoBehaviour
{
    public int doorID;
    public bool isFinalDoor;

    // (เพิ่ม) สร้างช่องใน Inspector
    // ให้ลาก "GameObject ที่เป็นกำแพงประตู" มาใส่ในช่องนี้
    public GameObject actualDoorObject;

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
                    Debug.Log("จบเกม!");
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                }
                else
                {
                    // (แก้ไข) สั่งทำลาย "กำแพงประตู" ที่เราลากมาใส่
                    if (actualDoorObject != null)
                    {
                        Destroy(actualDoorObject);
                    }

                    // (แนะนำ) ทำลายตัว Trigger เองด้วย
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