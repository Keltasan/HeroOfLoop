using UnityEngine;

/// <summary>
/// Главный менеджер игры - синглтон
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private GameData gameData;
    private SaveManager saveManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        saveManager = GetComponent<SaveManager>();
        if (saveManager == null)
            saveManager = gameObject.AddComponent<SaveManager>();
        
        LoadGame();
    }

    public void LoadGame()
    {
        gameData = saveManager.LoadGame();
        ApplySettings();
    }

    public void SaveGame()
    {
        saveManager.SaveGame(gameData);
    }

    public void ApplySettings()
    {
        Screen.SetResolution(gameData.settings.screenWidth, gameData.settings.screenHeight, gameData.settings.screenMode);
        AudioListener.volume = gameData.settings.masterVolume;
    }

    public GameData GetGameData() => gameData;
    
    public void ResetProgress()
    {
        gameData.ResetProgress();
        SaveGame();
    }

    public void AddCoins(int amount)
    {
        gameData.totalCoins += amount;
        SaveGame();
    }

    public void NextLevel()
    {
        gameData.currentLevel++;
        SaveGame();
    }
}
