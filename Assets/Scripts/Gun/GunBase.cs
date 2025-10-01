using System.Collections;
using UnityEngine;
using TMPro; // ต้องใส่นี้

public class GunBase : MonoBehaviour
{
    public TMP_Text ammoDisplay; // เปลี่ยนจาก Text เป็น TMP_Text

    [Header("Gun Settings")]
    public string gunName = "DefaultGun";
    public int maxAmmo = 30;
    public int currentAmmo;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletLifeTime = 3f;

    public virtual void Start()
    {
        currentAmmo = maxAmmo;

        // อัพเดต UI ตอนเริ่มเกม
        UpdateAmmoUI();
    }

    public virtual void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            Debug.Log("Shoot with " + gunName + " (" + currentAmmo + "/" + maxAmmo + ")");

            if (bulletPrefab != null && bulletSpawn != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddForce(bulletSpawn.forward * bulletVelocity, ForceMode.Impulse);

                StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));
            }

            UpdateAmmoUI(); // อัพเดต UI หลังยิง
        }
        else
        {
            Debug.Log(gunName + " is out of ammo!");
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoDisplay != null)
            ammoDisplay.text = currentAmmo + " / " + maxAmmo;
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(bullet);
    }
}
