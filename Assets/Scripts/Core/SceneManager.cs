using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Менеджер переходов между сценами
/// </summary>
public class SceneController : MonoBehaviour
{
    public static void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public static void LoadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level");
    }

    public static void LoadSettings()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Settings");
    }

    public static void LoadUpgrades()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Upgrades");
    }

    public static void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
