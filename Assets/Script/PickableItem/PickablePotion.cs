using UnityEngine;

public class PickablePotion : MonoBehaviour
{
    [Header("Potion Settings")]
    public int targetSlotIndex = 4; // ช่องที่จะเก็บ (นับจาก 0 เช่น ช่องที่ 5 คือ Index 4)
    public int amountToAdd = 1;     // จำนวนขวดที่จะได้รับเพิ่ม

    [Header("UI Reference")]
    public GameObject interactUI;

    private bool playerIsClose = false;
    private PlayerInventory playerInventory;

    void Start()
    {
        if (interactUI != null)
        {
            interactUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerIsClose && Input.GetKeyDown(KeyCode.E))
        {
            PickUpPotion();
        }
    }

    void PickUpPotion()
    {
        if (playerInventory != null)
        {
            // แปลง targetSlotIndex ให้ตรงกับ ItemSlot ของยา
            ItemSlot targetSlot = ItemSlot.HealthPotion;
            if (targetSlotIndex == 3) targetSlot = ItemSlot.HealthPotion;
            else if (targetSlotIndex == 4) targetSlot = ItemSlot.ShieldPotion;

            // ดึงจำนวนเดิมมาบวกเพิ่มเข้าไป
            int currentCount = playerInventory.GetCountBySlot(targetSlot);
            if (currentCount < 0) currentCount = 0;

            int newTotalCount = currentCount + amountToAdd;

            // อัปเดตจำนวนกลับเข้าไปใน PlayerInventory
            if (targetSlot == ItemSlot.HealthPotion)
            {
                playerInventory.healthPotionCount = newTotalCount;
            }
            else if (targetSlot == ItemSlot.ShieldPotion)
            {
                playerInventory.shieldPotionCount = newTotalCount;
            }

            Debug.Log($"เก็บโพชันเข้าช่องที่ {targetSlotIndex + 1} สำเร็จ! เพิ่มจำนวน: {amountToAdd} | รวมทั้งหมด: {newTotalCount}");

            // อัปเดต UI หน้าจอ
            if (playerInventory.inventoryUI != null)
            {
                playerInventory.inventoryUI.UpdateAllSlotsItemDisplay();
            }

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
                if (interactUI != null) interactUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            playerInventory = null;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }
}