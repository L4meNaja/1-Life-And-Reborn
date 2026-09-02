using UnityEngine;

public class BlockWall : MonoBehaviour
{
    [SerializeField] private Transform targetObject; // ใส่ Object ที่ต้องการให้ขยับ
    [SerializeField] private float moveDistance = 10f; // ระยะทางที่ต้องการให้ขยับ
    
    private bool hasTriggered = false; // ตัวแปรเช็กว่าทำงานไปแล้วหรือยัง

    private void OnTriggerEnter(Collider other)
    {
        // เช็กว่าวัตถุที่เข้ามาชนมี Tag ว่า "Player" และยังไม่เคยทำงานมาก่อน
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // ล็อคไว้ทันทีเพื่อไม่ให้ทำงานซ้ำอีก
            MoveObjectRight();
        }
    }

    private void MoveObjectRight()
    {
        if (targetObject != null)
        {
            // บวกตำแหน่งแกน X เพิ่มไปตามระยะที่กำหนด
            Vector3 newPosition = targetObject.position;
            newPosition.x += moveDistance;
            targetObject.position = newPosition;
        }
        else
        {
            Debug.LogWarning("กรุณาใส่ Target Object ใน Inspector ด้วยครับ!");
        }
    }
}
