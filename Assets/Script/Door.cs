using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject interactTextUI; // ลาก Canvas World Space มาใส่ที่นี่

    [Header("Movement Settings")]
    public GameObject objectToMove;  // ลากวัตถุที่ต้องการให้ขยับมาใส่ที่นี่ (ถ้าเป็นตัวมันเองก็ลากตัวเองมาใส่)
    public float moveDistance = -10f; // ระยะทางที่ต้องการขยับในแกน X

    private bool isPlayerInTrigger = false;

    void Start()
    {
        // เริ่มต้นเกมให้แน่ใจว่าข้อความถูกซ่อนไว้ก่อน
        if (interactTextUI != null)
        {
            interactTextUI.SetActive(false);
        }

        // หากไม่ได้ลากวัตถุที่จะขยับมา ให้ถือว่าเป็นวัตถุที่ใส่สคริปต์นี้
        if (objectToMove == null)
        {
            objectToMove = this.gameObject;
        }
    }

    void Update()
    {
        // ถ้า Player อยู่ในโซน และกดปุ่ม E
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    // ฟังก์ชันทำงานเมื่อกดปุ่ม E
    void Interact()
    {
        // ขยับตำแหน่งแกน X ของวัตถุไปทางซ้าย -10 (เป้าหมายแบบ Relative)
        Vector3 currentPosition = objectToMove.transform.position;
        objectToMove.transform.position = new Vector3(currentPosition.x + moveDistance, currentPosition.y, currentPosition.z);

        // หลังจากกดใช้งานแล้ว สามารถเลือกว่าจะซ่อนข้อความเลยไหม (ตัวเลือกเสริม)
        if (interactTextUI != null)
        {
            interactTextUI.SetActive(false);
        }
    }

    // ตรวจสอบเมื่อ Player เดินเข้ามาในโซน Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            if (interactTextUI != null)
            {
                interactTextUI.SetActive(true); // แสดง Text Worldspace
            }
        }
    }

    // ตรวจสอบเมื่อ Player เดินออกจากโซน Trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            if (interactTextUI != null)
            {
                interactTextUI.SetActive(false); // ซ่อน Text Worldspace
            }
        }
    }
}
