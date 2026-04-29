# Extension Guide для Hero of Loop

Руководство по расширению проекта и добавлению новых функций.

## 🏗️ Архитектура расширений

Проект разработан с использованием модульной архитектуры, что облегчает добавление новых функций.

## 🗺️ Добавление новых типов локаций

### 1. Расширьте LocationType enum
```csharp
// В LocationCard.cs
public enum LocationType
{
    Empty,
    Forest,
    Mountain,
    Cemetery,
    Library,
    Shop,
    Mansion,
    Battle,
    // Новые типы:
    Bridge,
    Market,
    Temple,
    Castle
}
```

### 2. Добавьте карточку в LocationCardConfig
```csharp
public class LocationCard
{
    public LocationType type;
    public string displayName;      // "Мост", "Рынок", etc.
    public Sprite icon;             // Иконка локации
    public float dropChance = 0.1f;  // Вероятность выпадения
    public int hpBonus;             // Бонусы
    public int damageBonus;
    public int armorBonus;
}
```

### 3. Обновите цвета в MapRenderer
```csharp
private Color GetColorForLocationType(LocationType type)
{
    return type switch
    {
        // ... существующие типы ...
        LocationType.Bridge => Color.cyan,
        LocationType.Market => new Color(1f, 0.5f, 0f),
        LocationType.Temple => new Color(1f, 0.84f, 0f),
        LocationType.Castle => Color.gray,
        _ => Color.white
    };
}
```

### 4. Добавьте эффекты каждой локации
```csharp
// Создайте новый класс Location.cs
public class Location : MonoBehaviour
{
    public LocationType type;
    
    public virtual void ApplyEffect(Character player)
    {
        // По умолчанию - ничего
    }
}

public class ForestLocation : Location
{
    public override void ApplyEffect(Character player)
    {
        player.Heal(30); // Лес лечит
        Debug.Log("Лес восстановил 30 HP!");
    }
}

public class LibraryLocation : Location
{
    public override void ApplyEffect(Character player)
    {
        player.armor += 2;  // Библиотека дает броню
        Debug.Log("Библиотека дала +2 броню!");
    }
}
```

## 👹 Добавление новых типов врагов

### 1. Создайте подклассы Enemy
```csharp
public class EliteEnemy : Enemy
{
    public EliteEnemy(int difficultyLevel) : base(difficultyLevel)
    {
        maxHealth *= 2;           // Вдвое больше HP
        armor += 5;               // Больше брони
        damage *= 1.5f;           // Больше урона
        goldReward *= 3;          // Больше награды
    }
}

public class BossEnemy : Enemy
{
    public BossEnemy(int difficultyLevel) : base(difficultyLevel)
    {
        maxHealth *= 4;
        armor = difficultyLevel * 5;
        damage = (int)(10 + (difficultyLevel * 3));
        goldReward = difficultyLevel * 100;
    }
}
```

### 2. Обновите боевую систему
```csharp
// В CombatSystem.cs
public void StartCombat(int enemyLevel, EnemyType enemyType = EnemyType.Normal)
{
    inCombat = true;
    
    // Создаем врага нужного типа
    currentEnemy = enemyType switch
    {
        EnemyType.Normal => new Enemy(enemyLevel),
        EnemyType.Elite => new EliteEnemy(enemyLevel),
        EnemyType.Boss => new BossEnemy(enemyLevel),
        _ => new Enemy(enemyLevel)
    };
    
    ExecuteCombat();
}
```

### 3. Добавьте специальные способности
```csharp
public class EliteEnemy : Enemy
{
    private int specialAttackCooldown = 0;
    
    public override int GetDamage()
    {
        specialAttackCooldown--;
        
        if (specialAttackCooldown <= 0 && Random.value > 0.6f)
        {
            specialAttackCooldown = 3;
            return (int)(base.GetDamage() * 2.5f); // Специальная атака
        }
        
        return base.GetDamage();
    }
}
```

## 🎯 Система достижений

### 1. Создайте класс достижений
```csharp
[System.Serializable]
public class Achievement
{
    public string id;
    public string displayName;
    public string description;
    public bool unlocked;
    public Sprite icon;
}

[System.Serializable]
public class AchievementData
{
    public List<Achievement> achievements = new List<Achievement>();
}
```

### 2. Добавьте менеджер
```csharp
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    private AchievementData achievementData;
    
    public void UnlockAchievement(string id)
    {
        var achievement = achievementData.achievements.Find(a => a.id == id);
        if (achievement != null && !achievement.unlocked)
        {
            achievement.unlocked = true;
            Debug.Log($"Достижение разблокировано: {achievement.displayName}");
            GameManager.Instance.SaveGame();
        }
    }
    
    public void OnBattleWon(int level)
    {
        UnlockAchievement("first_battle");
        if (level >= 10)
            UnlockAchievement("level_10");
    }
}
```

## 🎨 Система эффектов

### 1. Добавьте систему частиц
```csharp
public class EffectManager : MonoBehaviour
{
    public static void PlayDamageEffect(Vector3 position)
    {
        // Создать и воспроизвести эффект урона
        var effect = EffectPool.GetDamageEffect();
        effect.transform.position = position;
        effect.Play();
    }
    
    public static void PlayHealEffect(Vector3 position)
    {
        var effect = EffectPool.GetHealEffect();
        effect.transform.position = position;
        effect.Play();
    }
    
    public static void PlayLevelUpEffect(Vector3 position)
    {
        var effect = EffectPool.GetLevelUpEffect();
        effect.transform.position = position;
        effect.Play();
    }
}
```

### 2. Добавьте звуковые эффекты
```csharp
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip defeatlSound;
    
    private AudioSource audioSource;
    
    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(attackSound);
    }
    
    public void PlayVictorySound()
    {
        audioSource.PlayOneShot(victorySound);
    }
}
```

## 📊 Система статистики

### 1. Создайте окно статистики
```csharp
public class StatsScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalBattlesText;
    [SerializeField] private TextMeshProUGUI totalEnemiesDefeatedText;
    [SerializeField] private TextMeshProUGUI totalCoinsEarnedText;
    [SerializeField] private TextMeshProUGUI highestLevelText;
    
    private void OnEnable()
    {
        GameData data = GameManager.Instance.GetGameData();
        
        // Отобразить статистику
        totalBattlesText.text = $"Битв: {data.statistics.totalBattles}";
        totalEnemiesDefeatedText.text = $"Врагов побеждено: {data.statistics.enemiesDefeated}";
        totalCoinsEarnedText.text = $"Золота заработано: {data.statistics.totalCoinsEarned}";
        highestLevelText.text = $"Макс. уровень: {data.statistics.highestLevel}";
    }
}
```

## 🎮 Система прогрессии

### 1. Расширьте GameData
```csharp
[System.Serializable]
public class ProgressionData
{
    public int currentStreak = 0;           // Текущая серия побед
    public int highestStreak = 0;          // Рекордная серия
    public int levelUpgrades = 0;          // Всего покупок улучшений
    public Dictionary<string, int> specialUnlocks = new Dictionary<string, int>();
}
```

### 2. Добавьте бонусы за достижения
```csharp
public void CheckMilestones()
{
    if (gameData.currentLevel == 5)
        GrantMilestoneReward("level_5");
    
    if (gameData.currentLevel == 10)
        GrantMilestoneReward("level_10");
    
    if (gameData.totalCoins >= 1000)
        GrantMilestoneReward("1000_coins");
}

private void GrantMilestoneReward(string milestoneId)
{
    // Дать награду игроку
    gameData.AddCoinBonus(100);
    NotifyPlayer($"Миллион достигнут: {milestoneId}");
}
```

## 🔊 Система очков

### 1. Система очков за действия
```csharp
public enum ScoreAction
{
    BattleWon = 50,
    LevelCompleted = 100,
    BossDefeated = 500,
    NoDamageVictory = 250,
    SpeedRun = 75
}

public void AddScore(ScoreAction action)
{
    int points = (int)action;
    gameData.totalScore += points;
    GameManager.Instance.SaveGame();
}
```

### 2. Лидерборд
```csharp
[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public int level;
    public System.DateTime date;
}

public class LeaderboardManager : MonoBehaviour
{
    private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    
    public void AddEntry(string playerName, int score, int level)
    {
        entries.Add(new LeaderboardEntry
        {
            playerName = playerName,
            score = score,
            level = level,
            date = System.DateTime.Now
        });
        
        entries = entries.OrderByDescending(e => e.score).ToList();
    }
}
```

## 📱 Мобильная оптимизация

### 1. Адаптивный UI
```csharp
public class ResponsiveUI : MonoBehaviour
{
    private void OnEnable()
    {
        #if UNITY_IOS || UNITY_ANDROID
            OptimizeForMobile();
        #endif
    }
    
    private void OptimizeForMobile()
    {
        // Увеличить размер кнопок
        // Оптимизировать шрифты
        // Адаптировать разрешение
    }
}
```

### 2. Управление сенсорным экраном
```csharp
public class TouchInput : MonoBehaviour
{
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                HandleTouchDown(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                HandleTouchUp(touch.position);
            }
        }
    }
    
    private void HandleTouchDown(Vector2 position)
    {
        // Обработка нажатия
    }
}
```

## 🌍 Локализация

### 1. Система строк
```csharp
[System.Serializable]
public class LocalizationStrings
{
    private Dictionary<string, Dictionary<string, string>> strings = 
        new Dictionary<string, Dictionary<string, string>>();
    
    public string GetString(string key, string language = "ru")
    {
        if (strings.ContainsKey(language) && strings[language].ContainsKey(key))
            return strings[language][key];
        return key; // Fallback
    }
}
```

### 2. Использование
```csharp
// Вместо hardcoded строк:
// continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Продолжить";

// Используйте:
string continueText = LocalizationManager.GetString("main_menu.continue");
```

## ✅ Чек-лист для расширений

- [ ] Добавлены новые типы локаций
- [ ] Добавлены новые враги
- [ ] Система достижений работает
- [ ] Эффекты воспроизводятся
- [ ] Звуки работают
- [ ] Статистика ведется
- [ ] Все UI элементы обновлены
- [ ] Тестирование на всех разрешениях
- [ ] Локализация добавлена

---

**Помните: тестируйте каждое расширение перед добавлением нового!**
