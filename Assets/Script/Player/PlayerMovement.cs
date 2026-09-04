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

    [Header("Slide Settings")]
    public float slideSpeedMultiplier = 1.6f; // ความเร็วตอนสไลด์ (แรงกว่าวิ่งปกติ)
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float slideDuration = 0.75f;       // เวลาสไลด์ 1 วินาที
    private Vector3 slideDirection;           // ทิศทางที่จะพุ่งไปตอนสไลด์

    public float sliderTimer = 2.0f;

    public float jumpPower = 7f;
    public float gravity = 50f;
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
        
        if (sliderTimer > 0)
        {
            if (sliderTimer > 2.0f)
            {
                sliderTimer = 2.0f;
            }
            
            sliderTimer -= Time.deltaTime;
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
        float currentSlideSpeed = baseSpeed * slideSpeedMultiplier;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isPressingCrouch = Input.GetKey(KeyCode.LeftControl);

        // เช็คเงื่อนไขเริ่มสไลด์: ต้องอยู่บนพื้น กำลังวิ่งอยู่ แล้วจังหวะนั้นกดย่อลงพอดี (GetKeyDown)
        if (characterController.isGrounded && isRunning && Input.GetKeyDown(KeyCode.LeftControl) && !isSliding && canMove)
        {
            isSliding = true;
            slideTimer = slideDuration;
            // ล็อคทิศทางข้างหน้าที่กำลังมองหรือกำลังเดินอยู่ตอนกดสไลด์
            slideDirection = forward;
            slideDirection.y = 0;
            slideDirection = slideDirection.normalized;
        }

        float targetSpeed = currentWalkSpeed;

        if (isSliding)
        {
            characterController.height = crouchHeight;
            targetSpeed = currentSlideSpeed;

            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0 || !canMove)
            {
                isSliding = false; // หมดเวลา 1 วิ หยุดสไลด์
            }
        }
        else if (isPressingCrouch && canMove)
        {
            characterController.height = crouchHeight;
            targetSpeed = currentCrouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            targetSpeed = isRunning ? currentRunSpeed : currentWalkSpeed;
        }

        float curSpeedX = 0;
        float curSpeedY = 0;

        if (canMove)
        {
            if (isSliding)
            {
                // ถ้ากำลังสไลด์ จะไม่สนปุ่มบังคับทิศทาง แต่จะพุ่งไปตามทิศทางสไลด์ตรงๆ เองเลย
                curSpeedX = targetSpeed; 
                curSpeedY = 0;
            }
            else
            {
                curSpeedX = targetSpeed * Input.GetAxis("Vertical");
                curSpeedY = targetSpeed * Input.GetAxis("Horizontal");
            }
        }

        float movementDirectionY = moveDirection.y;
        
        if (isSliding)
        {
            moveDirection = slideDirection * curSpeedX;
        }
        else
        {
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        }

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