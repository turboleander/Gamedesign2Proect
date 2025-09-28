using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float gravity = 10f;

    [Header("Turning")]
    public float turnInterval = 3f;
    public float turnAngle = 90f;
    public float turnSpeed = 5f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private float nextTurnTime = 0f;
    private Quaternion targetRotation;

    private EnemyHealthBar healthBar; // อ้างอิง HP

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        targetRotation = transform.rotation;
        nextTurnTime = Time.time + turnInterval;

        // หา EnemyHealthBar ที่อยู่บนตัวเดียวกัน
        healthBar = GetComponent<EnemyHealthBar>();
    }

    void Update()
    {
        // ถ้า HP หมด → ไม่ทำงาน
        if (healthBar != null && healthBar.currentHealth <= 0f)
        {
            return;
        }

        // ค่อย ๆ หมุนไปหาทิศใหม่
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        // เดินไปข้างหน้า
        Vector3 forward = transform.forward;
        moveDirection = forward * walkSpeed;

        // แรงโน้มถ่วง
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = -1f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // เปลี่ยนทิศใหม่ทุกช่วงเวลา
        if (Time.time >= nextTurnTime)
        {
            float randomY = Random.Range(-turnAngle, turnAngle);
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + randomY, 0);
            nextTurnTime = Time.time + turnInterval;
        }
    }
}
