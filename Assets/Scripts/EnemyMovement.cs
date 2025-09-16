using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float walkSpeed = 3f;      
    public float gravity = 10f;       
    public float turnInterval = 3f;   
    public float turnAngle = 90f;     

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private float nextTurnTime = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        nextTurnTime = Time.time + turnInterval;
    }

    void Update()
    {
      
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        moveDirection = forward * walkSpeed;

        
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = -1f; 
        }

       
        characterController.Move(moveDirection * Time.deltaTime);

       
        if (Time.time >= nextTurnTime)
        {
            float randomY = Random.Range(-turnAngle, turnAngle);
            transform.Rotate(0, randomY, 0);
            nextTurnTime = Time.time + turnInterval;
        }
    }
}
