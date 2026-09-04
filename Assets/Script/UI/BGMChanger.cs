using UnityEngine;

public class BGMChanger : MonoBehaviour
{
    [Header("BGM Source")]
    public AudioSource bgmSource;

    [Header("Music List")]
    public AudioClip[] bgmList;

    private int currentMusicIndex = 0;

    public void ChangeBGM()
    {
        if (bgmList == null || bgmList.Length == 0)
            return;

        currentMusicIndex++;

        // ถ้าเกินเพลงสุดท้าย ให้กลับไปเพลงแรก
        if (currentMusicIndex >= bgmList.Length)
        {
            currentMusicIndex = 0;
        }

        bgmSource.clip = bgmList[currentMusicIndex];
        bgmSource.Play();
    }
}   