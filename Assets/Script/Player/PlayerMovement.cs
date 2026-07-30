using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    
    [Header("Speed Settings")]
    public float baseSpeed = 10f;
    
    public float walkSpeedMultiplier = 0.6f;  
    public float runSpeedMultiplier = 1.2f;   
    public float crouchSpeedMultiplier = 0.3f;

    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (PlayerStats.playerStats != null)
        {
            baseSpeed = PlayerStats.playerStats.spd;
        }
    }

    void Update()
    {
        // ซ่อนเมาส์และล็อคไว้ตรงกลางหน้าจอเสมอเวลากดคลิก (เผื่อเผลอกด Esc แล้วเมาส์หลุดไปที่ UI)
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (PlayerStats.playerStats != null)
        {
            baseSpeed = PlayerStats.playerStats.spd;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float currentWalkSpeed = baseSpeed * walkSpeedMultiplier;
        float currentRunSpeed = baseSpeed * runSpeedMultiplier;
        float currentCrouchSpeed = baseSpeed * crouchSpeedMultiplier;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        
        float targetSpeed = currentWalkSpeed;
        if (Input.GetKey(KeyCode.LeftControl) && canMove)
        {
            characterController.height = crouchHeight;
            targetSpeed = currentCrouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            targetSpeed = isRunning ? currentRunSpeed : currentWalkSpeed;
        }

        float curSpeedX = canMove ? targetSpeed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? targetSpeed * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}