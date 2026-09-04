using UnityEngine;

public class PickableAmmo : MonoBehaviour
{
    [Header("Ammo Settings")]
    public ItemSlot targetSlot = ItemSlot.Primary; // เลือกช่องที่จะเติมกระสุน (Primary หรือ Secondary)
    public int ammoAmount = 30;                    // จำนวนกระสุนที่จะได้เมื่อเก็บ

    [Header("UI Reference")]
    public GameObject interactUI;                  // ลาก UI ข้อความ "Press E to pick up" มาใส่

    private bool playerIsClose = false;
    private PlayerInventory playerInventory;

    void Start()
    {
        if (interactUI != null)
        {
            interactUI.SetActive(false); // ซ่อน UI ตั้งแต่เริ่มเกม
        }
    }

    void Update()
    {
        // ถ้าผู้เล่นอยู่ในระยะ และกดปุ่ม E
        if (playerIsClose && Input.GetKeyDown(KeyCode.E))
        {
            PickUpAmmo();
        }
    }

    void PickUpAmmo()
    {
        if (playerInventory != null)
        {
            // เติมกระสุนสำรองตาม Slot ที่เลือกไว้โดยตรง
            if (targetSlot == ItemSlot.Primary)
            {
                playerInventory.primaryInvAmmo += ammoAmount;
                Debug.Log($"เก็บกระสุนช่อง Primary สำเร็จ! ได้รับเพิ่ม: {ammoAmount}");
            }
            else if (targetSlot == ItemSlot.Secondary)
            {
                playerInventory.secondaryInvAmmo += ammoAmount;
                Debug.Log($"เก็บกระสุนช่อง Secondary สำเร็จ! ได้รับเพิ่ม: {ammoAmount}");
            }

            // อัปเดต UI หน้าจอหลัก
            if (playerInventory.inventoryUI != null)
            {
                playerInventory.inventoryUI.UpdateAllSlotsItemDisplay();
            }

            // ปิด UI และลบกล่องกระสุนทิ้ง
            if (interactUI != null) interactUI.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInventory = collision.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                playerIsClose = true;
                if (interactUI != null) interactUI.SetActive(true); // แสดงข้อความ "Press E..."
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            playerInventory = null;
            if (interactUI != null) interactUI.SetActive(false); // ซ่อนข้อความเมื่อเดินออก
        }
    }
}