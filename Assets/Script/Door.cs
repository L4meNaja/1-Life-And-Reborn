using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject interactTextUI;

    [Header("Movement Settings")]
    public GameObject objectToMove;
    public float moveDistance = -10f;

    [Header("Level Generation")]
    public LevelGenerator levelGenerator;

    private bool isPlayerInTrigger = false;
    private bool hasGenerated = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        // ซ่อนข้อความตอนเริ่มเกม
        if (interactTextUI != null)
        {
            interactTextUI.SetActive(false);
        }

        // ถ้าไม่ได้ใส่วัตถุที่จะขยับ ให้ใช้ตัวเอง
        if (objectToMove == null)
        {
            objectToMove = this.gameObject;
        }
    }

    void Update()
    {
        // Player อยู่ในโซน + กด E + ยังไม่เคยเปิด
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && !hasGenerated)
        {
            Interact();
        }
    }

    void Interact()
    {
        // เปิดประตู
        Vector3 currentPosition = objectToMove.transform.position;

        objectToMove.transform.position = new Vector3(
            currentPosition.x + moveDistance,
            currentPosition.y,
            currentPosition.z
        );

        // เล่นเสียง
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // สร้าง Corridor + Room
        if (levelGenerator != null)
        {
            levelGenerator.GenerateLevel();
        }

        // ป้องกันการสร้างด่านซ้ำ
        hasGenerated = true;

        // ซ่อนข้อความ
        if (interactTextUI != null)
        {
            interactTextUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;

            if (interactTextUI != null && !hasGenerated)
            {
                interactTextUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;

            if (interactTextUI != null)
            {
                interactTextUI.SetActive(false);
            }
        }
    }
}