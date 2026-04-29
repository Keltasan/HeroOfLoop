# Hero of Loop - 2D Rogue-Like Autobattler

Проект клона Loop Hero на Unity с процедурной генерацией карт, системой улучшений и автоматическим боевым процессом.

## 📋 Структура проекта

```
Assets/Scripts/
├── Core/                 # Основные системы управления
│   ├── GameManager.cs   # Главный менеджер игры (синглтон)
│   ├── SaveManager.cs   # Система сохранения/загрузки
│   └── SceneController.cs # Навигация между сценами
├── Data/                # Классы данных
│   └── GameData.cs      # Структура сохраняемых данных
├── UI/                  # Контроллеры интерфейса
│   ├── MainMenuController.cs      # Главное меню
│   ├── UpgradesController.cs      # Экран улучшений
│   ├── SettingsController.cs      # Экран настроек
│   └── CombatUIController.cs      # UI боевой системы
├── Game/               # Логика уровней
│   └── LevelManager.cs # Менеджер уровня
├── World/              # Система мира и карт
│   ├── LocationCard.cs # Данные карточек локаций
│   ├── MapGenerator.cs # Генератор карт
│   └── MapRenderer.cs  # Визуализация карт
└── Combat/             # Боевая система
    ├── Character.cs    # Персонажи и враги
    └── CombatSystem.cs # Логика боя
```

## 🎮 Основные компоненты

### 1. **GameManager** (Синглтон)
- Управляет состоянием игры
- Загружает и сохраняет прогресс
- Применяет настройки

```csharp
// Использование:
GameData data = GameManager.Instance.GetGameData();
GameManager.Instance.AddCoins(100);
GameManager.Instance.SaveGame();
```

### 2. **SaveManager**
- Сохраняет данные в JSON
- Сохраняется в `Application.persistentDataPath`
- Поддерживает автоматическое восстановление

### 3. **GameData** (Сереализуемо)
Содержит:
- **PlayerUpgrades**: Уровни улучшений (HP, Броня, Урон)
- **GameSettings**: Настройки (разрешение, звук, режим экрана)
- **Общая информация**: Уровень, монеты, ...)

### 4. **MapGenerator**
Генерирует процедурные карты с гарантией:
- Путь игрока остается в пределах экрана
- Автоматический выбор направления движения
- Рандомизированные локации

```csharp
// Использование:
mapRenderer.RenderMap();
mapGenerator.PlaceLocationCard(5, 3, LocationType.Forest);
```

### 5. **LocationCard** (Карточки локаций)
Типы локаций:
- `Battle` - Боевая локация
- `Forest`, `Mountain`, `Cemetery` - Природные локации
- `Library`, `Shop`, `Mansion` - Здания

Каждая карточка имеет:
- `dropChance` - Вероятность выпадения (0.0 - 1.0)
- Бонусы: HP, DMG, Armor

### 6. **CombatSystem** (Автобой)
- Автоматическое выполнение боя
- События для UI обновлений
- Логирование хода боя

```csharp
// Использование:
combatSystem.StartCombat(enemyLevel);
// Бой выполняется автоматически
```

### 7. **Character** и **Enemy**
- **Character**: Базовый класс для персонажей
  - health, armor, damage, name
- **Enemy**: Враг с уровнем сложности
  - goldReward автоматически рассчитывается

## 🛠 Настройка проекта

### Создание Сцен

Создайте 4 сцены в `Assets/Scenes/`:

1. **MainMenu**
   - Добавьте Canvas с UI
   - Добавьте GameManager (префаб) в сцену
   - Присоедините MainMenuController к Canvas

2. **Upgrades**
   - Создайте UI с 3 группами улучшений
   - Присоедините UpgradesController

3. **Settings**
   - Создайте UI с выпадающими меню для разрешения и режима
   - Добавьте слайдеры для звука
   - Присоедините SettingsController

4. **Level**
   - Создайте контейнер для тайлов карты
   - Добавьте MapGenerator и MapRenderer
   - Добавьте LevelManager и CombatSystem
   - Создайте Canvas для UI боя

### Префабы и Конфиги

1. **GameManager Префаб**
   - Объект с компонентами GameManager и SaveManager
   - Сделайте DontDestroyOnLoad

2. **LocationCardConfig** (ScriptableObject)
   - Создайте: Right-click → Create → Game → Location Card Config
   - Настройте вероятности выпадения каждой карточки

3. **Tile Префаб** (для MapRenderer)
   - Простой Sprite с SpriteRenderer

## 📊 Система Улучшений

Каждый уровень улучшения дает:
- **HP**: +10 HP за уровень
- **Armor**: +1 броня за уровень
- **Damage**: +1 урон за уровень

Стоимость: 100 монет за уровень

```csharp
int maxHealth = 100 + (hpLevel * 10);
int armor = 0 + (armorLevel * 1);
int damage = 1 + (damageLevel * 1);
```

## 🎯 Боевая система

### Механика
1. Игрок и враг атакуют по очереди
2. Урон зависит от характеристик и варьируется ±20%
3. Броня уменьшает входящий урон
4. Минимальный урон = 1

### Враг
- Уровень влияет на все характеристики
- Враги могут быть разного уровня
- Награда за победу = базовое золото + уровень

```csharp
maxHealth = 50 + (level * 10);
armor = level * 2;
damage = 5 + (level * 2);
goldReward = 50 + (level * 10);
```

## 🗺 Процедурная генерация

### Алгоритм
1. Инициализируем пустую карту (15x10 по умолчанию)
2. Начинаем с центра
3. Генерируем "змейку" пути случайными ходами
4. Каждая клетка на пути - это боевая локация
5. Гарантируем, что путь не выходит за границы

## 📝 Примеры использования

### Старт нового уровня
```csharp
LevelManager levelManager = GetComponent<LevelManager>();
// Карта генерируется и отображается автоматически в Start()
```

### Начало боя
```csharp
CombatSystem combat = GetComponent<CombatSystem>();
combat.StartCombat(currentEnemyLevel); // Бой выполняется автоматически

if (combat.IsPlayerAlive())
    Debug.Log("Победа!");
```

### Получение карточки
```csharp
LocationCard card = locationCardConfig.GetRandomCard();
levelManager.PlaceLocationCard(x, y, card);
```

### Сохранение прогресса
```csharp
GameManager.Instance.AddCoins(100);
GameManager.Instance.NextLevel();
GameManager.Instance.SaveGame();
```

## ⚙️ Параметры для настройки

### MapGenerator (в Inspector)
- `mapWidth` - Ширина карты (клетки)
- `mapHeight` - Высота карты (клетки)
- `tileSize` - Размер одного тайла (пиксели)
- `pathLength` - Количество боевых локаций

### CombatSystem
- Вероятность попадания: 80-120% от базового урона
- Минимальный урон: 1

### LocationCardConfig
- `dropChance` для каждой карточки (0.0 - 1.0)
- Сумма всех шансов должна быть = 1.0

## 🐛 TODO для доработки

- [ ] Визуализация боя в реальном времени
- [ ] Система частиц и эффектов
- [ ] Звуковые эффекты и музыка
- [ ] Анимации персонажей
- [ ] Система уровней сложности
- [ ] Лидерборд
- [ ] Мобильная поддержка
- [ ] Локализация на русский язык

## 📦 Зависимости

- Unity 2021.3+ (или новее)
- TextMeshPro (встроено в Unity)

## 🔗 Навигация между сценами

```csharp
SceneController.LoadMainMenu();      // Главное меню
SceneController.LoadLevel();         // Уровень
SceneController.LoadSettings();      // Настройки
SceneController.LoadUpgrades();      // Улучшения
SceneController.QuitGame();          // Выход
```

## 💾 Формат сохраняемых данных

Сохраняется в формате JSON в `Application.persistentDataPath`:

```json
{
  "upgrades": {
    "hpLevel": 2,
    "armorLevel": 1,
    "damageLevel": 3
  },
  "settings": {
    "screenWidth": 1920,
    "screenHeight": 1080,
    "screenMode": 3
  },
  "currentLevel": 5,
  "totalCoins": 250
}
```

---

**Создано для Loop Hero Clone на Unity**
