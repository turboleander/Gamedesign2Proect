using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Door : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.hasKey)
            {
#if UNITY_EDITOR
                
                EditorApplication.isPlaying = false;
#else
                // ถ้าเป็น Build จริง จะปิดเกมไปเลย
                Application.Quit();
#endif
            }
            else
            {
                Debug.Log("ต้องใช้กุญแจก่อนนะ!");
            }
        }
    }
}
