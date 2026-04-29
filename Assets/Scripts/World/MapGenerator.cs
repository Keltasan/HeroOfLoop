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
        Debug.Log($"MapGenerator: Генерирую путь вокруг центрального прямоугольника");
        
        // 1. Генерируем случайные координаты прямоугольника
        int minMargin = 2;  // Минимальный отступ от границ
        int minRectWidth = 4;   // Минимальная ширина прямоугольника
        int minRectHeight = 3;  // Минимальная высота прямоугольника
        
        // Генерируем x1, x2 где x1 < x2
        int x1 = Random.Range(minMargin, mapWidth / 2 - minRectWidth);
        int x2 = Random.Range(x1 + minRectWidth, mapWidth - minMargin);
        
        // Генерируем y1, y2 где y1 > y2 (y1 выше на экране)
        int y2 = Random.Range(minMargin, mapHeight / 2 - minRectHeight);
        int y1 = Random.Range(y2 + minRectHeight, mapHeight - minMargin);
        
        Debug.Log($"MapGenerator: Случайный прямоугольник: точка1({x1},{y1}) точка2({x2},{y1}) точка3({x2},{y2}) точка4({x1},{y2})");
        
        // 2. Выбираем 4 угловые точки прямоугольника в правильном порядке (по часовой стрелке)
        Vector2Int point1 = new Vector2Int(x1, y1);      // Верхний левый
        Vector2Int point2 = new Vector2Int(x2, y1);     // Верхний правый
        Vector2Int point3 = new Vector2Int(x2, y2);     // Нижний правый
        Vector2Int point4 = new Vector2Int(x1, y2);     // Нижний левый
        
        // 3. Генерируем дополнительные точки и распределяем их по регионам
        int boundaryOffset = 2;
        List<Vector2Int> topPoints = new List<Vector2Int>();      // Между точкой 1 и 2 (верхняя)
        List<Vector2Int> rightPoints = new List<Vector2Int>();    // Между точкой 2 и 3 (правая)
        List<Vector2Int> bottomPoints = new List<Vector2Int>();   // Между точкой 3 и 4 (нижняя)
        List<Vector2Int> leftPoints = new List<Vector2Int>();     // Между точкой 4 и 1 (левая)
        
        int additionalPoints = Random.Range(3, 7);  // 3-6 дополнительных точек
        
        for (int i = 0; i < additionalPoints; i++)
        {
            Vector2Int newPoint = Vector2Int.zero;
            bool isValid = false;
            int attempts = 0;
            
            while (!isValid && attempts < 10)
            {
                int side = Random.Range(0, 4);
                
                switch (side)
                {
                    case 0: // Верхняя сторона
                        newPoint = new Vector2Int(
                            Random.Range(x1 - boundaryOffset, x2 + boundaryOffset),
                            Random.Range(0, y1 - boundaryOffset)
                        );
                        break;
                    case 1: // Правая сторона
                        newPoint = new Vector2Int(
                            Random.Range(x2 + boundaryOffset, mapWidth),
                            Random.Range(y2 - boundaryOffset, y1 + boundaryOffset)
                        );
                        break;
                    case 2: // Нижняя сторона
                        newPoint = new Vector2Int(
                            Random.Range(x1 - boundaryOffset, x2 + boundaryOffset),
                            Random.Range(y2 + boundaryOffset, mapHeight)
                        );
                        break;
                    case 3: // Левая сторона
                        newPoint = new Vector2Int(
                            Random.Range(0, x1 - boundaryOffset),
                            Random.Range(y2 - boundaryOffset, y1 + boundaryOffset)
                        );
                        break;
                }
                
                if (newPoint.x >= 0 && newPoint.x < mapWidth && newPoint.y >= 0 && newPoint.y < mapHeight)
                {
                    // Проверяем что это не основные точки и не дубликат
                    if (newPoint != point1 && newPoint != point2 && newPoint != point3 && newPoint != point4)
                    {
                        bool isDuplicate = topPoints.Contains(newPoint) || rightPoints.Contains(newPoint) ||
                                         bottomPoints.Contains(newPoint) || leftPoints.Contains(newPoint);
                        
                        if (!isDuplicate)
                        {
                            isValid = true;
                            // Добавляем в соответствующий регион
                            if (side == 0) topPoints.Add(newPoint);
                            else if (side == 1) rightPoints.Add(newPoint);
                            else if (side == 2) bottomPoints.Add(newPoint);
                            else leftPoints.Add(newPoint);
                            
                            Debug.Log($"MapGenerator: Добавлена точка на сторону {side}: ({newPoint.x}, {newPoint.y})");
                        }
                    }
                }
                
                attempts++;
            }
        }
        
        // 4. Сортируем точки в каждом регионе
        // Верхняя: слева направо (по X)
        topPoints.Sort((a, b) => a.x.CompareTo(b.x));
        
        // Правая: сверху вниз (по Y)
        rightPoints.Sort((a, b) => a.y.CompareTo(b.y));
        
        // Нижняя: справа налево (по X, в обратном порядке)
        bottomPoints.Sort((a, b) => b.x.CompareTo(a.x));
        
        // Левая: снизу вверх (по Y, в обратном порядке)
        leftPoints.Sort((a, b) => b.y.CompareTo(a.y));
        
        // 5. Собираем финальный список точек в правильном порядке
        List<Vector2Int> finalPathPoints = new List<Vector2Int> { point1 };
        finalPathPoints.AddRange(topPoints);
        finalPathPoints.Add(point2);
        finalPathPoints.AddRange(rightPoints);
        finalPathPoints.Add(point3);
        finalPathPoints.AddRange(bottomPoints);
        finalPathPoints.Add(point4);
        finalPathPoints.AddRange(leftPoints);
        
        Debug.Log($"MapGenerator: Всего точек для соединения: {finalPathPoints.Count}");
        for (int i = 0; i < finalPathPoints.Count; i++)
        {
            Debug.Log($"MapGenerator: Точка {i}: ({finalPathPoints[i].x}, {finalPathPoints[i].y})");
        }
        
        // 6. Соединяем точки путем поиска маршрута, который не пересекает самого себя
        HashSet<Vector2Int> usedTiles = new HashSet<Vector2Int>();
        int totalPathTiles = 0;
        
        for (int i = 0; i < finalPathPoints.Count; i++)
        {
            Vector2Int currentPoint = finalPathPoints[i];
            Vector2Int nextPoint = finalPathPoints[(i + 1) % finalPathPoints.Count];
            
            // Соединяем точки простым алгоритмом (сначала по X, потом по Y)
            ConnectPoints(currentPoint.x, currentPoint.y, nextPoint.x, nextPoint.y, usedTiles, ref totalPathTiles);
        }
        
        // 7. Отмечаем первую точку как стартовую
        if (point1.x >= 0 && point1.x < mapWidth && point1.y >= 0 && point1.y < mapHeight)
        {
            map[point1.x, point1.y].locationType = LocationType.Start;
            Debug.Log($"MapGenerator: Start position: ({point1.x}, {point1.y})");
        }
        
        Debug.Log($"MapGenerator: Всего плиток на пути: {totalPathTiles}");
    }

    // Соединяет две точки, отмечая использованные плитки
    private void ConnectPoints(int x0, int y0, int x1, int y1, HashSet<Vector2Int> usedTiles, ref int tileCount)
    {
        int x = x0;
        int y = y0;
        
        // Сначала двигаемся по X
        while (x != x1)
        {
            x += x < x1 ? 1 : -1;
            
            Vector2Int pos = new Vector2Int(x, y);
            if (!usedTiles.Contains(pos))
            {
                usedTiles.Add(pos);
                if (x >= 1 && x < mapWidth - 1 && y >= 1 && y < mapHeight - 1)
                {
                    map[x, y].locationType = LocationType.Battle;
                    map[x, y].isPartOfPath = true;
                    tileCount++;
                }
            }
        }
        
        // Потом двигаемся по Y
        while (y != y1)
        {
            y += y < y1 ? 1 : -1;
            
            Vector2Int pos = new Vector2Int(x, y);
            if (!usedTiles.Contains(pos))
            {
                usedTiles.Add(pos);
                if (x >= 1 && x < mapWidth - 1 && y >= 1 && y < mapHeight - 1)
                {
                    map[x, y].locationType = LocationType.Battle;
                    map[x, y].isPartOfPath = true;
                    tileCount++;
                }
            }
        }
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
