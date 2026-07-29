using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("UI Reference")]
    public InventoryUI inventoryUI;

    [Header("Hand Transform (จุดอ้างอิงที่มือ)")]
    public Transform handTransform; // ลาก Transform เปล่าที่มือมาใส่ตรงนี้
    private GameObject currentSpawnedModel; // เก็บโมเดลที่กำลังถืออยู่ปัจจุบัน

    [Header("Equipped Items (Scriptable Objects)")]
    public InventoryEquipment primaryEquipment;
    public InventoryEquipment secondaryEquipment;
    public InventoryEquipment meleeEquipment;
    public InventoryEquipment healthPotionEquipment;
    public InventoryEquipment shieldPotionEquipment;

    [Header("Active Slot")]
    public ItemSlot currentSelectedSlot = ItemSlot.Primary;

    void Start()
    {
        // เริ่มต้นเลือกช่อง 1
        SelectSlot(ItemSlot.Primary, 0);
    }

    void Update()
    {
        CheckNumberInput();
    }

    void CheckNumberInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) SelectSlot(ItemSlot.Primary, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) SelectSlot(ItemSlot.Secondary, 1);
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) SelectSlot(ItemSlot.Melee, 2);
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) SelectSlot(ItemSlot.HealthPotion, 3);
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) SelectSlot(ItemSlot.ShieldPotion, 4);
    }

    void SelectSlot(ItemSlot slot, int slotIndex)
    {
        currentSelectedSlot = slot;
        Debug.Log($"เปลี่ยนมาเลือกช่อง: {slot}");

        // 1. สั่ง UI ทำไฮไลต์
        if (inventoryUI != null)
        {
            inventoryUI.HighlightSlot(slotIndex);
            
            // ---> เพิ่มบรรทัดนี้: สั่งให้ UI อัปเดตการแสดงผลรูปไอเทมทั้งหมดใหม่ทุกครั้งที่เปลี่ยนช่อง <---
            inventoryUI.UpdateAllSlotsItemDisplay();
        }

        // 2. ดึงข้อมูล SO ของช่องนั้นๆ ออกมา
        InventoryEquipment targetEquipment = GetEquipmentBySlot(slot);

        // 3. สั่งอัปเดตโมเดลที่มือตาม SO นั้น
        SpawnModelToHand(targetEquipment);
    }

    // ฟังก์ชันช่วยหา SO ตามช่องที่เลือก
    InventoryEquipment GetEquipmentBySlot(ItemSlot slot)
    {
        switch (slot)
        {
            case ItemSlot.Primary: return primaryEquipment;
            case ItemSlot.Secondary: return secondaryEquipment;
            case ItemSlot.Melee: return meleeEquipment;
            case ItemSlot.HealthPotion: return healthPotionEquipment;
            case ItemSlot.ShieldPotion: return shieldPotionEquipment;
            default: return null;
        }
    }

    // ฟังก์ชันจัดการสร้างโมเดลไปติดที่ Hand
    void SpawnModelToHand(InventoryEquipment equipment)
    {
        // ลบโมเดลเก่าทิ้งก่อนทุกครั้งที่เปลี่ยนช่อง
        if (currentSpawnedModel != null)
        {
            Destroy(currentSpawnedModel);
        }
    
        // เช็กว่าช่องนั้นมี SO อยู่จริงไหม และมี Mesh หรือไม่
        if (equipment != null && equipment.itemMesh != null && handTransform != null)
        {
            // สร้าง GameObject เปล่าขึ้นมาใหม่เพื่อให้เป็นลูก (Child) ของ handTransform ทันที
            currentSpawnedModel = new GameObject("EquippedItem_Mesh");
            currentSpawnedModel.transform.SetParent(handTransform);
            
            // รีเซ็ตตำแหน่ง มุมหมุน และสเกลให้อยู่พอดีกับมือ
            currentSpawnedModel.transform.localPosition = Vector3.zero;
            currentSpawnedModel.transform.localRotation = Quaternion.identity;
            currentSpawnedModel.transform.localScale = Vector3.one;
    
            // เพิ่ม MeshFilter เพื่อใส่รูปทรง 3D
            MeshFilter mf = currentSpawnedModel.AddComponent<MeshFilter>();
            mf.mesh = equipment.itemMesh;
    
            // เพิ่ม MeshRenderer เพื่อใส่ Material ลงสี
            MeshRenderer mr = currentSpawnedModel.AddComponent<MeshRenderer>();
            if (equipment.itemMaterial != null)
            {
                mr.material = equipment.itemMaterial;
            }
        }
    }
}