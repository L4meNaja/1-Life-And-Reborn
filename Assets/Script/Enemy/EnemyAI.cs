using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float maxHP = 50f;
    public float currentHP;
    public float attackDamage = 10f;

    [Header("Detection")]
    public float detectionRange = 10f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float stoppingDistance = 2f;

    [Header("Attack")]
    public float attackCooldown = 1.5f;
    public float attackRange = 2.5f;

    private float lastAttackTime = -999f;
    private Transform playerTransform;
    private CharacterController characterController;

    void Start()
    {
        currentHP = maxHP;

        characterController = GetComponent<CharacterController>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            Debug.Log("Enemy เจอ Player แล้ว!");
        }
        else
        {
            Debug.LogError("Enemy หา Player ไม่เจอ! เช็ก Tag Player");
        }
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        if (currentHP <= 0)
            return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, playerTransform.position);

        // ==============================
        // อยู่นอกระยะ Detection
        // ==============================

        if (distanceToPlayer > detectionRange)
        {
            return;
        }

        // ==============================
        // อยู่ใน Detection Range
        // ==============================

        if (distanceToPlayer > stoppingDistance)
        {
            Vector3 direction =
                playerTransform.position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                direction.Normalize();

                characterController.Move(
                    direction * moveSpeed * Time.deltaTime
                );
            }
        }

        // ==============================
        // Attack
        // ==============================

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
        Debug.Log(
            $"{gameObject.name} โจมตี Player! Damage: {attackDamage}"
        );

        if (PlayerStats.playerStats != null)
        {
            PlayerStats.playerStats.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        Debug.Log(
            $"{gameObject.name} โดนตี! HP เหลือ: {currentHP}/{maxHP}"
        );

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

    // แสดง Detection Range ใน Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}