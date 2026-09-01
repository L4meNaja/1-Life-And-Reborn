using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private float currentFPS = 0.0f;

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.2f; 
    private float nextUpdateTime = 0.0f;
    
    // สร้างตัวแปรแชร์ให้สคริปต์อื่น (เช่น สคริปต์ปุ่ม) สั่งการข้ามฉากได้
    public static FPSDisplay instance; 

    // เพิ่มตัวแปรเช็คว่าผู้เล่นสั่งให้เปิดหรือปิดอยู่
    private bool isShowFPS = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // ดึงค่าที่เซฟไว้จากรอบก่อน (0 = ปิด, 1 = เปิด) ถ้าไม่เคยเซฟเลยให้ค่าเริ่มต้นเป็น 0 (ปิดไว้ก่อน)
            isShowFPS = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;
    }

    // ฟังก์ชันส่วนกลางที่เอาไว้ให้ปุ่มเปิด-ปิดในหน้า Setting เรียกใช้งาน
    public void SetFPSVisibility(bool visible)
    {
        isShowFPS = visible;
        
        // เซฟค่าเก็บไว้ในเครื่องคอมพิวเตอร์ของผู้เล่นทันที ปิดเกมเปิดใหม่ค่าก็จะไม่หาย
        PlayerPrefs.SetInt("ShowFPS", visible ? 1 : 0);
        PlayerPrefs.Save();
    }

    void Update()
    {
        // ถ้าผู้เล่นสั่งปิดอยู่ ไม่ต้องเสียเวลาคำนวณเลขให้กินทรัพยากรเครื่อง
        if (!isShowFPS) return;

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (Time.unscaledTime >= nextUpdateTime)
        {
            currentFPS = 1.0f / deltaTime;
            nextUpdateTime = Time.unscaledTime + updateInterval;
        }
    }

    void OnGUI()
    {
        // ถ้าผู้เล่นสั่งปิดอยู่ ไม่ต้องวาดตัวอักษรลงบนหน้าจอ
        if (!isShowFPS) return;

        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(20, 20, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 3 / 100; 
        style.normal.textColor = Color.green; 

        string text = string.Format("{0:0.} FPS", currentFPS);
        GUI.Label(rect, text, style);
    }
}
