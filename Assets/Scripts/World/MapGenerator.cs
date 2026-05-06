using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Одна ячейка на карте
/// </summary>
public class MapTile
{
    public int x;
    public int y;
    public LocationType locationType;
    public bool isPartOfPath;

    public MapTile(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.locationType = LocationType.Empty;
        this.isPartOfPath = false;
    }
}

/// <summary>
/// Генератор карты с garantией, что путь игрока не выходит за экран
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidth = 15;
    [SerializeField] private int mapHeight = 10;
    [SerializeField] private int tileSize = 64;

    private MapTile[,] map;

    public MapTile[,] GenerateMap()
    {
        Debug.Log($"MapGenerator: GenerateMap() начало. Размер: {mapWidth}x{mapHeight}");
        
        // Полностью очищаем старую карту
        map = null;
        
        // Создаем новую карту
        map = new MapTile[mapWidth, mapHeight];
        
        // Инициализируем карту пустыми тайлами
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                map[x, y] = new MapTile(x, y);
                map[x, y].locationType = LocationType.Empty;
                map[x, y].isPartOfPath = false;
            }
        }

        // Генерируем путь
        GeneratePath();

        Debug.Log($"MapGenerator: GenerateMap() завершено");
        return map;
    }

    private void GeneratePath()
    {
        Debug.Log($"MapGenerator: Генерирую путь случайным блужданием");
        
        int targetPathLength = Random.Range(15, 20);  // Целевая длина пути
        int maxAttempts = 10000;  // Максимум попыток
        
        // 1. Выбираем рандомную стартовую точку
        Vector2Int startPos = new Vector2Int(
            Random.Range(1, mapWidth - 1),
            Random.Range(1, mapHeight - 1)
        );
        
        Debug.Log($"MapGenerator: Стартовая позиция: ({startPos.x}, {startPos.y}), целевая длина: {targetPathLength}");
        
        // 2. Инициализируем путь и повторяем генерацию до достижения целевой длины
        HashSet<Vector2Int> pathTiles = new HashSet<Vector2Int> { startPos };
        Vector2Int currentPos = startPos;
        Vector2Int previousPos = startPos;
        Vector2Int twoStepsBackPos = startPos;
        Vector2Int lastDirection = Vector2Int.zero;
        bool pathIsValid = false;
        int generationAttempts = 0;
        
        while (!pathIsValid && generationAttempts < 10)
        {
            generationAttempts++;
            pathTiles.Clear();
            pathTiles.Add(startPos);
            currentPos = startPos;
            previousPos = startPos;
            twoStepsBackPos = startPos;
            lastDirection = Vector2Int.zero;
        
        // Возможные направления: вверх, вниз, влево, вправо
        Vector2Int[] directions = new Vector2Int[4]
        {
            new Vector2Int(0, 1),   // вверх
            new Vector2Int(0, -1),  // вниз
            new Vector2Int(-1, 0),  // влево
            new Vector2Int(1, 0)    // вправо
        };
        
        // 8 направлений для проверки соседей
        Vector2Int[] allDirections = new Vector2Int[8]
        {
            new Vector2Int(0, 1),    // вверх
            new Vector2Int(0, -1),   // вниз
            new Vector2Int(-1, 0),   // влево
            new Vector2Int(1, 0),    // вправо
            new Vector2Int(-1, 1),   // диагональ вверх-влево
            new Vector2Int(1, 1),    // диагональ вверх-вправо
            new Vector2Int(-1, -1),  // диагональ вниз-влево
            new Vector2Int(1, -1)    // диагональ вниз-вправо
        };
        
        // 3. Генерируем путь методом случайного блуждания
        int attempts = 0;
        while (pathTiles.Count < targetPathLength && attempts < maxAttempts)
        {
            // Выбираем случайное направление
            int dirIndex = Random.Range(0, 4);
            Vector2Int nextDir = directions[dirIndex];
            Vector2Int nextPos = currentPos + nextDir;
            
            // Проверяем направление (изменилось ли)
            bool directionChanged = (nextDir != lastDirection && lastDirection != Vector2Int.zero);
            
            // Проверяем границы
            if (nextPos.x < 1 || nextPos.x >= mapWidth - 1 || nextPos.y < 1 || nextPos.y >= mapHeight - 1)
            {
                attempts++;
                continue;
            }
            
            // Проверяем, что мы не возвращаемся на предыдущие шаги
            if (nextPos == previousPos)
            {
                attempts++;
                continue;
            }
            
            // Эту исключаем только если направление изменилось
            if (directionChanged && nextPos == twoStepsBackPos)
            {
                attempts++;
                continue;
            }
            
            // Проверяем, что позиция еще не в пути
            if (pathTiles.Contains(nextPos))
            {
                attempts++;
                continue;
            }
            
            // Позиция валидна! Добавляем ее
            pathTiles.Add(nextPos);
            twoStepsBackPos = previousPos;
            previousPos = currentPos;
            currentPos = nextPos;
            lastDirection = nextDir;
            attempts = 0;  // Сбрасываем счетчик неудачных попыток
        }
        
            Debug.Log($"MapGenerator: Путь сгенерирован, текущая длина: {pathTiles.Count}");
            
            // 4. Пытаемся вернуться в стартовую позицию
            int closeAttempts = 0;
            int maxCloseAttempts = 5000;
            
            while (currentPos != startPos && closeAttempts < maxCloseAttempts)
        {
            // Вычисляем направление к стартовой позиции
            Vector2Int toStart = Vector2Int.zero;
            
            if (currentPos.x < startPos.x)
                toStart.x = 1;
            else if (currentPos.x > startPos.x)
                toStart.x = -1;
            
            if (currentPos.y < startPos.y)
                toStart.y = 1;
            else if (currentPos.y > startPos.y)
                toStart.y = -1;
            
            // Если мы на одной линии с стартом, двигаемся по этой линии
            Vector2Int nextPos;
            if (toStart.x == 0)
                nextPos = currentPos + new Vector2Int(0, toStart.y);
            else if (toStart.y == 0)
                nextPos = currentPos + new Vector2Int(toStart.x, 0);
            else
            {
                // Выбираем случайное из двух направлений
                nextPos = currentPos + (Random.value > 0.5f ? new Vector2Int(toStart.x, 0) : new Vector2Int(0, toStart.y));
            }
            
            // Проверяем границы
            if (nextPos.x < 1 || nextPos.x >= mapWidth - 1 || nextPos.y < 1 || nextPos.y >= mapHeight - 1)
            {
                closeAttempts++;
                continue;
            }
            
            // Проверяем пересечение (разрешаем добавление новых клеток на финальном отрезке)
            if (!pathTiles.Contains(nextPos))
            {
                pathTiles.Add(nextPos);
                currentPos = nextPos;
                closeAttempts = 0;
            }
            else if (nextPos == startPos)
            {
                // Достигли стартовой позиции
                currentPos = nextPos;
                break;
            }
            else
            {
                closeAttempts++;
            }
            }
            
            Debug.Log($"MapGenerator: Финальная длина пути: {pathTiles.Count}");
            
            // Проверяем, достаточна ли длина пути
            if (pathTiles.Count >= targetPathLength)
            {
                pathIsValid = true;
                Debug.Log($"MapGenerator: Путь валиден (длина {pathTiles.Count} >= {targetPathLength})");
            }
            else
            {
                Debug.Log($"MapGenerator: Путь слишком короткий ({pathTiles.Count} < {targetPathLength}), повторная попытка {generationAttempts}");
            }
        }
        
        if (!pathIsValid)
        {
            Debug.LogWarning($"MapGenerator: Не удалось сгенерировать путь достаточной длины после 10 попыток");
        }
        
        // 5. Размещаем путь на карте
        int pathTileCount = 0;
        foreach (Vector2Int pos in pathTiles)
        {
            if (pos != startPos)
            {
                int randomLocationType = Random.Range(0, 100);
                if (randomLocationType < 12)
                    map[pos.x, pos.y].locationType = LocationType.Battle;
                else
                    map[pos.x, pos.y].locationType = LocationType.Path;
                map[pos.x, pos.y].isPartOfPath = true;
                pathTileCount++;
            }
        }
        
        // 6. Размещаем стартовую позицию
        map[startPos.x, startPos.y].locationType = LocationType.Start;
        map[startPos.x, startPos.y].isPartOfPath = true;
        
        Debug.Log($"MapGenerator: Размещено {pathTileCount} плиток боевого пути, старт в ({startPos.x}, {startPos.y})");
    }

    public void PlaceLocationCard(int x, int y, LocationType locationType)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            map[x, y].locationType = locationType;
        }
    }

    public MapTile GetTile(int x, int y)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            return map[x, y];
        return null;
    }

    public int GetMapWidth() => mapWidth;
    public int GetMapHeight() => mapHeight;
    public int GetTileSize() => tileSize;
}
