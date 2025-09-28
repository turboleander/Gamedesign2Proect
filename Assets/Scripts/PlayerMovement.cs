using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // =======================
    // References
    // =======================
    [Header("References")]
    public Transform cameraHolder;   // Empty ที่หิ้วกล้อง
    public Camera playerCamera;

    [Header("Player Health")]
    public int maxHP = 100;
    public int currentHP;
    public GameObject deadScreen;

    [Header("HP UI")]
    public Scrollbar healthBar;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.4f;
    public float dashCooldown = 0.2f;

    [Header("Movement Settings")]
    public float walkSpeed = 12f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 85f; // มุมกล้องแกน X
    public float defaultHeight = 2f;

    // =======================
    // Private Variables
    // =======================
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;

    private bool canMove = true;
    private bool isDashing = false;
    private bool canDash = true;

    private int jumpCount = 0;
    private int maxJumpCount = 2;

    private bool hasDead = false;

    // =======================
    // Unity Callbacks
    // =======================
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentHP = maxHP;
        UpdateHealthBar();
    }

    void Update()
    {
        if (hasDead) return;

        HandleMovement();
        HandleCamera();
        HandleDash();
    }

    // =======================
    // Movement + Jump
    // =======================
    void HandleMovement()
    {
        // Get input
        //bool isRunning = Input.GetKey(KeyCode.LeftShift);
        // ตัดการวิ่งออกไป ใช้แต่ walkSpeed
        float speed = walkSpeed;

        float moveZ = Input.GetAxis("Vertical") * (walkSpeed);
        float moveX = Input.GetAxis("Horizontal") * (walkSpeed);

        // Calculate horizontal movement
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 horizontalMove = (forward * moveZ) + (right * moveX);

        moveDirection.x = horizontalMove.x;
        moveDirection.z = horizontalMove.z;

        // Reset jump count if grounded
        if (characterController.isGrounded)
        {
            jumpCount = 0;
            moveDirection.y = -1f; // กันตัวลอยค้างบนพื้น
        }

        // Double Jump
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            moveDirection.y = jumpPower;
            jumpCount++;
        }

        // Gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Apply movement
        characterController.Move(moveDirection * Time.deltaTime);
    }

    // =======================
    // Camera Look
    // =======================
    /*void HandleCamera()
    {
        // Rotate camera on X
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        cameraHolder.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // Rotate player on Y
        float rotationY = Input.GetAxis("Mouse X") * lookSpeed;
        transform.rotation *= Quaternion.Euler(0, rotationY, 0);
    }*/

    // =======================
    // Dash
    // =======================
    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.C) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float startTime = Time.time;

        // Dash direction = input direction if any, otherwise forward
        Vector3 dashDir = (moveDirection.x != 0 || moveDirection.z != 0) ?
            new Vector3(moveDirection.x, 0, moveDirection.z).normalized :
            transform.forward;

        // Smooth dash
        while (Time.time < startTime + dashDuration)
        {
            float t = (Time.time - startTime) / dashDuration;
            float speedMultiplier = Mathf.Lerp(1f, 0f, t); // smooth slow down
            characterController.Move(dashDir * dashSpeed * speedMultiplier * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // =======================
    // Health
    // =======================
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            currentHP = 0;
            hasDead = true;
            deadScreen.SetActive(true);
        }
        UpdateHealthBar();
    }

    public void AddHP(int heal)
    {
        currentHP += heal;
        if (currentHP > maxHP) currentHP = maxHP;
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.size = (float)currentHP / maxHP;
        }
    }
    void HandleCamera()
    {
        // กล้องก้มเงย (X axis)
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        cameraHolder.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // ตัวละครหันซ้ายขวา (Y axis)
        float rotationY = Input.GetAxis("Mouse X") * lookSpeed;
        transform.Rotate(0, rotationY, 0);
    }


}
