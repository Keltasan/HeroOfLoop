using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Система автоматического боя с пошаговыми ходами
/// </summary>
public class CombatSystem : MonoBehaviour
{
    public delegate void CombatEventHandler(string message);
    public event CombatEventHandler OnCombatLog;
    public event CombatEventHandler OnCombatEnd;

    private Character player;
    private Enemy currentEnemy;
    private List<string> combatLog;
    private bool inCombat = false;
    private bool isPaused = false;
    private Coroutine combatCoroutine;
    private int round = 0;
    private const float ACTION_DELAY = 0.5f; // Задержка между ходами

    private void Start()
    {
        combatLog = new List<string>();
    }

    public void StartCombat(int enemyLevel)
    {
        if (inCombat)
            return;

        inCombat = true;
        combatLog.Clear();
        round = 0;

        // Инициализируем игрока
        int playerMaxHealth = 100;
        int playerCurrentHealth = 100;
        int playerArmor = 5;
        int playerDamage = 10;

        // Пытаемся получить данные из GameManager
        if (GameManager.Instance != null)
        {
            GameData gameData = GameManager.Instance.GetGameData();
            if (gameData != null)
            {
                playerMaxHealth = gameData.upgrades.GetMaxHealth();
                playerArmor = gameData.upgrades.GetArmor();
                playerDamage = gameData.upgrades.GetDamage();
                
                // Проверяем сохраненный HP
                if (gameData.currentPlayerHealth > 0)
                {
                    playerCurrentHealth = Mathf.Min(gameData.currentPlayerHealth, playerMaxHealth);
                    Debug.Log($"CombatSystem: Используется сохраненный HP игрока: {playerCurrentHealth}/{playerMaxHealth}");
                }
                else
                {
                    playerCurrentHealth = playerMaxHealth;
                    Debug.Log($"CombatSystem: Новый бой, полное восстановление HP: {playerMaxHealth}");
                }
            }
            else
            {
                Debug.LogWarning("CombatSystem: GameData null! Используются значения по умолчанию");
            }
        }
        else
        {
            Debug.LogWarning("CombatSystem: GameManager.Instance null! Используются значения по умолчанию");
        }

        player = new Character(
            "Игрок",
            playerMaxHealth,
            playerArmor,
            playerDamage
        );
        
        // Устанавливаем текущий HP (может быть ниже максимума)
        player.currentHealth = playerCurrentHealth;

        // Создаем врага
        currentEnemy = new Enemy(enemyLevel);

        AddLog($"=== Battle Started! ===");
        AddLog($"Player: {player.maxHealth} HP, {player.armor} Armor, {player.damage} Damage");
        AddLog($"Enemy: {currentEnemy.maxHealth} HP, {currentEnemy.armor} Armor, {currentEnemy.damage} Damage");

        // Начинаем боевые ходы
        StartCoroutine(ExecuteCombatRoutine());
    }

    private IEnumerator ExecuteCombatRoutine()
    {
        while (player.IsAlive() && currentEnemy.IsAlive() && round < 100)
        {
            // Проверяем паузу
            while (isPaused)
                yield return new WaitForSeconds(0.1f);
            
            round++;
            AddLog($"\n--- Round {round} ---");
            
            // Ход игрока
            yield return StartCoroutine(PlayerTurnRoutine());
            
            if (!currentEnemy.IsAlive())
                break;
            
            // Проверяем паузу перед ходом врага
            while (isPaused)
                yield return new WaitForSeconds(0.1f);
            
            yield return new WaitForSeconds(ACTION_DELAY);
            
            // Ход врага
            yield return StartCoroutine(EnemyTurnRoutine());
            
            yield return new WaitForSeconds(ACTION_DELAY);
        }

        EndCombat();
    }

    private IEnumerator PlayerTurnRoutine()
    {
        int playerDamage = player.GetDamage();
        currentEnemy.TakeDamage(playerDamage);
        AddLog($"Player deals {playerDamage} damage! Enemy HP: {currentEnemy.currentHealth}/{currentEnemy.maxHealth}");
        yield return new WaitForSeconds(ACTION_DELAY);
    }

    private IEnumerator EnemyTurnRoutine()
    {
        int enemyDamage = currentEnemy.GetDamage();
        player.TakeDamage(enemyDamage);
        AddLog($"Enemy deals {enemyDamage} damage! Player HP: {player.currentHealth}/{player.maxHealth}");
        yield return new WaitForSeconds(ACTION_DELAY);
    }

    private void EndCombat()
    {
        bool playerWon = player.IsAlive();
        
        if (playerWon)
        {
            AddLog($"\nVictory! Enemy defeated!");
            AddLog($"Reward: {currentEnemy.goldReward} gold");
            
            // Добавляем награду если GameManager существует
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(currentEnemy.goldReward);
            }
        }
        else
        {
            AddLog($"\nDefeat! You've been defeated!");
        }

        inCombat = false;
        OnCombatEnd?.Invoke(playerWon ? "Victory!" : "Defeat!");
    }

    private void AddLog(string message)
    {
        combatLog.Add(message);
        OnCombatLog?.Invoke(message);
        Debug.Log($"[COMBAT LOG] {message}");
    }

    public void PauseCombat()
    {
        if (inCombat)
        {
            isPaused = true;
            AddLog("[PAUSED]");
        }
    }

    public void ResumeCombat()
    {
        if (inCombat && isPaused)
        {
            isPaused = false;
            AddLog("[RESUMED]");
        }
    }

    public bool IsInCombat() => inCombat;
    public bool IsPaused() => isPaused;
    public bool IsPlayerAlive() => player?.IsAlive() ?? false;
    public List<string> GetCombatLog() => new List<string>(combatLog);
    public Character GetPlayer() => player;
    public Enemy GetCurrentEnemy() => currentEnemy;
}
