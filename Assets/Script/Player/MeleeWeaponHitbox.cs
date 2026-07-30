using UnityEngine;
using System.Collections.Generic;

public class MeleeWeaponHitbox : MonoBehaviour
{
    public float damage = 0;
    public bool isAttacking = false;
    
    // เก็บรายชื่อศัตรูที่โดนตีไปแล้วในการฟัน 1 ครั้ง (จะได้ไม่เกิดบั๊กฟันครั้งเดียวโดนซ้ำหลายรอบ)
    private HashSet<Collider> hitEnemies = new HashSet<Collider>();

    public void StartAttack(float attackDamage)
    {
        damage = attackDamage;
        isAttacking = true;
        hitEnemies.Clear(); // ล้างข้อมูลเก่า
    }

    public void EndAttack()
    {
        isAttacking = false;
        hitEnemies.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        // ถ้าไม่ได้อยู่ในช่วงกำลังฟัน จะไม่เกิดดาเมจ (เช่น แค่เดินถือดาบไปชนศัตรู)
        if (!isAttacking) return;

        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null && !hitEnemies.Contains(other))
        {
            hitEnemies.Add(other);
            enemy.TakeDamage(damage);
            Debug.Log($"อาวุธฟาดโดน {enemy.name} ตรงๆ! Damage: {damage}");
        }
    }
}
