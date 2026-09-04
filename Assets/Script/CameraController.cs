using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    private CinemachineThirdPersonFollow _c3p;

    [Header("Default Offset Settings")]
    [SerializeField] private Vector3 defaultOffset = new Vector3(1.8f, 0.32f, -1.52f);
    
    [Header("Recoil Recovery Settings")]
    [SerializeField] private float returnSpeed = 10f; // ความเร็วในการดึงกล้องกลับจุดเดิม

    private Vector3 targetOffset;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        _c3p = GetComponent<CinemachineThirdPersonFollow>();
        targetOffset = defaultOffset;
    }

    void Update()
    {
        if (_c3p == null) return;

        // ค่อยๆ ดึง Offset ปัจจุบัน กลับเข้าหา defaultOffset แบบนุ่มนวล
        targetOffset = Vector3.Lerp(targetOffset, defaultOffset, Time.deltaTime * returnSpeed);
        _c3p.ShoulderOffset = targetOffset;
    }

    // เรียกฟังก์ชันนี้ตอนยิงปืน เพื่อเพิ่มแรงกระตุกชั่วคราว
    public void ShoulderOffset(float offsetX, float offsetY, float offsetZ) 
    {
        // บวกค่า Recoil เพิ่มเข้าไปจากตำแหน่งปัจจุบันทันที
        targetOffset += new Vector3(offsetX, offsetY, -offsetZ);
    }
}