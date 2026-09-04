using UnityEngine;

public class CameraMainMenu : MonoBehaviour
{
    public float moveAmount = 0.5f; // ความแรงในการขยับ
    public float smoothSpeed = 5f;  // ความลื่น

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // แปลงตำแหน่งเมาส์ให้อยู่ในช่วง -1 ถึง 1
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        Vector3 targetPosition = startPosition + new Vector3(
            mouseX * moveAmount,
            mouseY * moveAmount,
            0
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}