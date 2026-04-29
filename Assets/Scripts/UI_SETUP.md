# 🎮 UI Setup Instructions для Combat System

## 📋 Что нужно сделать в сцене Level.unity

### 1. Найти/Создать ScrollRect для логов боя

Логи теперь ограничены 20 строками и прокручиваются. Нужен ScrollRect!

**Если ScrollRect уже есть:**
1. Откройте иерархию сцены Level
2. Найдите элемент где находится combatLogText
3. Убедитесь что этот элемент находится ВНУТРИ ScrollRect

**Если ScrollRect нет - создайте:**
1. Right-click на Canvas → UI → ScrollView
2. Переименуйте в "CombatLogContainer"
3. Удалите Image компонент (если есть)
4. Установите размер (например Width=400, Height=300)
5. Установите позицию удобную для экрана

### 2. Переместить combatLogText внутрь ScrollRect

1. Найдите combatLogText в иерархии
2. Перетащите его ВНУТРь Viewport компонента ScrollRect
   - Путь: ScrollRect → Viewport → combatLogText
3. Установите размер combatLogText большим (чтобы был запас для прокрутки)

### 3. Обновить CombatUIController ссылки

1. Найдите Canvas в иерархии
2. На Canvas должен быть компонент **CombatUIController**
3. В Inspector установите поля:

```
Combat System: GameManagers → CombatSystem
Card Drop System: GameManagers → CardDropSystem  
Level Manager: GameManagers → LevelManager
Combat Log Text: Canvas → CombatLogContainer → Viewport → combatLogText
Combat Log Scroll: Canvas → CombatLogContainer (ScrollRect компонент)
Player Stats Text: [текстовое поле для статистики игрока]
Enemy Stats Text: [текстовое поле для статистики врага]
Continue Button: [кнопка Continue]
Card Reward Text: [текстовое поле для награды]
```

### 4. Настроить Continue Button

1. Найдите Continue Button в Canvas
2. На самой кнопке должен быть компонент **Button**
3. Проверьте что кнопка имеет TextMeshPro дочерний элемент с текстом
4. Убедитесь что текст изначально пуст (будет меняться динамически)

### 5. Проверить ScrollRect настройки

1. Выберите ScrollRect элемент
2. В Inspector установите:
   - **Vertical**: ✓ (галочка включена)
   - **Horizontal**: ☐ (галочка выключена)
   - **Vertical Scroll Bar**: Visibility = As Needed (или Auto Hide and Expand Viewport)

---

## 🧪 Тест

### Что должно происходить:

1. **Во время боя:**
   - Логи отображаются в UI (не только в Console)
   - Логи ограничены 20 строками
   - Если логов больше - они прокручиваются вверх
   - Кнопка показывает текст "Pause"
   - Нажатие на кнопку приостанавливает бой
   - После паузы текст меняется на "Continue"
   - Нажатие "Continue" возобновляет бой

2. **После боя:**
   - Кнопка показывает текст "Continue"
   - Отображается выпавшая карточка
   - Нажатие "Continue" начинает СЛЕДУЮЩИЙ бой

3. **После 5-ти побед:**
   - Возврат в главное меню
   - Монеты добавлены в сохранение

---

## ❓ Если что-то не работает

### Логи не отображаются в UI
- Проверьте что combatLogText назначен в CombatUIController
- Проверьте что combatLogScroll назначен
- В Console должны быть логи `[COMBAT LOG]` - если их нет, проверьте CombatSystem

### Кнопка не меняет текст
- Убедитесь что Continue Button имеет TextMeshPro дочерний элемент
- Текст должен быть в компоненте TextMeshProUGUI на дочернем элементе

### Следующий бой не начинается
- Проверьте Console логи
- Должно быть сообщение "LevelManager: Начинаю первый бой" для первого боя
- Должно быть сообщение "LevelManager: Победа #1" когда побеждаете врага

### Пауза не работает
- Убедитесь что combatSystem.IsPaused() и combatSystem.IsInCombat() вызываются
- В Console должны быть логи "[PAUSED]" и "[RESUMED]"

---

## 📝 Готово!

Теперь система боя полностью функциональна! 🎉
