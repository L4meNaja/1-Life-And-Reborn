using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // เอาตำแหน่ง Camera
        Vector3 targetPosition = cameraTransform.position;

        // ล็อกแกน Y ให้เท่ากับ Tutorial
        // ทำให้ข้อความไม่หันเงยหรือก้มตาม Camera
        targetPosition.y = transform.position.y;

        // หันเฉพาะซ้าย-ขวา
        transform.LookAt(targetPosition);
        transform.Rotate(0f, 180f, 0f);
    }
}