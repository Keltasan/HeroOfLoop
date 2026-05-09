using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Управляет движением игрока по пути на карте
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private float moveDelay = 0.25f;  // Задержка между шагами (0.25 сек)
    
    private List<Vector2Int> pathSequence;  // Путь в порядке генерации
    private int currentPathIndex = 0;  // Текущая позиция в пути
    private Vector2Int currentPos;
    private bool isMoving = false;
    private bool inCombat = false;
    
    private int tileSize;
    private Vector3 startWorldPosition;

    private void Start()
    {
        if (mapGenerator == null)
        {
            Debug.LogError("PlayerMovement: MapGenerator не назначен!");
            return;
        }
        
        tileSize = mapGenerator.GetTileSize();
        
        // Получаем стартовую позицию
        currentPos = mapGenerator.GetStartPosition();
        currentPathIndex = 0;
        
        // Установим позицию спрайта игрока в стартовую клетку
        startWorldPosition = new Vector3(currentPos.x * tileSize, currentPos.y * tileSize, -1);
        transform.localPosition = startWorldPosition;
        
        Debug.Log($"PlayerMovement: Инициализирован на позиции ({currentPos.x}, {currentPos.y})");
        
        // Начинаем движение
        StartCoroutine(MoveAlongPath());
    }

    private IEnumerator MoveAlongPath()
    {
        // Ждем, пока закончится первый бой на стартовой позиции
        yield return new WaitUntil(() => !inCombat);
        
        isMoving = true;
        
        while (true)
        {
            // Задержка между шагами
            yield return new WaitForSeconds(moveDelay);
            
            if (inCombat)
            {
                // Если в бою, ждем завершения боя
                yield return new WaitUntil(() => !inCombat);
                continue;
            }
            
            // Переходим к следующей клетке пути
            MoveToNextTile();
            
            // Проверяем, находимся ли на клетке боя
            MapTile currentTile = mapGenerator.GetTile(currentPos.x, currentPos.y);
            if (currentTile != null && currentTile.locationType == LocationType.Battle)
            {
                Debug.Log($"PlayerMovement: Игрок наступил на клетку боя на позиции ({currentPos.x}, {currentPos.y})");
                StartBattle();
                yield return new WaitUntil(() => !inCombat);
            }
        }
    }

    private void MoveToNextTile()
    {
        // Получаем следующую позицию в пути
        if (pathSequence == null || pathSequence.Count == 0)
        {
            Debug.LogWarning("PlayerMovement: pathSequence не инициализирован!");
            return;
        }
        
        // Переходим к следующей клетке
        currentPathIndex++;
        
        // Если дошли до конца пути, начинаем сначала
        if (currentPathIndex >= pathSequence.Count)
        {
            currentPathIndex = 0;
            Debug.Log("PlayerMovement: Игрок прошел весь путь, начинает сначала");
        }
        
        currentPos = pathSequence[currentPathIndex];
        
        // Плавное движение спрайта к новой позиции (опционально)
        Vector3 targetWorldPosition = new Vector3(currentPos.x * tileSize, currentPos.y * tileSize, -1);
        transform.localPosition = targetWorldPosition;
        
        Debug.Log($"PlayerMovement: Игрок переместился на позицию ({currentPos.x}, {currentPos.y}) [индекс {currentPathIndex}]");
    }

    private void StartBattle()
    {
        inCombat = true;
        int enemyLevel = GetEnemyLevelForBattle();
        
        if (combatSystem != null)
        {
            combatSystem.StartCombat(enemyLevel);
            Debug.Log($"PlayerMovement: Начат бой уровня {enemyLevel}");
        }
    }

    private int GetEnemyLevelForBattle()
    {
        // Уровень врага зависит от количества пройденных клеток
        // Можно усложнить логику позже
        return Mathf.Max(1, currentPathIndex / 5 + 1);
    }

    /// <summary>
    /// Инициализирует путь в порядке генерации
    /// Должна быть вызвана MapGenerator после генерации пути
    /// </summary>
    public void InitializePathSequence(List<Vector2Int> path)
    {
        pathSequence = new List<Vector2Int>(path);
        Debug.Log($"PlayerMovement: Инициализирован путь из {pathSequence.Count} клеток");
    }

    /// <summary>
    /// Вызывается когда бой завершен
    /// </summary>
    public void OnCombatEnded()
    {
        inCombat = false;
        Debug.Log("PlayerMovement: Бой завершен, продолжаем движение");
    }

    public Vector2Int GetCurrentPosition() => currentPos;
    public bool IsMoving() => isMoving;
    public bool IsInCombat() => inCombat;
}
