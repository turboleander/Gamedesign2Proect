using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GunBase gun = other.GetComponentInChildren<GunBase>();
        if (gun != null)
        {
            // เติม currentAmmo ให้เต็ม
            gun.currentAmmo = gun.maxAmmo;

            // อัพเดต UI 
            if (gun.ammoDisplay != null)
                gun.ammoDisplay.text = gun.currentAmmo + " / " + gun.maxAmmo;

            Debug.Log("Picked up ammo! " + gun.gunName + " refilled.");

            Destroy(gameObject); // pickup หายไป
        }
    }
}
