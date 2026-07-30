using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP = 50f;
    public float currentHP;
    public float attackDamage = 10f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float stoppingDistance = 2f; // ระยะที่หยุดเดินแล้วเริ่มตี

    [Header("Attack")]
    public float attackCooldown = 1.5f; // คูลดาวน์ระหว่างการโจมตี (วินาที)
    public float attackRange = 2.5f;    // ระยะโจมตี
    private float lastAttackTime = -999f;

    private Transform playerTransform;

    void Start()
    {
        currentHP = maxHP;

        // ค้นหา Player จาก Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("ไม่เจอ GameObject ที่มี Tag 'Player'!");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;
        if (currentHP <= 0) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // เดินเข้าหา Player
        if (distanceToPlayer > stoppingDistance)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0; // ไม่ให้บินขึ้น/ลง
            transform.position += direction * moveSpeed * Time.deltaTime;

            // หันหน้าไปหา Player
            transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        }

        // โจมตีถ้าอยู่ในระยะ
        if (distanceToPlayer <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
    }

    void AttackPlayer()
    {
        Debug.Log($"{gameObject.name} โจมตี Player! Damage: {attackDamage}");
        
        if (PlayerStats.playerStats != null)
        {
            PlayerStats.playerStats.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} โดนตี! HP เหลือ: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} ตายแล้ว!");
        Destroy(gameObject);
    }
}
