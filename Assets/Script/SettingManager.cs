using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class SettingManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject victoryPanel;

    [Header("Audio Settings (Mixer)")]
    public AudioMixer myMixer;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("Audio Settings (Source)")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    private bool isPaused = false;
    private float maxBgmVolume = 0.5f;
    public Test_Player player;

    void Start()
    {
        // โหลดค่าเสียงที่เซฟไว้
        float savedBgm = PlayerPrefs.GetFloat("BGMVol", 0.5f);
        float savedSfx = PlayerPrefs.GetFloat("SFXVol", 0.8f);

        // ตั้งค่า Mixer
        if (myMixer != null)
        {
            myMixer.SetFloat(
                "BGMVol",
                Mathf.Log10(Mathf.Clamp(savedBgm, 0.0001f, 1f)) * 20
            );

            myMixer.SetFloat(
                "SFXVol",
                Mathf.Log10(Mathf.Clamp(savedSfx, 0.0001f, 1f)) * 20
            );
        }

        // ตั้งค่า Slider
        if (bgmSlider != null)
        {
            bgmSlider.value = savedBgm / maxBgmVolume;
            bgmSlider.onValueChanged.AddListener(SetVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSfx;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // ตั้งค่า Audio Source
        if (bgmSource != null)
            bgmSource.volume = savedBgm;

        if (sfxSource != null)
            sfxSource.volume = savedSfx;

        AudioListener.volume = 1f;

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        // ล็อกเมาส์เมื่อเริ่มเกม
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ถ้าชนะแล้ว ไม่ให้กด Pause
        if (victoryPanel != null && victoryPanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
{
    settingsPanel.SetActive(true);

    Time.timeScale = 0f;
    isPaused = true;

    player.canMove = false;

    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}

    public void ResumeGame()
{
    settingsPanel.SetActive(false);

    Time.timeScale = 1f;
    isPaused = false;

    player.canMove = true;

    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}

    // ปรับเสียงเพลง
    public void SetVolume(float volume)
    {
        float finalVolume = volume * maxBgmVolume;

        if (myMixer != null)
        {
            myMixer.SetFloat(
                "BGMVol",
                Mathf.Log10(Mathf.Clamp(finalVolume, 0.0001f, 1f)) * 20
            );
        }

        if (bgmSource != null)
            bgmSource.volume = finalVolume;

        PlayerPrefs.SetFloat("BGMVol", finalVolume);
    }

    // ปรับเสียง SFX
    public void SetSFXVolume(float volume)
    {
        if (myMixer != null)
        {
            myMixer.SetFloat(
                "SFXVol",
                Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20
            );
        }

        if (sfxSource != null)
            sfxSource.volume = volume;

        PlayerPrefs.SetFloat("SFXVol", volume);
    }

    public void GoToMainMenu()
    {
        StartCoroutine(WaitAndLoad("MainMenu"));
    }

    public void NextLevel(string sceneName)
    {
        StartCoroutine(WaitAndLoad(sceneName));
    }

    public void RestartLevel()
    {
        StartCoroutine(WaitAndLoad(SceneManager.GetActiveScene().name));
    }

    IEnumerator WaitAndLoad(string sceneName)
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSecondsRealtime(1f);

        SceneManager.LoadScene(sceneName);
    }
}