using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Контроллер главного меню
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button upgradesButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI levelText;

    private void OnEnable()
    {
        newGameButton.onClick.AddListener(StartNewGame);
        upgradesButton.onClick.AddListener(SceneController.LoadUpgrades);
        settingsButton.onClick.AddListener(SceneController.LoadSettings);
        exitButton.onClick.AddListener(SceneController.QuitGame);

        UpdateLevelDisplay();
    }

    private void OnDisable()
    {
        newGameButton.onClick.RemoveListener(StartNewGame);
        upgradesButton.onClick.RemoveListener(SceneController.LoadUpgrades);
        settingsButton.onClick.RemoveListener(SceneController.LoadSettings);
        exitButton.onClick.RemoveListener(SceneController.QuitGame);
    }

    private void StartNewGame()
    {
        SceneController.LoadLevel();
    }

    private void UpdateLevelDisplay()
    {
        int currentLevel = GameManager.Instance.GetGameData().currentLevel;
        levelText.text = $"Level: {currentLevel}";
    }
}
