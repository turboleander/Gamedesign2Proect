using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 30; // จำนวนที่จะเพิ่ม ให้แก้ตามต้องการ

    private void OnTriggerEnter(Collider other)
    {
        GunBase gun = other.GetComponentInChildren<GunBase>();
        if (gun != null)
        {
            // บวกกระสุนเพิ่มเข้าไป
            gun.currentAmmo += ammoAmount;

            // ให้ GunBase อัพเดต UI เอง
            gun.SendMessage("UpdateAmmoUI", SendMessageOptions.DontRequireReceiver);

            Debug.Log($"Picked up ammo! {gun.gunName} +{ammoAmount} ammo.");

            Destroy(gameObject); // ทำลาย pickup
        }
    }
}
