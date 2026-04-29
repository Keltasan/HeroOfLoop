# Setup Guide для Hero of Loop

Пошаговое руководство по настройке проекта в Unity.

## 1. Создание Сцен

Создайте 4 новые сцены в папке `Assets/Scenes/`:
- MainMenu.unity
- Upgrades.unity
- Settings.unity
- Level.unity

## 2. Настройка MainMenu Сцены

### Иерархия
```
MainMenu (Scene)
├── Canvas
│   ├── NewGameButton
│   ├── UpgradesButton
│   ├── SettingsButton
│   └── ExitButton
│   └── LevelText
├── GameManager (префаб с синглтоном)
```

### Шаги

1. **Создайте Canvas**
   - Right-click в иерархии → UI → Panel
   - Переименуйте в "Canvas"

2. **Добавьте кнопки**
   - В Canvas добавьте 4 Button - TextMeshPro:
     - NewGameButton (текст: "Новая игра")
     - UpgradesButton (текст: "Улучшения")
     - SettingsButton (текст: "Настройки")
     - ExitButton (текст: "Выйти из игры")

3. **Добавьте TextMeshProUGUI для уровня**
   - В Canvas добавьте TextMeshProUGUI
   - Переименуйте: LevelText

4. **Добавьте GameManager**
   - Создайте новый пустой GameObject: "GameManager"
   - Добавьте компоненты:
     - GameManager (скрипт)
     - SaveManager (скрипт)
   - Присоедините MainMenuController к Canvas

5. **Присоедините скрипты**
   - Canvas → добавьте компонент MainMenuController
   - MainMenuController → установите ссылки:
     - New Game Button → NewGameButton
     - Upgrades Button → UpgradesButton
     - Settings Button → SettingsButton
     - Exit Button → ExitButton
     - Level Text → LevelText

## 3. Настройка Upgrades Сцены

### Иерархия
```
Upgrades (Scene)
├── Canvas
│   ├── HPUpgradeGroup
│   │   ├── IncreaseButton
│   │   ├── LevelText
│   │   └── CostText
│   ├── ArmorUpgradeGroup
│   │   ├── IncreaseButton
│   │   ├── LevelText
│   │   └── CostText
│   ├── DamageUpgradeGroup
│   │   ├── IncreaseButton
│   │   ├── LevelText
│   │   └── CostText
│   ├── CoinsText
│   ├── BackButton
```

### Шаги

1. **Создайте структуру UI**
   - Canvas с разделами для каждого улучшения
   - Каждый раздел: Button + Text (уровень) + Text (стоимость)

2. **Добавьте UpgradesController**
   - Canvas → добавьте компонент UpgradesController
   - Установите ссылки в инспекторе:
     - HP Upgrade (структура):
       - Increase Button → HPUpgradeButton
       - Level Text → HPLevelText
       - Cost Text → HPCostText
     - Armor Upgrade (аналогично)
     - Damage Upgrade (аналогично)
     - Coins Text → CoinsText
     - Back Button → BackButton

## 4. Настройка Settings Сцены

### Иерархия
```
Settings (Scene)
├── Canvas
│   ├── ScreenModeDropdown
│   ├── ResolutionDropdown
│   ├── MasterVolumeSlider
│   ├── MusicVolumeSlider
│   ├── SFXVolumeSlider
│   ├── ResetProgressButton
│   ├── BackButton
```

### Шаги

1. **Создайте Dropdowns**
   - Canvas → UI → Dropdown - TextMeshPro (Screen Mode)
   - Canvas → UI → Dropdown - TextMeshPro (Resolution)
   - В Screen Mode добавьте опции:
     - FullScreenMode.Fullscreen
     - FullScreenMode.FullScreenWindow
     - FullScreenMode.Windowed
     - FullScreenMode.MaximizedWindow

2. **Добавьте Sliders**
   - Canvas → UI → Slider (Master Volume)
   - Canvas → UI → Slider (Music Volume)
   - Canvas → UI → Slider (SFX Volume)
   - Установите Min: 0, Max: 1

3. **Добавьте Buttons**
   - Reset Progress Button
   - Back Button

4. **Присоедините SettingsController**
   - Canvas → добавьте компонент SettingsController
   - Установите все ссылки в инспекторе

## 5. Настройка Level Сцены

### Иерархия
```
Level (Scene)
├── World
│   └── TilesContainer
├── UI
│   ├── Canvas
│   │   ├── CombatLog
│   │   ├── PlayerStats
│   │   ├── EnemyStats
│   │   └── ContinueButton
├── GameManagers
│   ├── MapGenerator
│   ├── MapRenderer
│   ├── LevelManager
│   └── CombatSystem
```

### Шаги

1. **Создайте контейнер для тайлов**
   - Создайте пустой GameObject: "World"
   - В World создайте: "TilesContainer" (пустой GameObject)

2. **Создайте Tile Префаб**
   - Создайте новый GameObject: "Tile"
   - Добавьте SpriteRenderer компонент
   - Добавьте Sprite (используйте белый квадрат или простой спрайт)
   - Сохраните как префаб в Assets/Prefabs/

3. **Добавьте MapGenerator**
   - Создайте пустой GameObject: "MapGenerator"
   - Добавьте компонент MapGenerator (скрипт)
   - Установите параметры:
     - Map Width: 15
     - Map Height: 10
     - Tile Size: 64
     - Path Length: 20

4. **Добавьте MapRenderer**
   - Создайте пустой GameObject: "MapRenderer"
   - Добавьте компонент MapRenderer (скрипт)
   - Установите ссылки:
     - Map Generator → MapGenerator объект
     - Tiles Container → TilesContainer
     - Tile Prefab → Tile префаб

5. **Добавьте LevelManager**
   - Создайте пустой GameObject: "LevelManager" (в GameManagers)
   - Добавьте компонент LevelManager (скрипт)
   - **ВАЖНО: Установите ссылки в инспекторе:**
     - Map Generator → выберите объект "MapGenerator"
     - Map Renderer → выберите объект "MapRenderer"
     - Location Card Config → найдите Assets/Data/LocationCardConfig.asset
     - Combat System → выберите объект "CombatSystem"
   
   **Проверка в Console:**
   - Запустите уровень (нажмите Play)
   - В Console должны быть сообщения:
     ```
     LevelManager: Start() вызван
     LevelManager: Генерирую карту...
     LevelManager: Карта отрендерена
     LevelManager: Загружено X карточек
     ```
   - Если видите ошибки - проверьте что все ссылки установлены!

6. **Добавьте CombatSystem**
   - Создайте пустой GameObject: "CombatSystem"
   - Добавьте компонент CombatSystem (скрипт)

7. **Создайте UI для боя**
   - Canvas с текстовыми полями:
     - CombatLog (TextMeshProUGUI)
     - PlayerStats (TextMeshProUGUI)
     - EnemyStats (TextMeshProUGUI)
   - ContinueButton (Button)
   - Canvas → добавьте компонент CombatUIController
   - Установите ссылки

## 6. Создание LocationCardConfig

### Что такое LocationCardConfig?

**LocationCardConfig** - это ScriptableObject (конфигурационный файл Unity), который хранит информацию о всех доступных локациях (карточках) в игре. Каждая карточка имеет:
- **Имя локации** (Battle, Forest, Mountain и т.д.)
- **Вероятность выпадения** (dropChance) - шанс, что эта карточка выпадет в награду за бой
- **Спрайт** (опционально) - рисунок локации
- **Тип локации** - категория для игровой логики

### Структура данных

**LocationCard.cs** - класс для одной карточки:
```csharp
[System.Serializable]
public class LocationCard
{
    public string locationName;        // "Battle", "Forest", "Mountain"...
    public float dropChance;           // 0.15, 0.20 и т.д. (в сумме должны быть ~1.0)
    public Sprite locationSprite;      // Спрайт карточки
    public LocationType locationType;  // Battle, Treasure, Modifier...
}

public enum LocationType
{
    Battle,
    Treasure,
    Modifier,
    Empty
}
```

**LocationCardConfig.cs** - контейнер конфигурации:
```csharp
public class LocationCardConfig : ScriptableObject
{
    public List<LocationCard> cards = new List<LocationCard>();
    
    // Получить случайную карточку по вероятности
    public LocationCard GetRandomCard()
    {
        float random = Random.value;
        float cumulative = 0f;
        
        foreach (LocationCard card in cards)
        {
            cumulative += card.dropChance;
            if (random <= cumulative)
                return card;
        }
        
        return cards[cards.Count - 1]; // Последняя карточка по умолчанию
    }
    
    // Получить карточку по имени
    public LocationCard GetCardByName(string name)
    {
        return cards.Find(c => c.locationName == name);
    }
}
```

### Пошаговое создание

1. **Создайте скрипт LocationCard.cs**
   - In Assets/Scripts создайте новый файл
   - Добавьте код выше (класс LocationCard с enum LocationType)

2. **Создайте скрипт LocationCardConfig.cs**
   - In Assets/Scripts создайте новый файл
   - Добавьте код контейнера конфигурации

3. **Создайте файл конфигурации**
   - Right-click в папке Assets/Data
   - Create → ScriptableObject → LocationCardConfig
   - Переименуйте: LocationCardConfig

4. **Настройте карточки в инспекторе**
   - Откройте LocationCardConfig в инспекторе
   - Нажмите "+" чтобы добавить новый элемент
   - Добавьте 8 элементов с параметрами:

| Локация | dropChance | Смысл |
|---------|-----------|-------|
| Battle | 0.15 | Враги (15% шанс) |
| Forest | 0.20 | Лес дает бонусы (20% - самая крутая) |
| Mountain | 0.15 | Гора (15%) |
| Cemetery | 0.12 | Кладбище (12%) |
| Library | 0.12 | Библиотека (12%) |
| Shop | 0.10 | Магазин (10%) |
| Mansion | 0.10 | Особняк (10%) |
| Empty | 0.06 | Пусто (6% - используется если ничего другое) |

**Важно:** Сумма всех dropChance должна быть приблизительно 1.0 (100%)

### Интеграция с системой

**CardDropSystem.cs** использует LocationCardConfig:
```csharp
public class CardDropSystem : MonoBehaviour
{
    public LocationCardConfig locationConfig;
    
    public void DropRandomCard()
    {
        LocationCard droppedCard = locationConfig.GetRandomCard();
        Debug.Log("Выпала карточка: " + droppedCard.locationName);
        // Добавить карточку в инвентарь игрока
    }
}
```

### Использование в LevelManager

```csharp
public class LevelManager : MonoBehaviour
{
    public LocationCardConfig cardConfig;
    
    void OnBattleWon()
    {
        LocationCard reward = cardConfig.GetRandomCard();
        // Показать предложение разместить карточку на карте
    }
}
```

### Практический пример

После боя:
1. LevelManager вызывает `cardConfig.GetRandomCard()`
2. Система случайно выбирает одну из 8 локаций (например, Forest с 20% шансом)
3. Игроку показывается спрайт Forest карточки
4. Игрок может разместить её на карте
5. После размещения Forest дает бонусы (+1 HP каждый ход)

## 7. Создание GameManager Префаба

1. **В MainMenu сцене найдите GameManager объект**
2. **Перетащите его в папку Assets/Prefabs**
3. **Переименуйте: GameManager**
4. **Добавьте его в остальные сцены**
   - Откройте каждую сцену
   - Drag & Drop GameManager префаб в каждую

## 8. Финальная проверка

Проверьте, что все сцены добавлены в Build Settings:
1. File → Build Settings
2. Добавьте сцены в правильном порядке:
   - 0: MainMenu
   - 1: Upgrades
   - 2: Settings
   - 3: Level

## 9. Тестирование

1. **Запустите MainMenu сцену**
2. **Проверьте кнопки:**
   - Continue → загружает Level
   - Upgrades → загружает Upgrades
   - Settings → загружает Settings
   - Exit → закрывает игру

3. **На Upgrades:**
   - Проверьте отображение уровней
   - Попробуйте купить улучшение

4. **На Settings:**
   - Проверьте изменение разрешения
   - Проверьте громкость

5. **На Level:**
   - Карта должна отобразиться
   - Бой должен запуститься автоматически

## 10. Финальная Настройка Боевой Системы

### Добавить CardDropSystem

1. **Откройте сцену Level**
2. **В иерархии нажмите на GameManagers**
3. **В Inspector нажмите Add Component**
4. **Выберите CardDropSystem**
5. **В поле Location Card Config перетащите Assets/Data/LocationCardConfig.asset**

### Обновить CombatUIController Ссылки

1. **Найдите Canvas в иерархии**
2. **Нажмите на Canvas → в Inspector посмотрите CombatUIController**
3. **Установите ссылки:**
   - Combat System → GameManagers (должна быть уже)
   - Card Drop System → GameManagers > CardDropSystem (выберите из иерархии)
   - Level Manager → GameManagers > LevelManager
   - Combat Log Text → выберите TextMeshPro поле для логов
   - Player Stats Text → выберите TextMeshPro для статистики игрока
   - Enemy Stats Text → выберите TextMeshPro для статистики врага
   - Continue Button → выберите кнопку Continue
   - Card Reward Text → создайте новый TextMeshPro элемент или используйте существующий

### Добавить CardRewardText в Canvas (если его нет)

1. **Right-click на Canvas → TextMeshPro - Text**
2. **Переименуйте в "CardRewardText"**
3. **Установите позицию и размер для отображения награды**
4. **В CombatUIController установите эту ссылку в поле Card Reward Text**

## 11. Финальная Проверка

### Тест Полного Цикла

1. **Запустите сцену MainMenu**
2. **Нажмите "Новая игра"** → должна загрузиться Level сцена с картой
3. **Посмотрите Console** → должны быть логи:
   ```
   LevelManager: Start() вызван
   LevelManager: Генерирую карту...
   LevelManager: Карта отрендерена
   LevelManager: Загружено 8 карточек
   LevelManager: Начинаю первый бой
   CombatSystem: === Battle Started! ===
   ```

4. **Смотрите как начинается бой:**
   - Ходы должны выполняться пошагово (с задержкой 0.5 сек между ними)
   - Combat Log должен обновляться в реальном времени
   - Player Stats и Enemy Stats должны обновляться после каждого хода

5. **После боя:**
   - Continue кнопка должна стать активной
   - Должна отображаться выпавшая карточка (CardRewardText)
   - Нажав Continue → начинается следующий бой (враг будет сильнее)

6. **После 5-ти побед:**
   - Должны вернуться в MainMenu
   - Монеты должны добавиться в сохранение
   - Уровень должен увеличиться

## 12. Если что-то не работает

### Combat не запускается
- Проверьте что у CombatSystem есть ссылка на Character и Enemy классы
- В Console посмотрите ошибки (красные сообщения)
- Убедитесь что combatSystem не null в LevelManager

### Continue кнопка не работает
- Откройте Canvas
- Нажмите на Continue Button → в Inspector посмотрите OnClick() события
- Должно быть событие на ContinueAfterCombat

### Карточки не выпадают
- Проверьте что CardDropSystem имеет ссылку на LocationCardConfig
- В Console должны быть логи "CardDropSystem: Выпала карточка..."
- Убедитесь что combatSystem.IsPlayerAlive() = true после боя

---

## ✅ Готово! Проект завершен на 70%

**Осталось сделать:**
- ⏳ Система размещения карточек на карте (drag-n-drop UI)
- ⏳ Бонусы от размещенных карточек
- ⏳ Система волн врагов (сложность растет)
- ⏳ Улучшенная графика
- ⏳ Звуки и музыка
- ⏳ Полирование UI

**Основной функционал:**
✅ Главное меню с кнопками
✅ Система улучшений (HP, Armor, DMG)
✅ Система настроек (разрешение, звук, прогресс)
✅ Процедурная генерация карты
✅ Боевая система (пошаговая)
✅ Выпадение карточек после боя
✅ Система сохранения/загрузки

---

**Для вопросов смотрите README.md**

### "Can't load scene with name"
- Убедитесь, что сцены добавлены в Build Settings
- Проверьте орфографию названий сцен

### NullReferenceException
- Проверьте, что все ссылки установлены в инспекторе
- Убедитесь, что требуемые объекты существуют в сцене

### Кнопки не работают
- Убедитесь, что OnClick() события добавлены к кнопкам
- Проверьте, что скрипты присоедены к правильным объектам

### Карта не генерируется при запуске уровня

**Проблема:** При клике "New Game" в главном меню загружается Level сцена, но карта не отображается, текст остается старым.

**Причины и решения:**

#### 1️⃣ LevelManager не привязан правильно
1. Откройте сцену **Level**
2. В иерархии внизу найдите объект **GameManagers/LevelManager**
3. В Inspector посмотрите на компонент **LevelManager (Script)**
4. Проверьте что **все 4 поля заполнены** (не пусты):
   - Map Generator (красная подсветка = ошибка!)
   - Map Renderer (красная подсветка = ошибка!)
   - Location Card Config (красная подсветка = ошибка!)
   - Combat System (может быть пусто если боя нету)

**Как исправить:**
- Если поле Map Generator пусто → перетащите объект **GameManagers/MapGenerator** в это поле
- Если поле Map Renderer пусто → перетащите объект **GameManagers/MapRenderer** в это поле
- Если поле Location Card Config пусто → найдите **Assets/Data/LocationCardConfig.asset** и перетащите его

#### 2️⃣ MapRenderer не имеет ссылки на Tile Prefab
1. Найдите объект **GameManagers/MapRenderer**
2. В Inspector на компоненте MapRenderer посмотрите поле **Tile Prefab**
3. Если оно пусто (красное) - нужно создать Tile префаб:
   - Создайте пустой GameObject с именем "Tile"
   - Добавьте компонент **SpriteRenderer**
   - В SpriteRenderer добавьте **Sprite** (используйте Sprites/Square или любой простой спрайт)
   - Перетащите Tile в папку **Assets/Prefabs/**
   - В MapRenderer перетащите этот префаб в поле **Tile Prefab**

#### 3️⃣ TilesContainer не назначен
1. Найдите объект **GameManagers/MapRenderer**
2. В Inspector поле **Tiles Container** должно быть заполнено
3. Если оно пусто:
   - Найдите в иерархии **World/TilesContainer**
   - Перетащите этот объект в поле **Tiles Container** у MapRenderer

#### 4️⃣ Проверяйте Console (Ctrl+Shift+C)
**Правильные сообщения:**
```
LevelManager: Start() вызван
LevelManager: Генерирую карту...
LevelManager: Карта отрендерена
LevelManager: Загружено 8 карточек
```

**Ошибки:**
```
LevelManager: MapGenerator не назначен! → Назначьте MapGenerator в LevelManager
LevelManager: MapRenderer не назначен! → Назначьте MapRenderer в LevelManager
LevelManager: LocationCardConfig не назначен! → Назначьте LocationCardConfig в LevelManager
```

#### 6️⃣ Карта генерируется но НЕ видна на экране!

**Проблема:** Console логирует "Карта отрендерена" и "Создано 150 тайлов", но в Game окне ничего не видно.

**Проверка 1: Посмотрите в Console на новые логи**

После запуска должны быть сообщения:
```
MapRenderer: TilesContainer позиция: (478.7, 227.8, 0)
MapRenderer: Camera позиция: (X, Y, Z)
```

**Если TilesContainer имеет странную позицию (не 0,0,0):**
- Найдите **World/TilesContainer** в иерархии
- В Inspector справа вверху нажмите на 3 точки → Reset (на Transform)
- Убедитесь что Position = (0, 0, 0)

**Проверка 2: Camera должна смотреть в центр карты**

- Карта генерируется от (0,0) до (15*64, 10*64) = примерно (0,0) до (960, 640)
- Main Camera должна смотреть туда же

**Как исправить:**
1. Найдите объект **Main Camera** в иерархии
2. Установите его Position примерно на (480, 320, -10)
   - X: половина ширины карты (15*64/2)
   - Y: половина высоты карты (10*64/2) 
   - Z: -10 (чтобы камера была перед объектами)

**Проверка 3: Tile Prefab должен быть маленьким**

- Найдите **Assets/Prefabs/Tile** префаб
- Откройте его двойным кликом
- Посмотрите его Scale в Transform - должно быть (1, 1, 1)
- Спрайт должен быть примерно 64x64 пиксела (или размер вашего Tile Size)

**Проверка 4: UI Canvas должен быть ВВЕРХУ всего**

- В иерархии уровня найдите **Canvas**
- Убедитесь что Canvas находится **ПОСЛЕ** World/TilesContainer в иерархии
  - Если Canvas выше World - тайлы будут закрыты!
  - Перетащите Canvas вниз, чтобы он был последним объектом

**Проверка 5: Если всё ещё не видно - включите Grid**

В сцене уровня:
1. Scene окно (слева вверху)
2. В верхнем меню нажмите **Gizmos**
3. Включите **Grid** - появится сетка
4. Нажмите Play - посмотрите где появляются тайлы

### LocationCardConfig не видно в инспекторе LevelManager

**Проблема:** При добавлении поля `public LocationCardConfig cardConfig;` в LevelManager скрипт, оно не появляется в инспекторе.

**Решение:** Разделите enum и класс на отдельные файлы:

**LocationType.cs** (новый файл):
```csharp
using UnityEngine;

public enum LocationType
{
    Empty,
    Forest,
    Mountain,
    Cemetery,
    Library,
    Shop,
    Mansion,
    Battle
}
```

**LocationCard.cs** (только класс, без enum):
```csharp
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LocationCard
{
    public LocationType type;
    public string displayName;
    public Sprite icon;
    public float dropChance = 0.1f;
    public int hpBonus;
    public int damageBonus;
    public int armorBonus;
}

[CreateAssetMenu(fileName = "LocationCardConfig", menuName = "Game/Location Card Config")]
public class LocationCardConfig : ScriptableObject
{
    [SerializeField] private List<LocationCard> availableCards = new List<LocationCard>();
    // ... методы ...
}
```

**Ошибка:** `'LocationCard' is missing the class attribute 'ExtensionOfNativeClass'!`

**Причина:** Unity не может сериализовать класс когда enum и класс находятся в одном файле.

**Исправление:**
1. Закройте Unity полностью
2. Создайте файл `LocationType.cs` с enum (см. выше)
3. Оставьте только класс `LocationCard` в `LocationCard.cs`
4. Удалите папку `Library/` в проекте (пересоздастся автоматически)
5. Откройте проект заново в Unity
6. Дождитесь "Compiling..." внизу экрана
7. Проверьте что ошибка исчезла в Console

Причины и решения:

#### 1️⃣ Скрипт не скомпилировался
- Откройте Console (Ctrl+Shift+C)
- Если там красные ошибки - исправьте их
- После исправления Unity переcompilирует автоматически
- Если ошибок нет - попробуйте `Assets → Reimport All`

#### 2️⃣ Поле неPublic
Убедитесь что в LevelManager.cs написано правильно:
```csharp
public class LevelManager : MonoBehaviour
{
    public LocationCardConfig cardConfig;  // ✅ ПРАВИЛЬНО - public
    
    // ❌ НЕПРАВИЛЬНО - private
    // private LocationCardConfig cardConfig;
}
```

Если нужно скрыть от инспектора, используйте:
```csharp
[SerializeField] private LocationCardConfig cardConfig;
```

#### 3️⃣ LocationCardConfig не наследует ScriptableObject
Проверьте LocationCardConfig.cs:
```csharp
// ✅ ПРАВИЛЬНО
public class LocationCardConfig : ScriptableObject
{
    public List<LocationCard> cards = new List<LocationCard>();
}

// ❌ НЕПРАВИЛЬНО
public class LocationCardConfig : MonoBehaviour  // Это неправильный класс!
{
    ...
}
```

#### 4️⃣ Unity не перегрузил домен
Попробуйте:
1. Сохраните все файлы (Ctrl+S)
2. Вернитесь в Unity
3. Дождитесь "Compiling..." внизу экрана
4. Нажмите на объект LevelManager в иерархии снова

#### 5️⃣ Два скрипта с одинаковым именем
- Проверьте что нет двух файлов `LocationCardConfig.cs`
- Удалите дубликаты если они есть

#### 6️⃣ Неправильно подключен нэймспейс
Если LocationCardConfig в другой папке, может нужен using:
```csharp
using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public LocationCardConfig cardConfig;  // Unity должна найти этот класс
}
```

**Что делать пошагово:**

1. ✅ Закройте Unity полностью
2. ✅ Откройте папку проекта и удалите папку `Library/` (она пересоздастся)
3. ✅ Откройте проект заново
4. ✅ Дождитесь "Compiling..." 
5. ✅ Нажмите на LevelManager объект в сцене
6. ✅ Посмотрите в Inspector - поле должно появиться

**Если всё ещё не видно:**
- Посмотрите в Console на ошибки (красные сообщения)
- Пришлите скрины LevelManager.cs и LocationCardConfig.cs
- Проверьте что оба файла в папке Assets/Scripts/

---

**Для вопросов смотрите README.md**
