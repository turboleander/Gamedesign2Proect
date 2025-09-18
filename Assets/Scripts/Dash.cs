using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public float dashSpeed;
    Rigidbody rig;
    bool isDashing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isDashing = true;
        }

    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            Dashing();
        }
    }
    private void Dashing()
    {
        rig.AddForce(transform.forward * dashSpeed,ForceMode.Impulse);
        isDashing = false;
    }
}
