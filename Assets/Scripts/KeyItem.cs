using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // บอก Player ว่าเก็บกุญแจแล้ว
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.hasKey = true;
                Debug.Log("เก็บกุญแจแล้ว!");
            }

            // ลบกุญแจออกจากฉาก
            Destroy(gameObject);
        }
    }
}
