using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory playerInventory; // ลากตัว Player ที่มีสคริปต์ PlayerInventory มาใส่

    [System.Serializable]
    public struct SlotUI
    {
        public GameObject slotGameObject;       // ตัว GameObject หลักของช่องนั้น (เช่น Slot1)
        public Image slotImage;                 // ตัว Image ของกรอบช่องหลัก (ใช้ปรับ Alpha)
        public Graphic itemTextureGraphic;      // ตัวรูปไอเทมข้างใน (รองรับทั้ง Image และ RawImage)
        public RectTransform rectTransform;     // ใช้คุมขนาดและตำแหน่งช่องหลัก
        [HideInInspector] public Vector2 defaultAnchoredPosition;
        [HideInInspector] public Vector3 defaultScale;
    }

    [Header("UI Slots (1 to 5)")]
    public SlotUI[] slots = new SlotUI[5];

    [Header("Highlight Settings")]
    public float scaleMultiplier = 1.25f;       // ขยายใหญ่ขึ้น 1.25 เท่า
    public float positionOffset = 20f;          // ระยะที่จะให้ขยับขึ้นไป
    public float normalAlpha = 0.4f;            // ความโปร่งใสตอนไม่ได้เลือก
    public float selectedAlpha = 0.9f;          // ความโปร่งใสตอนเลือก

    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].slotGameObject != null)
            {
                slots[i].rectTransform = slots[i].slotGameObject.GetComponent<RectTransform>();
                
                if (slots[i].slotImage == null)
                {
                    slots[i].slotImage = slots[i].slotGameObject.GetComponent<Image>();
                }

                if (slots[i].itemTextureGraphic == null)
                {
                    Transform itemTexTrans = slots[i].slotGameObject.transform.Find("ItemTexture");
                    if (itemTexTrans != null)
                    {
                        slots[i].itemTextureGraphic = itemTexTrans.GetComponent<Graphic>();
                    }
                }

                slots[i].defaultAnchoredPosition = slots[i].rectTransform.anchoredPosition;
                slots[i].defaultScale = slots[i].rectTransform.localScale;
            }
        }

        if (playerInventory == null)
        {
            playerInventory = Object.FindFirstObjectByType<PlayerInventory>();
        }

        // เริ่มต้นเลือกช่อง 1 ไว้ก่อน
        HighlightSlot(0);
    }

    void Update()
    {
        UpdateAllSlotsItemDisplay();
    }

    // ฟังก์ชันดึงข้อมูล texture จาก PlayerInventory มาแสดงผลบน UI
    public void UpdateAllSlotsItemDisplay()
    {
        if (playerInventory == null) return;

        InventoryEquipment[] equipments = new InventoryEquipment[5]
        {
            playerInventory.primaryEquipment,
            playerInventory.secondaryEquipment,
            playerInventory.meleeEquipment,
            playerInventory.healthPotionEquipment,
            playerInventory.shieldPotionEquipment
        };

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].itemTextureGraphic == null) continue;

            InventoryEquipment eq = equipments[i];

            if (eq != null && eq.itemTexture != null)
            {
                slots[i].itemTextureGraphic.gameObject.SetActive(true);

                // ตั้งค่ารูปภาพตามประเภทของ Graphic (Image หรือ RawImage)
                if (slots[i].itemTextureGraphic is RawImage rawImg)
                {
                    rawImg.texture = eq.itemTexture;
                }
                else if (slots[i].itemTextureGraphic is Image img)
                {
                    img.sprite = Sprite.Create(eq.itemTexture, new Rect(0, 0, eq.itemTexture.width, eq.itemTexture.height), new Vector2(0.5f, 0.5f));
                }

                // === ปรับขนาด (SizeDelta) ตาม textureSize ใน ScriptableObject อัตโนมัติ ===
                RectTransform texRect = slots[i].itemTextureGraphic.rectTransform;
                texRect.sizeDelta = eq.textureSize;
            }
            else
            {
                slots[i].itemTextureGraphic.gameObject.SetActive(false);
            }
        }
    }

    public void HighlightSlot(int selectedIndex)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].slotGameObject == null) continue;

            if (i == selectedIndex)
            {
                // === ช่องที่ถูกเลือก ===
                slots[i].rectTransform.localScale = slots[i].defaultScale * scaleMultiplier;
                slots[i].rectTransform.anchoredPosition = slots[i].defaultAnchoredPosition + new Vector2(0, positionOffset);
                
                // ปรับ Alpha กรอบช่อง
                if (slots[i].slotImage != null)
                {
                    Color col = slots[i].slotImage.color;
                    col.a = selectedAlpha;
                    slots[i].slotImage.color = col;
                }

                // ปรับ Alpha ของรูปไอเทมข้างใน
                if (slots[i].itemTextureGraphic != null)
                {
                    Color texCol = slots[i].itemTextureGraphic.color;
                    texCol.a = selectedAlpha;
                    slots[i].itemTextureGraphic.color = texCol;
                }
            }
            else
            {
                // === ช่องอื่นๆ ที่ไม่ได้เลือก ===
                slots[i].rectTransform.localScale = slots[i].defaultScale;
                slots[i].rectTransform.anchoredPosition = slots[i].defaultAnchoredPosition;
                
                // ลด Alpha กรอบช่อง
                if (slots[i].slotImage != null)
                {
                    Color col = slots[i].slotImage.color;
                    col.a = normalAlpha;
                    slots[i].slotImage.color = col;
                }

                // ลด Alpha ของรูปไอเทมข้างใน
                if (slots[i].itemTextureGraphic != null)
                {
                    Color texCol = slots[i].itemTextureGraphic.color;
                    texCol.a = normalAlpha;
                    slots[i].itemTextureGraphic.color = texCol;
                }
            }
        }
    }
}