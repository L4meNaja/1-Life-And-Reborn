using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats playerStats;

    public float currentHP = 100;
    public float maxHP = 100;
    public float spd = 10;
    public float str = 10;
    public float dur = 10;
    public float maxShield;
    public float currentShield;

    [Header("UI Effects")]
    public HurtOverlay hurtOverlay;

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

    void RandomStat()
    {
        maxHP = Mathf.Round((100 * Random.Range(0.5f, 1.5f)) * 10f) / 10f;
        currentHP = maxHP;

        spd = Mathf.Round((10 * Random.Range(0.5f, 1.5f)) * 10f) / 10f;
        str = Mathf.Round((10 * Random.Range(0.5f, 1.5f)) * 10f) / 10f;
        dur = Mathf.Round((10 * Random.Range(0.5f, 1.5f)) * 10f) / 10f;

        Debug.Log($"สเตตัสหลังสุ่ม -> HP: {maxHP}, SPD: {spd}, STR: {str}, DUR: {dur}");
    }

    void Start()
    {
        RandomStat();
    }

    void Update()
    {
        if (currentShield > maxShield) currentShield = maxShield;
    }

    public void TakeDamage(float rawDamage) 
    {
        float finalDamage = rawDamage * (10f / dur);
    
        if (dur <= 0) finalDamage = rawDamage; 
    
        bool isShieldDamaged = false;
    
        // ถ้าโล่มีมากกว่า 
        if (currentShield > finalDamage) {
            currentShield -= finalDamage;
            isShieldDamaged = true;
        }
        // ถ้าโล่มีเท่ากัน 
        else if (currentShield == finalDamage) {
            currentShield -= finalDamage;
            isShieldDamaged = true;
        }
        // ถ้าโล่มีน้อยกว่า
        else if (currentShield < finalDamage && currentShield > 0) {
            currentShield -= finalDamage;
            if (currentShield < 0) {
                currentHP += currentShield;
                currentShield = 0;
            }
            isShieldDamaged = true;
        }
        // ถ้าไม่มีโล่ 
        else {
            currentHP -= finalDamage;
            isShieldDamaged = false;
        }
    
        if (hurtOverlay != null)
        {
            hurtOverlay.ShowHurtEffect(isShieldDamaged);
        }
    
        if (currentHP < 0) currentHP = 0;
    }
}