using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UI контроллер для отображения боя
/// </summary>
public class CombatUIController : MonoBehaviour
{
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private CardDropSystem cardDropSystem;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private TextMeshProUGUI combatLogText;
    [SerializeField] private TextMeshProUGUI playerStatsText;
    [SerializeField] private TextMeshProUGUI enemyStatsText;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI cardRewardText;
    [SerializeField] private ScrollRect combatLogScroll; // Для прокрутки логов

    private LocationCard droppedCard;
    private bool combatEnded = false;
    private bool combatStarted = false;
    private int logLineCount = 0;
    private const int MAX_LOG_LINES = 20; // Максимум строк в логе

    private void OnEnable()
    {
        // Очищаем логи при включении
        if (combatLogText != null)
            combatLogText.text = "";
        logLineCount = 0;
        combatEnded = false;
        combatStarted = false;
        
        if (combatSystem != null)
        {
            combatSystem.OnCombatLog += UpdateCombatLog;
            combatSystem.OnCombatEnd += OnCombatEnd;
            Debug.Log("CombatUIController: Subscribed to CombatSystem events");
        }
        else
        {
            Debug.LogError("CombatUIController: CombatSystem not assigned!");
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
            continueButton.gameObject.SetActive(false); // Изначально скрыта, активируется при начале боя
            UpdateButtonText();
        }
    }

    private void OnDisable()
    {
        if (combatSystem != null)
        {
            combatSystem.OnCombatLog -= UpdateCombatLog;
            combatSystem.OnCombatEnd -= OnCombatEnd;
        }

        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinueButtonClicked);
    }

    private void UpdateCombatLog(string message)
    {
        if (combatLogText == null)
        {
            Debug.LogError("CombatUIController: combatLogText not assigned!");
            return;
        }
        
        // Активируем кнопку при первом логе (начало боя)
        if (!combatStarted)
        {
            combatStarted = true;
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
                UpdateButtonText();
                Debug.Log("CombatUIController: Combat started, button activated with 'Pause' text");
            }
        }
        
        // Добавляем новую строку
        combatLogText.text += message + "\n";
        logLineCount++;
        
        // Ограничиваем количество строк в логе
        if (logLineCount > MAX_LOG_LINES)
        {
            string[] lines = combatLogText.text.Split('\n');
            combatLogText.text = "";
            for (int i = lines.Length - MAX_LOG_LINES; i < lines.Length; i++)
            {
                if (i >= 0 && !string.IsNullOrEmpty(lines[i]))
                    combatLogText.text += lines[i] + "\n";
            }
        }
        
        // Прокручиваем вниз
        if (combatLogScroll != null)
        {
            Canvas.ForceUpdateCanvases();
            combatLogScroll.verticalNormalizedPosition = 0f;
        }
        
        // Обновляем статистику
        Character player = combatSystem.GetPlayer();
        Enemy enemy = combatSystem.GetCurrentEnemy();

        if (player != null && playerStatsText != null)
            playerStatsText.text = $"Player:\nHP: {player.currentHealth}/{player.maxHealth}\nArmor: {player.armor}\nDamage: {player.damage}";

        if (enemy != null && enemyStatsText != null)
            enemyStatsText.text = $"Enemy:\nHP: {enemy.currentHealth}/{enemy.maxHealth}\nArmor: {enemy.armor}\nDamage: {enemy.damage}";
    }

    private void OnCombatEnd(string message)
    {
        combatEnded = true;
        Debug.Log($"CombatUIController: Battle ended - {message}");
        
        // Если игрок победил - выпадает карточка
        if (combatSystem.IsPlayerAlive() && cardDropSystem != null)
        {
            droppedCard = cardDropSystem.DropRandomCard();
            if (droppedCard != null)
            {
                cardRewardText.text = $"Reward: card '{droppedCard.displayName}'";
                cardRewardText.gameObject.SetActive(true);
            }
        }
        
        // Кнопка уже активна, просто обновляем текст
        if (continueButton != null)
        {
            UpdateButtonText();
            Debug.Log("CombatUIController: Combat ended, button text changed to 'Continue'");
        }
    }

    private void OnContinueButtonClicked()
    {
        if (combatEnded)
        {
            // Бой закончился - переходим к следующему
            Debug.Log("CombatUIController: Continue pressed - transitioning to next battle");
            
            if (combatSystem.IsPlayerAlive() && levelManager != null)
            {
                combatEnded = false;
                combatStarted = false; // Сбрасываем флаг для следующего боя
                cardRewardText.gameObject.SetActive(false);
                levelManager.OnBattleWon();
            }
            else
            {
                // Игрок проиграл - возвращаемся в меню
                SceneController.LoadMainMenu();
            }
        }
        else if (combatSystem.IsInCombat())
        {
            // Бой идёт - переключаем паузу
            if (combatSystem.IsPaused())
            {
                Debug.Log("CombatUIController: Resume battle");
                combatSystem.ResumeCombat();
            }
            else
            {
                Debug.Log("CombatUIController: Pausing battle");
                combatSystem.PauseCombat();
            }
            UpdateButtonText();
        }
    }

    private void UpdateButtonText()
    {
        if (continueButton == null)
        {
            Debug.LogError("CombatUIController: continueButton is null!");
            return;
        }

        TextMeshProUGUI buttonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null)
        {
            Debug.LogError("CombatUIController: Button TextMeshProUGUI component not found!");
            return;
        }

        if (combatSystem == null)
        {
            Debug.LogError("CombatUIController: combatSystem is null!");
            buttonText.text = "Error";
            return;
        }

        // Определяем текст кнопки
        string newText = "?";
        
        if (combatEnded)
        {
            newText = "Continue";
        }
        else if (combatSystem.IsPaused())
        {
            newText = "Continue";
        }
        else if (combatSystem.IsInCombat())
        {
            newText = "Pause";
        }
        else
        {
            newText = "Ready";
        }

        buttonText.text = newText;
        Debug.Log($"CombatUIController: Button text updated to '{newText}' (combatEnded={combatEnded}, paused={combatSystem.IsPaused()}, inCombat={combatSystem.IsInCombat()})");
    }
}
