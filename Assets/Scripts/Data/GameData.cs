using UnityEngine;
using System;

/// <summary>
/// Сохраняемые данные игрока
/// </summary>
[System.Serializable]
public class GameData
{
    [System.Serializable]
    public class PlayerUpgrades
    {
        public int hpLevel = 0;
        public int armorLevel = 0;
        public int damageLevel = 0;

        // Каждый уровень дает +10 HP, +1 Armor, +1 DMG
        public int GetMaxHealth() => 100 + (hpLevel * 10);
        public int GetArmor() => 5 + (armorLevel * 1);
        public int GetDamage() => 10 + (damageLevel * 1);
    }

    [System.Serializable]
    public class GameSettings
    {
        public int screenWidth = 1920;
        public int screenHeight = 1080;
        public FullScreenMode screenMode = FullScreenMode.Windowed;
        public float masterVolume = 0.8f;
        public float musicVolume = 0.6f;
        public float sfxVolume = 0.8f;
    }

    public PlayerUpgrades upgrades = new PlayerUpgrades();
    public GameSettings settings = new GameSettings();
    public int currentLevel = 1;
    public int totalCoins = 0;
    public int currentPlayerHealth = -1; // -1 = использовать максимум (для нового уровня)

    // Расскупка первоначального уровня
    public void ResetProgress()
    {
        upgrades = new PlayerUpgrades();
        totalCoins = 0;
        currentLevel = 1;
        currentPlayerHealth = -1;
    }
}
