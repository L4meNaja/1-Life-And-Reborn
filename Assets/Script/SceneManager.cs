using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    // โหลด Scene ตามชื่อ
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // โหลด Scene ปัจจุบันใหม่
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ออกจากเกม
    public void ExitGame()
    {
        Application.Quit();

        // ใช้ตอนกดใน Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}