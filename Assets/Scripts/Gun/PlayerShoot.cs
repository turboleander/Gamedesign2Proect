using UnityEngine;


public class PlayerShoot : MonoBehaviour
{
    public GunBase currentGun; // assign ปืนที่ถือ
    

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentGun.Shoot();
        }
    }
}
