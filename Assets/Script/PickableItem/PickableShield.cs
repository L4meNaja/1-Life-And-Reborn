using UnityEngine;
using TMPro;

public class PickableShield : MonoBehaviour
{
    [Header("Shield Settings")]
    [Tooltip("ใส่ค่าเกราะของชิ้นนี้ตรงๆ")]
    public float shieldAmount = 25f;    

    [Header("UI Reference")]
    public GameObject interactUI;          // UI กด E
    public GameObject textCanvasObject;    // Canvas ที่จะเปิด/ปิดตามระยะผู้เล่น
    public TextMeshProUGUI myAmountText;   // ลาก TextMeshProUGUI ของชิ้นนี้มาใส่

    private bool playerIsClose = false;
    private PlayerStats playerStats;

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (textCanvasObject != null) textCanvasObject.SetActive(false);
        // * ตัดการรัน Start / OnValidate / Update ทิ้งไป ไม่ให้มันฝืน Set ข้อความตอนเริ่มเกม *
    }

    void Update()
    {
        if (playerIsClose && Input.GetKeyDown(KeyCode.E))
        {
            PickUpShield();
        }
    }

    // ฟังก์ชันอัปเดตข้อความ จะถูกเรียกก็ต่อเมื่อเดินเข้าใกล้เท่านั้น
    void RefreshText()
    {
        if (myAmountText != null)
        {
            myAmountText.text = $"( {shieldAmount} )";
        }
    }

    void PickUpShield()
    {
        if (playerStats != null)
        {
            // ซ่อน UI และ Text ทันทีที่กดเก็บ
            if (interactUI != null) interactUI.SetActive(false);
            if (textCanvasObject != null) textCanvasObject.SetActive(false);

            playerStats.maxShield = shieldAmount;
            playerStats.currentShield = playerStats.maxShield;

            Debug.Log($"เก็บเกราะชิ้นนี้สำเร็จ! ตั้งค่า Shield เป็น: {shieldAmount}");

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerStats = collision.GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                playerStats = collision.GetComponentInChildren<PlayerStats>();
            }

            if (playerStats != null)
            {
                playerIsClose = true;
                
                // อัปเดตตัวเลขตามค่า shieldAmount ของชิ้นนี้ ณ ตอนที่เดินเข้าใกล้พอดี
                RefreshText();

                if (interactUI != null) interactUI.SetActive(true);
                if (textCanvasObject != null) textCanvasObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            playerStats = null;
            if (interactUI != null) interactUI.SetActive(false);
            if (textCanvasObject != null) textCanvasObject.SetActive(false);
        }
    }
}