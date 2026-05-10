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
    [SerializeField] private CardBonusSystem cardBonusSystem;

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
                bool placed = mapGenerator.AutoPlaceCard(card);  // Теперь передаем полную карточку
                if (placed)
                {
                    // Применяем бонусы от карточки к игроку
                    if (cardBonusSystem != null && combatSystem != null)
                    {
                        Character player = combatSystem.GetPlayer();
                        if (player != null)
                        {
                            cardBonusSystem.ApplyBonusesToCharacter(player);
                            Debug.Log($"LevelManager: Бонусы от карточки '{card.displayName}' применены к игроку");
                        }
                    }
                }
            }
        }
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

