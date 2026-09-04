using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Weapons")]
    public bool hasPistol;
    public bool hasRifle;  
    public bool hasShotgun;

    [Header("UI Reference")]
    public InventoryUI inventoryUI;
    public ReloadTimerUI reloadTimerUI; // ลากสคริปต์ ReloadTimerUI มาใส่ตรงนี้
    public CameraController cameraController;

    [Header("Hand Transform (จุดอ้างอิงที่มือ)")]
    public Transform handTransform; // ลาก Transform เปล่าที่มือมาใส่ตรงนี้
    public Transform armTransform;  // ลากโมเดลมือ/แขน ของจริงมาใส่ตรงนี้ เพื่อให้มันเหวี่ยงทั้งแขน

    private GameObject currentSpawnedModel; // เก็บโมเดลที่กำลังถืออยู่ปัจจุบัน

    [Header("Equipped Items (Scriptable Objects)")]
    public InventoryEquipment primaryEquipment;
    public InventoryEquipment secondaryEquipment;
    public InventoryEquipment meleeEquipment;
    public InventoryEquipment healthPotionEquipment;
    public InventoryEquipment shieldPotionEquipment;

    [Header("Consumable Counts")]
    public int healthPotionCount = 0;
    public int shieldPotionCount = 0;

    [Header("Ammo Status")]
    public int primaryAmmoCount = 0;
    public int primaryInvAmmo = 0;
    public int secondaryAmmoCount = 0;
    public int secondaryInvAmmo = 0;

    [Header("Shoot Settings")]
    private float lastShootTime = -999f;
    private Coroutine currentSwingCoroutine;
    private bool isReloading = false;
    private Coroutine currentReloadCoroutine;

    [Header("Active Slot")]
    public ItemSlot currentSelectedSlot = ItemSlot.Primary;

    void Start()
    {
        // เปลี่ยนค่าหลอกๆ ไว้ก่อน เพื่อบังคับให้ฟังก์ชัน SelectSlot(Primary) ทำงานตั้งแต่เริ่มเกม
        currentSelectedSlot = (ItemSlot)999; 
        SelectSlot(ItemSlot.Primary, 0);

        if (reloadTimerUI != null)
        {
            reloadTimerUI.ShowReloadUI(false); // ซ่อนหลอดรีโหลดตั้งแต่เริ่ม
        }
    }

    void Update()
    {
        CheckNumberInput();
        CheckUseItem();
        CheckReload();
    }

    public void ApplyRandomizedInventory(bool pistol, bool rifle, bool shotgun, int potions)
    {
        // กำหนดค่าสิทธิ์การครอบครองปืน
        hasPistol = pistol;
        hasRifle = rifle;
        hasShotgun = shotgun;
    
        // กำหนดจำนวนยา (อัปเดตใส่ healthPotionCount)
        healthPotionCount = potions; 
    
        // สั่งอัปเดต UI ให้ตรงกับไอเทมที่มี
        if (inventoryUI != null)
        {
            inventoryUI.UpdateAllSlotsItemDisplay();
        }
    }

    void CheckReload()
    {
        if (isReloading) return; // กำลังรีโหลดอยู่ กดซ้ำไม่ได้

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (reloadTimerUI == null)
            {
                Debug.LogWarning("ลืมลากสคริปต์ ReloadTimerUI ไปใส่ใน PlayerInventory หรือเปล่า!?");
            }

            if (currentSelectedSlot == ItemSlot.Primary || currentSelectedSlot == ItemSlot.Secondary)
            {
                currentReloadCoroutine = StartCoroutine(ReloadWeaponCoroutine());
            }
        }
    }

    void CheckUseItem()
    {
        if (isReloading) return; // ถ้ารีโหลดอยู่ ห้ามยิงหรือใช้ไอเทม
        // ยิง Primary แบบออโต้ (กดค้างได้)
        if (currentSelectedSlot == ItemSlot.Primary)
        {
            if (Input.GetMouseButton(0))
            {
                ShootWeapon();
            }
        }
        // ยิง Secondary หรือฟัน มีด/ยา (กดเป็นคลิกๆ)
        else if (Input.GetMouseButtonDown(0))
        {
            if (currentSelectedSlot == ItemSlot.Secondary)
            {
                ShootWeapon();
            }
            else if (currentSelectedSlot == ItemSlot.HealthPotion || currentSelectedSlot == ItemSlot.ShieldPotion)
            {
                UseCurrentItem();
            }
        }
    }

    void ShootWeapon()
    {
        InventoryEquipment gunEq = GetEquipmentBySlot(currentSelectedSlot);
        if (gunEq == null) return;

        // เช็ค Fire Rate
        float fireRate = gunEq.attackSpeed > 0 ? 1f / gunEq.attackSpeed : 0.2f;
        if (Time.time - lastShootTime < fireRate) return;

        // เช็คกระสุน
        ref int currentAmmo = ref GetAmmoCountRef(currentSelectedSlot);
        if (currentAmmo <= 0)
        {
            if (Input.GetMouseButtonDown(0)) Debug.Log("กระสุนหมด! ต้องรีโหลด (กด R)");
            return;
        }

        lastShootTime = Time.time;

        // ยิงปืน: ลดกระสุน
        currentAmmo--;
        Debug.Log($"ยิงปืน! กระสุนเหลือ: {currentAmmo}/{gunEq.maxAmmo}");

        // อัปเดต UI 
        if (inventoryUI != null) inventoryUI.UpdateAllSlotsItemDisplay();

        // เรียก Recoil โดยใช้ค่า camRecoil จาก SO ตัวเดียว
        if (CameraController.Instance != null)
        {
            Vector3 randomRecoil = Random.insideUnitSphere * gunEq.camRecoil;
            // Y ใช้ Mathf.Abs เพื่อให้กล้องสะบัดขึ้นเสมอ, Z ใช้ Mathf.Abs เพื่อให้ถอยหลัง
            CameraController.Instance.ShoulderOffset(randomRecoil.x, Mathf.Abs(randomRecoil.y), Mathf.Abs(randomRecoil.z));
        }

        // หา Camera
        Camera cam = Camera.main;
        if (cam == null) cam = GetComponentInChildren<Camera>();
        if (cam == null) cam = Object.FindFirstObjectByType<Camera>();
        if (cam == null) return;

        // 1. กำหนดจุดเกิดกระสุน
        Vector3 spawnPos = handTransform.position;
        if (currentSpawnedModel != null)
        {
            spawnPos = currentSpawnedModel.transform.TransformPoint(gunEq.ammoSpawnPos);
        }

        int count = gunEq.bulletCount > 0 ? gunEq.bulletCount : 1;

        // 2. ลูปสร้างกระสุน
        for (int i = 0; i < count; i++)
        {
            GameObject bulletObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulletObj.transform.position = spawnPos;

            Quaternion baseRotation = transform.rotation;

            float spreadAmount = gunEq.bulletSpread;
            float randomYaw = Random.Range(-spreadAmount, spreadAmount);
            float randomPitch = Random.Range(-spreadAmount, spreadAmount);

            Quaternion spreadRotation = Quaternion.Euler(randomPitch, randomYaw, 0f);
            bulletObj.transform.rotation = baseRotation * spreadRotation;

            bulletObj.transform.localScale = Vector3.zero;

            Destroy(bulletObj.GetComponent<Collider>());

            MeshRenderer mr = bulletObj.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                mr.material.color = Color.yellow;
            }

            TrailRenderer tr = bulletObj.AddComponent<TrailRenderer>();
            tr.time = 0.1f;
            tr.startWidth = 0.1f;
            tr.endWidth = 0f;
            tr.material = new Material(Shader.Find("Sprites/Default"));
            tr.startColor = Color.yellow;
            tr.endColor = new Color(1, 1, 0, 0);

            Bullet bulletScript = bulletObj.AddComponent<Bullet>();
            bulletScript.damage = gunEq.damageValue;
            bulletScript.speed = 100f;
        }
    }

    System.Collections.IEnumerator ReloadWeaponCoroutine()
    {
        InventoryEquipment gunEq = GetEquipmentBySlot(currentSelectedSlot);
        if (gunEq == null) yield break;

        // ดึงค่ามาตรวจสอบก่อน (แบบไม่ใช้ ref เพราะ C# ไม่อนุญาตใน Coroutine)
        int currentAmmo = currentSelectedSlot == ItemSlot.Primary ? primaryAmmoCount : secondaryAmmoCount;
        int invAmmo = currentSelectedSlot == ItemSlot.Primary ? primaryInvAmmo : secondaryInvAmmo;

        if (currentAmmo >= gunEq.maxAmmo)
        {
            Debug.Log("กระสุนเต็มอยู่แล้ว!");
            yield break;
        }

        if (invAmmo <= 0)
        {
            Debug.Log("ไม่มีกระสุนสำรอง!");
            yield break;
        }

        isReloading = true;
        
        // เวลาในการรีโหลดจาก SO (ถ้าไม่ได้ตั้งไว้ให้เป็น 2 วินาที)
        float reloadTime = gunEq.reloadTime > 0 ? gunEq.reloadTime : 2f;
        float t = 0f;

        if (reloadTimerUI != null) reloadTimerUI.ShowReloadUI(true, reloadTime);

        while (t < reloadTime)
        {
            t += Time.deltaTime;
            
            // ส่งค่าสัดส่วน (1 ไป 0) และเวลาที่เหลือไปให้ UI
            float progress = 1f - (t / reloadTime);
            float remainingTime = reloadTime - t;
            if (reloadTimerUI != null) reloadTimerUI.UpdateReloadProgress(progress, remainingTime);

            yield return null;
        }

        if (reloadTimerUI != null) reloadTimerUI.ShowReloadUI(false);

        // ดึงค่ามาคำนวณอีกรอบหลังเวลารีโหลดเสร็จ
        currentAmmo = currentSelectedSlot == ItemSlot.Primary ? primaryAmmoCount : secondaryAmmoCount;
        invAmmo = currentSelectedSlot == ItemSlot.Primary ? primaryInvAmmo : secondaryInvAmmo;

        int neededAmmo = gunEq.maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(neededAmmo, invAmmo);

        // อัปเดตกลับเข้าตัวแปรหลักโดยตรง
        if (currentSelectedSlot == ItemSlot.Primary)
        {
            primaryAmmoCount += ammoToReload;
            primaryInvAmmo -= ammoToReload;
            currentAmmo = primaryAmmoCount;
            invAmmo = primaryInvAmmo;
        }
        else if (currentSelectedSlot == ItemSlot.Secondary)
        {
            secondaryAmmoCount += ammoToReload;
            secondaryInvAmmo -= ammoToReload;
            currentAmmo = secondaryAmmoCount;
            invAmmo = secondaryInvAmmo;
        }

        Debug.Log($"รีโหลดเสร็จสิ้น! กระสุน: {currentAmmo}/{gunEq.maxAmmo} | สำรอง: {invAmmo}");

        if (inventoryUI != null) inventoryUI.UpdateAllSlotsItemDisplay();

        isReloading = false;
    }

    // ฟังก์ชันคืนค่า Reference ของกระสุน เพื่อให้สามารถแก้ไขตัวแปรต้นทางได้โดยตรง
    ref int GetAmmoCountRef(ItemSlot slot)
    {
        if (slot == ItemSlot.Primary) return ref primaryAmmoCount;
        return ref secondaryAmmoCount;
    }

    ref int GetInvAmmoRef(ItemSlot slot)
    {
        if (slot == ItemSlot.Primary) return ref primaryInvAmmo;
        return ref secondaryInvAmmo;
    }

    void UseCurrentItem()
    {
        InventoryEquipment currentEq = GetEquipmentBySlot(currentSelectedSlot);
        
        // ตรวจสอบว่ามีไอเทมถืออยู่ไหม
        if (currentEq == null) return;

        // ดึงจำนวนจาก PlayerInventory (ไม่ใช่จาก SO อีกต่อไป)
        int currentCount = GetCountBySlot(currentSelectedSlot);
        if (currentCount <= 0) return;

        bool isUsed = false;

        // เช็คการฮีลเลือด
        if (currentEq.healValue > 0)
        {
            // ถ้าเลือดเต็มอยู่แล้ว ให้ตัดจบไม่ใช้ยา
            if (PlayerStats.playerStats.currentHP >= PlayerStats.playerStats.maxHP)
            {
                Debug.Log("เลือดเต็มอยู่แล้ว ไม่สามารถใช้โพชันเลือดได้!");
                return;
            }

            PlayerStats.playerStats.currentHP += currentEq.healValue;
            if (PlayerStats.playerStats.currentHP > PlayerStats.playerStats.maxHP)
                PlayerStats.playerStats.currentHP = PlayerStats.playerStats.maxHP;
            isUsed = true;
        }

        // เช็คการเพิ่มเกราะ
        if (currentEq.shieldValue > 0)
        {
            // ถ้าเกราะเต็มอยู่แล้ว ให้ตัดจบไม่ใช้ยา
            if (PlayerStats.playerStats.currentShield >= PlayerStats.playerStats.maxShield)
            {
                Debug.Log("เกราะเต็มอยู่แล้ว ไม่สามารถใช้โพชันเกราะได้!");
                return;
            }

            PlayerStats.playerStats.currentShield += currentEq.shieldValue;
            if (PlayerStats.playerStats.currentShield > PlayerStats.playerStats.maxShield)
                PlayerStats.playerStats.currentShield = PlayerStats.playerStats.maxShield;
            isUsed = true;
        }

        // ถ้าใช้ไอเทมสำเร็จ
        if (isUsed)
        {
            // ลดจำนวนใน PlayerInventory
            SetCountBySlot(currentSelectedSlot, currentCount - 1);
            Debug.Log($"ใช้ไอเทม {currentEq.name} ไปแล้ว! เหลือ: {GetCountBySlot(currentSelectedSlot)}");

            // ถ้าใช้หมดแล้ว ลบออกจากช่องไปเลย
            if (GetCountBySlot(currentSelectedSlot) <= 0)
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

    // ดึงจำนวนตามช่อง (ยา)
    public int GetCountBySlot(ItemSlot slot)
    {
        switch (slot)
        {
            case ItemSlot.HealthPotion: return healthPotionCount;
            case ItemSlot.ShieldPotion: return shieldPotionCount;
            default: return -1; // อาวุธไม่มี count
        }
    }

    // ตั้งค่าจำนวนตามช่อง
    void SetCountBySlot(ItemSlot slot, int value)
    {
        switch (slot)
        {
            case ItemSlot.HealthPotion: healthPotionCount = value; break;
            case ItemSlot.ShieldPotion: shieldPotionCount = value; break;
        }
    }

    void RemoveEquipmentFromSlot(ItemSlot slot)
    {
        switch (slot)
        {
            case ItemSlot.Primary: primaryEquipment = null; break;
            case ItemSlot.Secondary: secondaryEquipment = null; break;
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
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) SelectSlot(ItemSlot.HealthPotion, 2);
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) SelectSlot(ItemSlot.ShieldPotion, 3);
    }

    void SelectSlot(ItemSlot slot, int slotIndex)
    {
        // ถ้ารีโหลดอยู่แล้วกดสลับปืน ให้ยกรเลิกการรีโหลดทันที
        if (isReloading && currentReloadCoroutine != null)
        {
            StopCoroutine(currentReloadCoroutine);
            isReloading = false;
            if (reloadTimerUI != null) reloadTimerUI.ShowReloadUI(false);
        }

        if (currentSelectedSlot == slot) return;
        
        currentSelectedSlot = slot; // <--- แก้บัค: สั่งให้ระบบจำว่าเราเปลี่ยนมาถือปืนนี้แล้วจริงๆ
        Debug.Log($"เปลี่ยนมาเลือกช่อง: {slot}");

        // 1. สั่ง UI ทำไฮไลต์ และอัปเดตหน้ากระสุน
        if (inventoryUI != null)
        {
            inventoryUI.HighlightSlot(slotIndex);
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
            case ItemSlot.HealthPotion: return healthPotionEquipment;
            case ItemSlot.ShieldPotion: return shieldPotionEquipment;
            default: return null;
        }
    }

    // ฟังก์ชันจัดการสร้างโมเดลไปติดที่ Hand
    void SpawnModelToHand(InventoryEquipment equipment)
    {
        if (currentSwingCoroutine != null)
        {
            StopCoroutine(currentSwingCoroutine);
            currentSwingCoroutine = null;
        }

        // ลบโมเดลเก่าทิ้งก่อนทุกครั้งที่เปลี่ยนช่อง
        if (currentSpawnedModel != null)
        {
            Destroy(currentSpawnedModel);
            currentSpawnedModel = null;
        }

        // เช็กถ้าเป็นโพชัน (Health หรือ Shield) แต่จำนวนเป็น 0 หรือต่ำกว่า ให้ตัดจบไม่สร้างโมเดล
        if (currentSelectedSlot == ItemSlot.HealthPotion && healthPotionCount <= 0) return;
        if (currentSelectedSlot == ItemSlot.ShieldPotion && shieldPotionCount <= 0) return;
    
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