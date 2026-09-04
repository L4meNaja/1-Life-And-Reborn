using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats playerStats;

    [Header("Spawn Settings")]
    public Vector3 spawnPosition = Vector3.zero; // ตั้งค่า X, Y, Z จุดเกิดใน Inspector ได้ที่นี่

    [Header("Base Stats")]
    public float currentHP = 100;
    public float maxHP = 100;
    public float spd = 10;
    public float str = 10;
    public float dur = 10;
    public float maxShield;
    public float currentShield;

    [Header("Inventory & Equipment")]
    public int healItemCount;       // จำนวนยาที่ได้จากการสุ่ม (1-2)
    public bool hasPistol;          // มี Pistol หรือไม่
    public bool hasRifle;           // มี Rifle หรือไม่
    public bool hasShotgun;         // มี Shotgun หรือไม่

    [Header("UI & Systems")]
    public HurtOverlay hurtOverlay;
    public DeathPanel deathPanel;   // ลาก DeathPanel มาใส่ใน Inspector

    private bool isDead = false;

    void Awake()
    {
        if (playerStats == null)
        {
            playerStats = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        RandomStat();
    }

    void Update()
    {
        if (currentShield > maxShield) currentShield = maxShield;
    }

    public void RandomStat()
    {
        isDead = false;

        // 0. รีเซ็ตตำแหน่งผู้เล่นกลับไปจุดเกิดที่ตั้งไว้
        ResetToSpawnPosition();

        // 1. สุ่ม Stats
        maxHP = Mathf.Round((100 * Random.Range(0.5f, 1.5f)) * 10f) / 10f;
        currentHP = maxHP;

        spd = Mathf.Round((10 * Random.Range(0.5f, 1.5f)) * 10f) / 10f;
        str = Mathf.Round((10 * Random.Range(0.5f, 1.5f)) * 10f) / 10f;
        dur = Mathf.Round((10 * Random.Range(0.5f, 1.5f)) * 10f) / 10f;

        // 2. สุ่มจำนวนยา (1 ถึง 2 ชิ้น)
        healItemCount = Random.Range(1, 3);

        // 3. สุ่มอาวุธ (การันตีอย่างน้อย 1 ชิ้น)
        hasPistol = Random.value > 0.5f;
        hasRifle = Random.value > 0.5f;
        hasShotgun = Random.value > 0.5f;

        if (!hasPistol && !hasRifle && !hasShotgun)
        {
            int guaranteedWeapon = Random.Range(0, 3);
            if (guaranteedWeapon == 0) hasPistol = true;
            else if (guaranteedWeapon == 1) hasRifle = true;
            else hasShotgun = true;
        }

        // 4. ส่งข้อมูลอาวุธและยาไปอัปเดตที่ PlayerInventory และ UI
        PlayerInventory inventory = GetComponent<PlayerInventory>();
        if (inventory == null) inventory = FindFirstObjectByType<PlayerInventory>();

        if (inventory != null)
        {
            inventory.ApplyRandomizedInventory(hasPistol, hasRifle, hasShotgun, healItemCount);
        }
    }

    // ฟังก์ชันย้ายตำแหน่งผู้เล่นกลับจุดเกิด
    public void ResetToSpawnPosition()
    {
        // หากมี CharacterController ต้องย้ายตำแหน่งผ่าน CharacterController เพื่อไม่ให้ติด Physics
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            transform.position = spawnPosition;
            cc.enabled = true;
        }
        else
        {
            transform.position = spawnPosition;
        }

        // หากมี Rigidbody ให้ล้างค่าความเร็วตกค้างด้วย
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // ฟังก์ชันสำหรับเรียกสั่งอัปเดต UI ทั้งหมด
    public void UpdatePlayerUI()
    {
    }

    public void TakeDamage(float rawDamage) 
    {
        if (isDead) return;

        float finalDamage = (dur > 0) ? rawDamage * (10f / dur) : rawDamage;
        bool isShieldDamaged = false;

        // คำนวณความเสียหายลง Shield และ HP
        if (currentShield > 0)
        {
            isShieldDamaged = true;
            if (currentShield >= finalDamage)
            {
                currentShield -= finalDamage;
            }
            else
            {
                float leftoverDamage = finalDamage - currentShield;
                currentShield = 0;
                currentHP -= leftoverDamage;
            }
        }
        else
        {
            currentHP -= finalDamage;
            isShieldDamaged = false;
        }

        if (hurtOverlay != null)
        {
            hurtOverlay.ShowHurtEffect(isShieldDamaged);
        }

        // ตรวจสอบการตาย
        if (currentHP <= 0)
        {
            currentHP = 0;
            isDead = true;

            if (deathPanel != null)
            {
                deathPanel.PlayDeathSequence();
            }
        }
    }
}