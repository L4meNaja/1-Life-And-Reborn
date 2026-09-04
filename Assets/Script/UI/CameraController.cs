using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Target & Follow")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothSpeed = 10f;

    [Header("Mouse Offset (Optional)")]
    public bool enableMouseOffset = true;
    public float mouseSensitivity = 2f;
    public float maxMouseOffset = 3f;

    [Header("Recoil Settings")]
    public float recoilRecoverySpeed = 12f;
    private Vector3 recoilOffset;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // คำนวณตำแหน่งปกติ
        Vector3 targetPosition = target.position + offset;

        if (enableMouseOffset && Camera.main != null)
        {
            Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector3 mouseOffset = new Vector3(mousePos.x - 0.5f, mousePos.y - 0.5f, 0f) * mouseSensitivity;
            mouseOffset = Vector3.ClampMagnitude(mouseOffset, maxMouseOffset);
            targetPosition += mouseOffset;
        }

        // บวก Recoil เข้าไปตามมุมมอง Local ของกล้อง
        targetPosition += transform.TransformDirection(recoilOffset);

        // เลื่อนกล้อง
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // คืนค่า Recoil กลับเป็น 0
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);
    }

    public void ShoulderOffset(float x, float y, float z)
    {
        // บวกแรง recoil จาก SO เข้าไปตรงๆ
        recoilOffset += new Vector3(x, y, z);
    }
}