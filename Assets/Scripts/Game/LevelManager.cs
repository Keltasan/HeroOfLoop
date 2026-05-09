using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Менеджер уровня - управляет процессом игры, картой, врагами и наградами
/// </summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private MapRenderer mapRenderer;
    [SerializeField] private LocationCardConfig locationCardConfig;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private CardDropSystem cardDropSystem;
    [SerializeField] private PlayerMovement playerMovement;

    private List<LocationCard> availableCards;
    private int battlesWon = 0;
    private int totalCoinsThisLevel = 0;
    private int currentEnemyLevel = 1;
    private bool levelComplete = false;

    private void Start()
    {
        Debug.Log("LevelManager: Start() вызван");
        
        // Инициализируем Random с новым seed для каждой генерации карты
        int seed = (int)System.DateTime.Now.Ticks;
        Random.InitState(seed);
        Debug.Log($"LevelManager: Random seed инициализирован: {seed}");
        
        if (mapGenerator == null)
        {
            Debug.LogError("LevelManager: MapGenerator не назначен!");
            return;
        }
        
        if (mapRenderer == null)
        {
            Debug.LogError("LevelManager: MapRenderer не назначен!");
            return;
        }

        Debug.Log("LevelManager: Генерирую карту...");
        mapRenderer.RenderMap();
        Debug.Log("LevelManager: Карта отрендерена");

        // Инициализируем движение игрока
        if (playerMovement != null)
        {
            List<Vector2Int> pathSequence = mapGenerator.GetPathSequence();
            playerMovement.InitializePathSequence(pathSequence);
            Debug.Log("LevelManager: PlayerMovement инициализирован");
        }
        else
        {
            Debug.LogError("LevelManager: PlayerMovement не назначен!");
        }

        if (locationCardConfig != null)
        {
            availableCards = locationCardConfig.GetAllCards();
            Debug.Log($"LevelManager: Загружено {availableCards.Count} карточек");
        }
        else
        {
            Debug.LogError("LevelManager: LocationCardConfig не назначен!");
        }

        // Боя начинаются автоматически когда игрок наступает на боевую клетку (управляется PlayerMovement)
        // Нет необходимости в StartFirstBattle()
    }

    public void OnBattleWon()
    {
        battlesWon++;
        currentEnemyLevel++;
        totalCoinsThisLevel += 50; // Награда за победу
        
        Debug.Log($"LevelManager: Победа #{battlesWon}! Всего монет: {totalCoinsThisLevel}");

        // Сохраняем текущий HP игрока для следующего боя
        if (combatSystem != null && GameManager.Instance != null)
        {
            Character player = combatSystem.GetPlayer();
            if (player != null)
            {
                GameManager.Instance.GetGameData().currentPlayerHealth = player.currentHealth;
                GameManager.Instance.SaveGame(); // Сохраняем HP
                Debug.Log($"LevelManager: Сохранен HP игрока: {player.currentHealth} для следующего боя");
            }
        }

        // Даем рандомную карточку локации
        if (cardDropSystem != null)
        {
            LocationCard randomCard = cardDropSystem.DropRandomCard();
            ShowLocationCardReward(randomCard);
        }

        // Если прошли 5 врагов - конец уровня
        if (battlesWon >= 5)
        {
            CompleteLevel();
        }
        else
        {
            // Уведомляем PlayerMovement об окончании боя и продолжаем движение
            if (playerMovement != null)
            {
                playerMovement.OnCombatEnded();
            }
        }
    }

    private void ShowLocationCardReward(LocationCard card)
    {
        if (card != null)
        {
            Debug.Log($"Выпала карточка: {card.displayName}");
            
            // Автоматически выставляем карточку на карте
            if (mapGenerator != null && mapRenderer != null)
            {
                bool placed = mapGenerator.AutoPlaceCard(card.type);
                if (placed)
                {
                    // Обновляем визуализацию карты
                    Vector2Int cardPos = GetRandomCardPosition();
                    mapRenderer.UpdateTileVisualization(cardPos.x, cardPos.y, card.type);
                    Debug.Log($"LevelManager: Карточка '{card.displayName}' автоматически выставлена");
                }
            }
        }
    }
    
    /// <summary>
    /// Получает случайную позицию для выставления карточки
    /// </summary>
    private Vector2Int GetRandomCardPosition()
    {
        // Это вспомогательный метод для получения позиции выставленной карточки
        // Можно оптимизировать, если нужно
        Vector2Int startPos = mapGenerator.GetStartPosition();
        
        // Для упрощения возвращаем случайную позицию рядом со стартом
        // В реальном приложении можно отслеживать последнюю позицию выставления
        Vector2Int randomOffset = new Vector2Int(Random.Range(-2, 3), Random.Range(-2, 3));
        Vector2Int pos = startPos + randomOffset;
        
        // Убеждаемся, что позиция в границах карты
        pos.x = Mathf.Clamp(pos.x, 0, mapGenerator.GetMapWidth() - 1);
        pos.y = Mathf.Clamp(pos.y, 0, mapGenerator.GetMapHeight() - 1);
        
        return pos;
    }

    /*public void PlaceLocationCard(int x, int y, LocationCard card)
    {
        mapGenerator.PlaceLocationCard(x, y, card.type);
        
        // Применяем бонусы
        // TODO: Применить бонусы к статистике игрока
    }*/

    private void CompleteLevel()
    {
        levelComplete = true;
        Debug.Log($"LevelManager: Уровень завершён! Всего побед: {battlesWon}, монет: {totalCoinsThisLevel}");
        
        // Сбрасываем HP на максимум для нового уровня
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GetGameData().currentPlayerHealth = -1;
            Debug.Log("LevelManager: HP сброшен на максимум для нового уровня");
        }
        
        GameManager.Instance.AddCoins(totalCoinsThisLevel);
        GameManager.Instance.NextLevel();
        SceneController.LoadMainMenu();
    }

    public int GetBattlesWon() => battlesWon;
    public int GetTotalCoinsThisLevel() => totalCoinsThisLevel;
    public bool IsLevelComplete() => levelComplete;
}

