using UnityEngine;

public class Billboard3D : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0; // ป้องกันเอียงขึ้นลง
        if (lookPos != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
