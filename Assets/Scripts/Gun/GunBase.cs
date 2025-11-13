using System.Collections;
using UnityEngine;
using TMPro;

public class GunBase : MonoBehaviour
{
    public TMP_Text ammoDisplay;

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

        // UI เริ่มต้นจะเป็น "Ammo 30"
        UpdateAmmoUI();
    }

    public virtual void Shoot()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            Debug.Log($"Shoot with {gunName} ({currentAmmo})");

            if (bulletPrefab != null && bulletSpawn != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddForce(bulletSpawn.forward * bulletVelocity, ForceMode.Impulse);

                StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));
            }

            UpdateAmmoUI();
        }
        else
        {
            Debug.Log($"{gunName} is out of ammo!");
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;

        // ถ้าต้องการไม่ให้มี max ammo limit ก็ไม่ต้องเช็ค
        // ถ้าต้องการ limit ค่อยใส่เพิ่มได้

        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoDisplay != null)
            ammoDisplay.text = $"Ammo {currentAmmo}";
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(bullet);
    }
}
