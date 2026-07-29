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
        CheckUseItem();
    }

    void CheckUseItem()
    {
        // ใช้ไอเทมเมื่อกดคลิกซ้าย (หรือเปลี่ยนเป็นปุ่มอื่นได้ตามต้องการ)
        if (Input.GetMouseButtonDown(0))
        {
            UseCurrentItem();
        }
    }

    void UseCurrentItem()
    {
        // ใช้ได้เฉพาะไอเทมที่เป็น Consumable (ช่อง 4 HealthPotion, ช่อง 5 ShieldPotion)
        if (currentSelectedSlot != ItemSlot.HealthPotion && currentSelectedSlot != ItemSlot.ShieldPotion) return;

        InventoryEquipment currentEq = GetEquipmentBySlot(currentSelectedSlot);
        
        // ตรวจสอบว่ามีไอเทมถืออยู่ไหม และจำนวนมากกว่า 0 ไหม
        if (currentEq == null || currentEq.count <= 0) return;

        bool isUsed = false;

        // เช็คการฮีลเลือด
        if (currentEq.healValue > 0)
        {
            PlayerStats.playerStats.currentHP += currentEq.healValue;
            if (PlayerStats.playerStats.currentHP > PlayerStats.playerStats.maxHP)
                PlayerStats.playerStats.currentHP = PlayerStats.playerStats.maxHP;
            isUsed = true;
        }

        // เช็คการเพิ่มเกราะ
        if (currentEq.shieldValue > 0)
        {
            PlayerStats.playerStats.currentShield += currentEq.shieldValue;
            if (PlayerStats.playerStats.currentShield > PlayerStats.playerStats.maxShield)
                PlayerStats.playerStats.currentShield = PlayerStats.playerStats.maxShield;
            isUsed = true;
        }

        // ถ้าใช้ไอเทมสำเร็จ (มี heal หรือ shield อย่างใดอย่างหนึ่ง)
        if (isUsed)
        {
            currentEq.count--;
            Debug.Log($"ใช้ไอเทม {currentEq.name} ไปแล้ว! เหลือ: {currentEq.count}");

            // ถ้าใช้หมดแล้ว ลบออกจากช่องไปเลย
            if (currentEq.count <= 0)
            {
                RemoveEquipmentFromSlot(currentSelectedSlot);
            }

            // อัปเดต UI 
            if (inventoryUI != null)
            {
                inventoryUI.UpdateAllSlotsItemDisplay();
            }
        }
    }

    void RemoveEquipmentFromSlot(ItemSlot slot)
    {
        // ล้างข้อมูลใน SO ช่องนั้น
        switch (slot)
        {
            case ItemSlot.Primary: primaryEquipment = null; break;
            case ItemSlot.Secondary: secondaryEquipment = null; break;
            case ItemSlot.Melee: meleeEquipment = null; break;
            case ItemSlot.HealthPotion: healthPotionEquipment = null; break;
            case ItemSlot.ShieldPotion: shieldPotionEquipment = null; break;
        }

        // ถ้ากำลังถือไอเทมนี้อยู่ ให้ลบโมเดลทิ้งด้วย
        if (currentSelectedSlot == slot && currentSpawnedModel != null)
        {
            Destroy(currentSpawnedModel);
            currentSpawnedModel = null;
        }
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
            
            // ตั้งค่าตำแหน่ง มุมหมุน และสเกลจาก ScriptableObject
            currentSpawnedModel.transform.localPosition = equipment.itemPos;
            currentSpawnedModel.transform.localRotation = Quaternion.Euler(equipment.itemRot);
            
            // ใช้ itemSize ถ้ามีการตั้งค่าไว้ (ป้องกันกรณีค่าเป็น 0,0,0 แล้วโมเดลล่องหน)
            if (equipment.itemSize != Vector3.zero)
                currentSpawnedModel.transform.localScale = equipment.itemSize;
            else
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