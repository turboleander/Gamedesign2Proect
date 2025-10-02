using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.hasKey)
            {
                Debug.Log("เปิดประตู!");
                Destroy(gameObject); // ลบประตูออก
                Time.timeScale = 0f;
            }
            else
            {
                Debug.Log("ต้องการกุญแจเพื่อเปิดประตู!");
            }
        }
    }
}
