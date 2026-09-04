using UnityEngine;
public class ButtonSound : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clickSfx; // เสียงคลิกทั่วไป / เสียงเป็ด // เสียงตอนกด "ย้อนกลับ"
    public void PlayClick()
    {
        if (source != null && clickSfx != null)
        {
            source.PlayOneShot(clickSfx);
        }
    }

    // ฟังก์ชันพิเศษ: เล่นเสียงอะไรก็ได้ที่ส่งเข้ามา
    public void PlayCustomSound(AudioClip clip)
    {
        if (source != null && clip != null)
        {
            source.PlayOneShot(clip);
        }
    }
}
