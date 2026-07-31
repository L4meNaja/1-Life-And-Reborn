using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public float speed = 50f;
    public float lifetime = 3f;

    private Vector3 previousPosition;

    void Start()
    {
        // ทำลายตัวเองเมื่อหมดอายุ (กันขยะในซีน)
        Destroy(gameObject, lifetime);
        previousPosition = transform.position;
    }

    void Update()
    {
        // คำนวณระยะทางที่จะพุ่งไปในเฟรมนี้
        float distanceThisFrame = speed * Time.deltaTime;

        // ยิง Raycast จากตำแหน่งเดิมไปยังตำแหน่งใหม่ เพื่อเช็คการชน
        if (Physics.Raycast(previousPosition, transform.forward, out RaycastHit hit, distanceThisFrame))
        {
            if (!hit.collider.CompareTag("Player") && !hit.collider.isTrigger)
            {
                Debug.Log($"กระสุนชนกับ: {hit.collider.gameObject.name} (Tag: {hit.collider.tag})");

                // ถ้าโดนศัตรู ให้ทำดาเมจ
                EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Debug.Log($"กระสุนทำดาเมจใส่ {enemy.name}! ({damage})");
                }

                // สั่งทำลายกระสุนเมื่อชน
                Destroy(gameObject);
                return; // หยุดการเคลื่อนที่
            }
        }

        // ขยับตำแหน่งไปข้างหน้า
        transform.position += transform.forward * distanceThisFrame;
        previousPosition = transform.position;
    }
}
