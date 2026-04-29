# 📋 Архитектура проекта Hero of Loop

## 🏗️ Общая структура системы

```
┌─────────────────────────────────────────────────────────────┐
│                    ИГРОВОЙ МЕНЕДЖЕР                         │
│                   (GameManager Синглтон)                    │
│  ┌─────────────────┬──────────────────┬────────────────┐   │
│  │ PlayerUpgrades  │ GameSettings     │ GameProgress   │   │
│  │ (HP/DMG/Armor)  │ (Звук/Разр-е)   │ (Монеты/Level) │   │
│  └─────────────────┴──────────────────┴────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌──────────────────────────────────────────────────────────────┐
│                    SaveManager                               │
│  Сохраняет/загружает JSON в Application.persistentDataPath   │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│                    SceneController                           │
│  MainMenu → Upgrades/Settings → Level → MainMenu             │
└──────────────────────────────────────────────────────────────┘
```

## 🎮 UI Контроллеры

```
MainMenuController
├── ContinueButton → StartNewLevel()
├── UpgradesButton → LoadUpgrades()
├── SettingsButton → LoadSettings()
└── ExitButton → QuitGame()

UpgradesController
├── HP Upgrade (Button, Level, Cost)
├── Armor Upgrade
└── Damage Upgrade
└── Buy Logic (100 coins per level)

SettingsController
├── Screen Mode Dropdown (Fullscreen/Windowed/Borderless)
├── Resolution Dropdown
├── Master Volume Slider
├── Music Volume Slider
├── SFX Volume Slider
├── Reset Progress Button
└── Back Button

CombatUIController
├── Combat Log Display
├── Player Stats Display
├── Enemy Stats Display
└── Continue Button
```

## 🗺️ Генерация карты

```
MapGenerator
├── Initialize empty 15x10 grid
├── Generate path starting from center
│   ├── Random horizontal movement (±1)
│   ├── Random vertical movement (±1)
│   └── Stay within boundaries
├── Create 20 battle locations
└── Allow placement of location cards

MapRenderer
├── Create tiles from prefab
├── Color tiles by type
│  ├── Red = Battle
│  ├── Green = Forest
│  ├── Gray = Mountain
│  ├── etc.
└── Display on screen
```

## ⚔️ Боевая система

```
CombatSystem
├── Create Player (from GameData)
├── Create Enemy (level-based)
└── Execute Auto Combat
    ├── Round loop:
    │   ├── Player attacks (variance ±20%)
    │   ├── Check if enemy alive
    │   ├── Enemy attacks
    │   ├── Check if player alive
    │   └── Log turn
    └── End combat
        ├── If player wins: Grant coins & card
        └── If player loses: Return to menu

Character
├── Health
├── Armor (reduces incoming damage)
├── Damage (with variance)
└── Methods: TakeDamage(), Heal(), IsAlive()

Enemy
├── Difficulty level scaling
├── Health: 50 + (level * 10)
├── Armor: level * 2
├── Damage: 5 + (level * 2)
└── Gold: 50 + (level * 10)
```

## 🎴 Система карточек

```
LocationCardConfig (ScriptableObject)
├── List<LocationCard>
│   ├── Type (Forest, Mountain, etc.)
│   ├── Icon & Display Name
│   ├── Drop Chance (0.0-1.0)
│   └── Bonuses (HP/DMG/Armor)
└── GetRandomCard() - Weighted random selection

LocationCard Types:
├── Battle (Red) - Combat encounter
├── Empty (White) - No effect
├── Forest (Green) - Green location
├── Mountain (Gray) - Gray location
├── Cemetery (Dark) - Dark location
├── Library (Blue) - Blue location
├── Shop (Yellow) - Yellow location
└── Mansion (Purple) - Purple location
```

## 💾 Сохранение данных

```
JSON Structure (gamesave.json)
{
  "upgrades": {
    "hpLevel": 0-N,
    "armorLevel": 0-N,
    "damageLevel": 0-N
  },
  "settings": {
    "screenWidth": 1920,
    "screenHeight": 1080,
    "screenMode": 0-3,
    "masterVolume": 0.0-1.0,
    "musicVolume": 0.0-1.0,
    "sfxVolume": 0.0-1.0
  },
  "currentLevel": 1-N,
  "totalCoins": 0-N
}

Location: Application.persistentDataPath/gamesave.json
```

## 🔄 Игровой цикл

```
START
  │
  ▼
[MainMenu]
  ├─ [Continue] → [GenerateLevel]
  │                    │
  │                    ▼
  │              [RenderMap]
  │                    │
  │                    ▼
  │              [StartBattle]
  │                    │
  │                    ▼
  │              [ExecuteCombat]
  │                    │
  │              ┌─────┴─────┐
  │              ▼ Win       ▼ Lose
  │         [GiveReward]  [EndLevel]
  │              │            │
  │              └─────┬──────┘
  │                    ▼
  │          [ShowLocationCard]
  │                    │
  │         ┌──────────┴──────────┐
  │         ▼                     ▼
  │    [PlaceCard]           [NextBattle]
  │         │                     │
  │         └──────────┬──────────┘
  │                    │
  │            [MoreBattles?]
  │              YES  │  NO
  │                  ▼
  │            [CompleteLevel]
  │                    │
  │            [AddCoinsToBank]
  │                    │
  │            [IncrementLevel]
  │                    │
  │                    ▼
  └──────────[Return MainMenu]

[Upgrades]
  ├─ Buy HP/Armor/DMG (100 coins)
  └─ [Back] → [MainMenu]

[Settings]
  ├─ Resolution
  ├─ Screen Mode
  ├─ Volume
  ├─ Reset Progress
  └─ [Back] → [MainMenu]

[Exit] → END
```

## 📊 Баланс системы

### Улучшения (за 100 монет)
```
Level 1: 110 HP      Level 1: 1 ARM      Level 1: 2 DMG
Level 2: 120 HP      Level 2: 2 ARM      Level 2: 3 DMG
Level 3: 130 HP      Level 3: 3 ARM      Level 3: 4 DMG
...
```

### Враги (по уровню)
```
Lv.1: 60 HP   | 2 ARM | 7 DMG | 60 gold
Lv.2: 70 HP   | 4 ARM | 9 DMG | 70 gold
Lv.3: 80 HP   | 6 ARM | 11 DMG| 80 gold
...
Lv.N: 50+10N  | 2N    | 5+2N  | 50+10N
```

### Экономика
```
Battle won: 50-200+ золота
Upgrade cost: 100 золота
Path length: 20 боевых локаций = 1000-4000 золота за уровень
```

## 🎛️ Где настраивать параметры

| Параметр | Файл | Класс | Переменная |
|----------|------|-------|-----------|
| Map size | MapGenerator.cs | MapGenerator | mapWidth, mapHeight |
| Tile size | MapGenerator.cs | MapGenerator | tileSize |
| Path length | MapGenerator.cs | MapGenerator | pathLength |
| Upgrade cost | UpgradesController.cs | UpgradesController | UPGRADE_COST |
| Enemy health base | Character.cs | Enemy | maxHealth = 50 |
| Health formula | Character.cs | Enemy | 50 + (level * 10) |
| Card chances | LocationCardConfig | Inspector | dropChance |

## 🔗 Зависимости между системами

```
GameManager (Core)
  │
  ├─→ SaveManager (Loading/Saving)
  │
  └─→ SceneController (Scene Navigation)
       │
       ├─→ MainMenuController (UI)
       │
       ├─→ UpgradesController (UI)
       │
       ├─→ SettingsController (UI)
       │
       └─→ LevelManager (Game Logic)
            │
            ├─→ MapGenerator (World)
            ├─→ MapRenderer (World)
            ├─→ LocationCardConfig (World)
            │
            └─→ CombatSystem (Combat)
                 │
                 ├─→ Character (Combat)
                 ├─→ Enemy (Combat)
                 │
                 └─→ CombatUIController (UI)
```

## 📝 Порядок выполнения на старт

1. **Unity Load Scene** → MainMenu.unity
2. **Awake()** → GameManager.Instance создается
3. **Start()** → GameManager загружает GameData из JSON
4. **OnEnable()** → MainMenuController устанавливает события кнопок
5. **Player видит главное меню**
6. **Player нажимает кнопку** → OnClick событие
7. **Скрипт загружает новую сцену** → SceneController.LoadScene()
8. **Повторяется цикл для новой сцены**

## 🧪 Тестовые сценарии

### Scenario 1: First Run
1. Запустить MainMenu
2. Нажать "Продолжить"
3. Проверить, что карта сгенерирована
4. Проверить, что боевая система работает
5. Проверить, что получены монеты и карточка

### Scenario 2: Upgrades
1. Пройти 1 уровень (получить монеты)
2. Нажать "Улучшения"
3. Купить улучшение HP
4. Проверить, что уровень увеличился
5. Вернуться в меню

### Scenario 3: Settings
1. Нажать "Настройки"
2. Изменить разрешение экрана
3. Изменить громкость
4. Нажать "Вернуться"
5. Перезагрузить игру
6. Проверить, что настройки сохранились

### Scenario 4: Reset
1. Пройти несколько уровней
2. Купить улучшения
3. Нажать "Настройки"
4. Нажать "Сброс прогресса"
5. Проверить, что всё сброшено

---

**Последнее обновление: 7 апреля 2024**
