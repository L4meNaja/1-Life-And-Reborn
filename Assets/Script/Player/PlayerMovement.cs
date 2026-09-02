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

    public bool canMove = true;

    [Header("Footstep Sound")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;

    [Range(0.5f, 2f)]
    public float footstepPitch = 1f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (PlayerStats.playerStats != null)
        {
            baseSpeed = PlayerStats.playerStats.spd;
        }

        // ตั้งค่า AudioSource
        if (footstepSource != null)
        {
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
            footstepSource.clip = footstepClip;
            footstepSource.pitch = footstepPitch;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
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

        // =========================
        // Footstep Sound
        // =========================

        bool isMoving =
            canMove &&
            characterController.isGrounded &&
            new Vector3(curSpeedX, 0, curSpeedY).magnitude > 0.1f;

        if (isMoving)
        {
            if (footstepSource != null &&
                footstepClip != null &&
                !footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource != null && footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }

        // =========================
        // Camera / Mouse Look
        // =========================

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            playerCamera.transform.localRotation =
                Quaternion.Euler(rotationX, 0, 0);

            transform.rotation *=
                Quaternion.Euler(
                    0,
                    Input.GetAxis("Mouse X") * lookSpeed,
                    0
                );
        }
    }
}