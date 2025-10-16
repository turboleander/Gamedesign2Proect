using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // เก็บรายการ ID ของกุญแจที่ผู้เล่นถืออยู่
    public List<int> keys = new List<int>();

    public bool HasKey(int keyID)
    {
        return keys.Contains(keyID);
    }

    public void AddKey(int keyID)
    {
        if (!keys.Contains(keyID))
        {
            keys.Add(keyID);
            Debug.Log("เก็บกุญแจหมายเลข " + keyID);
        }
    }
}
