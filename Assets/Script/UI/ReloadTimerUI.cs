using UnityEngine;
using UnityEngine.UI;

public class ReloadTimerUI : MonoBehaviour
{
    [Header("Reload UI Elements")]
    public GameObject reloadUIPanel; // ตัว GameObject รวมของหลอดรีโหลด
    public Image reloadFillImage;    // ใส่ถ้าใช้ Image ปรับ Fill Amount
    public Slider reloadSlider;      // ใส่ถ้าใช้ Slider

    public void ShowReloadUI(bool show, float maxTime = 1f)
    {
        // ถ้ามีการใส่ Panel คลุมไว้ ให้เปิด/ปิดที่ Panel หลัก
        if (reloadUIPanel != null) 
        {
            reloadUIPanel.SetActive(show);
        }
        else 
        {
            // แต่ถ้าไม่ได้ใส่ Panel ให้เปิด/ปิดที่ตัวหลอดโดยตรง
            if (reloadSlider != null) reloadSlider.gameObject.SetActive(show);
            if (reloadFillImage != null) reloadFillImage.gameObject.SetActive(show);
        }

        // ตั้งค่าเริ่มต้นให้กับ Slider (หลอดเต็ม = maxTime)
        if (show && reloadSlider != null)
        {
            reloadSlider.maxValue = maxTime;
            reloadSlider.value = maxTime;
        }
    }

    public void UpdateReloadProgress(float progressRatio, float remainingTime)
    {
        if (reloadFillImage != null) reloadFillImage.fillAmount = progressRatio;
        if (reloadSlider != null) reloadSlider.value = remainingTime;
    }
}
