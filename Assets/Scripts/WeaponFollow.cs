using UnityEngine;

public class WeaponFollowCamera : MonoBehaviour
{
    public Camera playerCamera;
    public Vector3 offset = new Vector3(0.5f, -0.5f, 1f); // ปรับตำแหน่งปืน relative กับกล้อง

    void LateUpdate()
    {
        // ให้ปืนอยู่ที่ตำแหน่งกล้อง + offset
        transform.position = playerCamera.transform.position + playerCamera.transform.TransformDirection(offset);

        // ให้ปืนหันไปทางเดียวกับกล้อง
        transform.rotation = playerCamera.transform.rotation;
    }
}
