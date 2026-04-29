using UnityEngine;
using System.IO;

/// <summary>
/// Система сохранения и загрузки данных игры
/// </summary>
public class SaveManager : MonoBehaviour
{
    private string savePath;
    private const string SAVE_FILENAME = "gamesave.json";

    private void OnEnable()
    {
        savePath = Path.Combine(Application.persistentDataPath, SAVE_FILENAME);
        Debug.Log($"Save path: {savePath}");
    }

    public void SaveGame(GameData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log("Game saved successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public GameData LoadGame()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                GameData data = JsonUtility.FromJson<GameData>(json);
                Debug.Log("Game loaded successfully!");
                return data;
            }
            else
            {
                Debug.Log("No save file found, creating new game data");
                return new GameData();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return new GameData();
        }
    }

    public void DeleteSave()
    {
        try
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("Save file deleted!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to delete save: {e.Message}");
        }
    }
}
