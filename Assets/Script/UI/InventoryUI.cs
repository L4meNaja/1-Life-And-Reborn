using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // เพิ่ม TMPro สำหรับ TextMeshPro

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
        public TextMeshProUGUI countText;       // ตัวแสดงจำนวนยา หรือ กระสุนปัจจุบัน
        public TextMeshProUGUI ammoInvText;     // ตัวแสดงจำนวนกระสุนสำรอง (ถ้ามี)
        public RectTransform rectTransform;     // ใช้คุมขนาดและตำแหน่งช่องหลัก
        [HideInInspector] public Vector2 defaultAnchoredPosition;
        [HideInInspector] public Vector3 defaultScale;
        [HideInInspector] public Vector2 defaultSizeDelta;
    }

    [Header("UI Slots (1 to 5)")]
    public SlotUI[] slots = new SlotUI[5];

    [Header("Highlight Settings")]
    public float scaleMultiplier = 1.25f;       // ขยายใหญ่ขึ้น 1.25 เท่า
    public float positionOffsetY = 20f;          // ระยะที่จะให้ขยับขึ้นไป
    public float positionOffsetX = -50f; 
    public float normalAlpha = 0.4f;            // ความโปร่งใสตอนไม่ได้เลือก
    public float selectedAlpha = 0.9f;          // ความโปร่งใสตอนเลือก

    private Dictionary<Texture2D, Sprite> cachedSprites = new Dictionary<Texture2D, Sprite>();

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

                // ค้นหา Object ชื่อ "ItemCount" ถ้ายังไม่ได้ลากใส่
                if (slots[i].countText == null)
                {
                    Transform countTrans = slots[i].slotGameObject.transform.Find("ItemCount");
                    if (countTrans != null)
                    {
                        slots[i].countText = countTrans.GetComponent<TextMeshProUGUI>();
                    }
                }

                // ค้นหา Object ชื่อ "AmmoInv" อัตโนมัติเผื่อลืมลากใส่
                if (slots[i].ammoInvText == null)
                {
                    Transform ammoInvTrans = slots[i].slotGameObject.transform.Find("AmmoInv");
                    if (ammoInvTrans != null)
                    {
                        slots[i].ammoInvText = ammoInvTrans.GetComponent<TextMeshProUGUI>();
                    }
                }

                slots[i].defaultAnchoredPosition = slots[i].rectTransform.anchoredPosition;
                slots[i].defaultScale = slots[i].rectTransform.localScale;
                if (slots[i].itemTextureGraphic != null)
                {
                    slots[i].defaultSizeDelta = slots[i].itemTextureGraphic.rectTransform.sizeDelta;
                }
            }
        }

        if (playerInventory == null)
        {
            playerInventory = Object.FindFirstObjectByType<PlayerInventory>();
        }

        // เริ่มต้นเลือกช่อง 1 ไว้ก่อน
        HighlightSlot(0);
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

            bool isConsumable = (eq != null) && (eq.itemSlot == ItemSlot.HealthPotion || eq.itemSlot == ItemSlot.ShieldPotion);
            bool hasItem = (eq != null && eq.itemTexture != null);
            bool shouldShowGraphic = hasItem;

            // ดึง count จาก PlayerInventory แทน SO
            int itemCount = isConsumable ? playerInventory.GetCountBySlot(eq.itemSlot) : -1;

            if (isConsumable && itemCount <= 0)
            {
                shouldShowGraphic = false; // ซ่อนเฉพาะยาถ้าหมด
            }

            if (shouldShowGraphic)
            {
                slots[i].itemTextureGraphic.gameObject.SetActive(true);

                // ตั้งค่ารูปภาพตามประเภทของ Graphic (Image หรือ RawImage)
                if (slots[i].itemTextureGraphic is RawImage rawImg)
                {
                    rawImg.texture = eq.itemTexture;
                }
                else if (slots[i].itemTextureGraphic is Image img)
                {
                    if (!cachedSprites.ContainsKey(eq.itemTexture))
                    {
                        Sprite newSprite = Sprite.Create(eq.itemTexture, new Rect(0, 0, eq.itemTexture.width, eq.itemTexture.height), new Vector2(0.5f, 0.5f));
                        cachedSprites[eq.itemTexture] = newSprite;
                    }
                    img.sprite = cachedSprites[eq.itemTexture];
                }

                // === ปรับขนาดโดยใช้ LocalScale ตาม textureSize ใน ScriptableObject อัตโนมัติ ===
                RectTransform texRect = slots[i].itemTextureGraphic.rectTransform;
                if (eq.textureSize.x > 0 && eq.textureSize.y > 0)
                {
                    texRect.localScale = eq.textureSize;
                }
                else
                {
                    texRect.localScale = new Vector3(10, 10, 10);
                }

                bool isGun = (eq.itemSlot == ItemSlot.Primary || eq.itemSlot == ItemSlot.Secondary);

                // แสดงจำนวน ItemCount (ยา หรือ กระสุนแม็กกาซีน)
                if (slots[i].countText != null)
                {
                    if (isConsumable)
                    {
                        slots[i].countText.gameObject.SetActive(true);
                        slots[i].countText.text = itemCount.ToString();
                    }
                    else if (isGun)
                    {
                        slots[i].countText.gameObject.SetActive(true);
                        int currentAmmo = eq.itemSlot == ItemSlot.Primary ? playerInventory.primaryAmmoCount : playerInventory.secondaryAmmoCount;
                        slots[i].countText.text = currentAmmo.ToString();
                    }
                    else
                    {
                        slots[i].countText.gameObject.SetActive(false); // ซ่อนเลขของมีด
                    }
                }

                // แสดงจำนวนกระสุนสำรอง (แสดงเฉพาะปืน)
                if (slots[i].ammoInvText != null)
                {
                    if (isGun)
                    {
                        slots[i].ammoInvText.gameObject.SetActive(true);
                        int invAmmo = eq.itemSlot == ItemSlot.Primary ? playerInventory.primaryInvAmmo : playerInventory.secondaryInvAmmo;
                        slots[i].ammoInvText.text = invAmmo.ToString();
                    }
                    else
                    {
                        slots[i].ammoInvText.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                // ถ้าไม่มีไอเทม หรือเป็นยาที่หมดแล้ว
                slots[i].itemTextureGraphic.gameObject.SetActive(false);
                if (slots[i].countText != null) slots[i].countText.gameObject.SetActive(false);
                if (slots[i].ammoInvText != null) slots[i].ammoInvText.gameObject.SetActive(false);
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
                slots[i].rectTransform.anchoredPosition = slots[i].defaultAnchoredPosition + new Vector2(positionOffsetX, positionOffsetY);
                
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