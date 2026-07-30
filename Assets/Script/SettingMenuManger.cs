using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingMenuManger : MonoBehaviour
{
    public TMP_Dropdown ResDropDown;
    public Toggle FullscreenToggle;
    public Toggle FPSToggle; // ลากปุ่มติ๊กถูกเปิดปิด FPS ในหน้า Setting มาใส่ช่องนี้
    Resolution[] AllResolution;
    bool IsFullScreen;
    int SelectResolution;
    List<Resolution> SelectResolutionList = new List<Resolution>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ตรวจสอบว่าในเครื่องเซฟไว้ให้เปิดหรือปิด FPS แล้วปรับหัวข้อติ๊กถูกให้ตรงตามนั้น
        FPSToggle.isOn = PlayerPrefs.GetInt("ShowFPS", 0) == 1;

        IsFullScreen = true;
        AllResolution = Screen.resolutions;

        // ล้างค่าเก่าใน Dropdown ออกก่อนเพื่อความชัวร์
        ResDropDown.ClearOptions();

        List<string> ResolutionStringList = new List<string>();
        string newRes;
        
        int defaultResolutionIndex = 0; // ตัวจำตำแหน่งความละเอียดเริ่มต้น

        for (int i = 0; i < AllResolution.Length; i++)
        {
            Resolution res = AllResolution[i];
            newRes = res.width.ToString() + "x" + res.height.ToString();
            
            if(!ResolutionStringList.Contains(newRes))
            {
                ResolutionStringList.Add(newRes);
                SelectResolutionList.Add(res);

                // ค้นหาความละเอียด 1920x1080 หรือขนาดที่ตรงกับหน้าจอปัจจุบันของเครื่องผู้เล่น
                if (res.width == 1920 && res.height == 1080)
                {
                    // ถ้าเจอ 1920x1080 ให้จำตำแหน่งนี้ไว้ใน Dropdown
                    defaultResolutionIndex = SelectResolutionList.Count - 1;
                }
                else if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height && defaultResolutionIndex == 0)
                {
                    // หรือถ้าไม่เจอ 1920x1080 แต่เจอขนาดที่ตรงกับจอคอมปัจจุบัน (เช่น จอ 2K/4K) ให้ใช้ค่านี้สำรองไว้ก่อน
                    defaultResolutionIndex = SelectResolutionList.Count - 1;
                }
            }
        }

        ResDropDown.AddOptions(ResolutionStringList);

        // สั่งให้ Dropdown แสดงผลและเลือกช่องที่เป็นค่าเริ่มต้นที่เราหาไว้
        SelectResolution = defaultResolutionIndex;
        ResDropDown.value = defaultResolutionIndex;
        ResDropDown.RefreshShownValue();

        // ตั้งค่า Toggle ให้ตรงกับค่า IsFullScreen จริงๆ
        FullscreenToggle.isOn = IsFullScreen;
    }

    public void ChangeResolution()
    {
        SelectResolution = ResDropDown.value;
        Screen.SetResolution(SelectResolutionList[SelectResolution].width, SelectResolutionList[SelectResolution].height, IsFullScreen);
    }

    public void ChangeFullscreen()
    {
        IsFullScreen = FullscreenToggle.isOn;
        Screen.SetResolution(SelectResolutionList[SelectResolution].width, SelectResolutionList[SelectResolution].height, IsFullScreen);
    }
    // ฟังก์ชันเมื่อผู้เล่นกดติ๊กถูกเข้าๆ ออกๆ ที่หน้าเมนูตั้งค่า

    public void ChangeFPSVisibility()
    {
        // สั่งการข้ามฉากไปยังสคริปต์ FPSDisplay ตัวจริงที่เป็นอมตะอยู่ทันทีผ่านคำว่า instance
        if (FPSDisplay.instance != null)
        {
            FPSDisplay.instance.SetFPSVisibility(FPSToggle.isOn);
        }
    }
}