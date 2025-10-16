using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public int keyID = 1; // รหัสกุญแจ

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddKey(keyID);
                Destroy(gameObject); // ลบกุญแจหลังเก็บ
            }
        }
    }
}
