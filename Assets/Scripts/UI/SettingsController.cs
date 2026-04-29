using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

/// <summary>
/// Контроллер экрана настроек
/// </summary>
public class SettingsController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Button resetProgressButton;
    [SerializeField] private Button backButton;

    private GameData gameData;
    private Resolution[] resolutions;

    private void OnEnable()
    {
        gameData = GameManager.Instance.GetGameData();
        
        InitializeControls();
        SetupEventListeners();
    }

    private void InitializeControls()
    {
        // Screen Mode
        screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
        screenModeDropdown.value = (int)gameData.settings.screenMode;

        // Resolution
        resolutions = Screen.resolutions;
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        SetCurrentResolutionIndex();

        // Audio
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        masterVolumeSlider.value = gameData.settings.masterVolume;

        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        musicVolumeSlider.value = gameData.settings.musicVolume;

        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        sfxVolumeSlider.value = gameData.settings.sfxVolume;

        // Buttons
        resetProgressButton.onClick.AddListener(ResetProgress);
        backButton.onClick.AddListener(BackToMainMenu);
    }

    private void SetupEventListeners()
    {
        // Already done in InitializeControls
    }

    private void OnDisable()
    {
        screenModeDropdown.onValueChanged.RemoveListener(OnScreenModeChanged);
        resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        resetProgressButton.onClick.RemoveListener(ResetProgress);
        backButton.onClick.RemoveListener(BackToMainMenu);
    }

    private void OnScreenModeChanged(int value) => ChangeScreenMode(value);
    private void OnResolutionChanged(int value) => ChangeResolution(value);
    private void OnMasterVolumeChanged(float value) => ChangeMasterVolume(value);
    private void OnMusicVolumeChanged(float value) => ChangeMusicVolume(value);
    private void OnSFXVolumeChanged(float value) => ChangeSFXVolume(value);
    private void BackToMainMenu() => SceneController.LoadMainMenu();

    private void ChangeScreenMode(int value)
    {
        gameData.settings.screenMode = (FullScreenMode)value;
        GameManager.Instance.ApplySettings();
        GameManager.Instance.SaveGame();
    }

    private void ChangeResolution(int value)
    {
        Resolution res = resolutions[value];
        gameData.settings.screenWidth = res.width;
        gameData.settings.screenHeight = res.height;
        Screen.SetResolution(res.width, res.height, gameData.settings.screenMode);
        GameManager.Instance.SaveGame();
    }

    private void SetCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == gameData.settings.screenWidth && 
                resolutions[i].height == gameData.settings.screenHeight)
            {
                resolutionDropdown.value = i;
                return;
            }
        }
    }

    private void ChangeMasterVolume(float value)
    {
        gameData.settings.masterVolume = value;
        AudioListener.volume = value;
        GameManager.Instance.SaveGame();
    }

    private void ChangeMusicVolume(float value)
    {
        gameData.settings.musicVolume = value;
        GameManager.Instance.SaveGame();
    }

    private void ChangeSFXVolume(float value)
    {
        gameData.settings.sfxVolume = value;
        GameManager.Instance.SaveGame();
    }

    private void ResetProgress()
    {
#if UNITY_EDITOR
        if (EditorUtility.DisplayDialog("Delete progress", 
            "Are you sure? This will delete all level improvements!", "Yes", "Cancel"))
        {
            GameManager.Instance.ResetProgress();
            gameData = GameManager.Instance.GetGameData();
        }
#else
        GameManager.Instance.ResetProgress();
        gameData = GameManager.Instance.GetGameData();
#endif
    }
}
