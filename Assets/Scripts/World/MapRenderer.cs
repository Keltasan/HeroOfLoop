using UnityEngine;

/// <summary>
/// Воспроизведение карты в виде спрайтов
/// </summary>
public class MapRenderer : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private Transform tilesContainer;
    [SerializeField] private GameObject tilePrefab;
    private MapTile[,] currentMap;

    public void RenderMap()
    {
        Debug.Log("MapRenderer: RenderMap() начало");
        Debug.Log($"MapRenderer: TilesContainer: {(tilesContainer != null ? tilesContainer.name : "NULL")}");
        
        if (tilesContainer == null)
        {
            Debug.LogError("MapRenderer: TilesContainer не назначен!");
            return;
        }
        
        if (tilePrefab == null)
        {
            Debug.LogError("MapRenderer: Tile Prefab не назначен!");
            return;
        }

        // Надежная очистка всех старых тайлов
        int childCountBefore = tilesContainer.childCount;
        Debug.Log($"MapRenderer: Начинаю очистку. Найдено {childCountBefore} старых тайлов");
        
        // Удаляем в обратном порядке для избежания проблем с индексами
        while (tilesContainer.childCount > 0)
        {
            Transform child = tilesContainer.GetChild(0); // Берем первого, потом индексы сдвигаются
            Debug.Log($"MapRenderer: Удаляю тайл '{child.name}' (осталось: {tilesContainer.childCount - 1})");
            DestroyImmediate(child.gameObject);
        }
        
        Debug.Log($"MapRenderer: Очистка завершена. Удалено {childCountBefore} тайлов, осталось {tilesContainer.childCount}");

        currentMap = mapGenerator.GenerateMap();
        
        int width = mapGenerator.GetMapWidth();
        int height = mapGenerator.GetMapHeight();
        int tileSize = mapGenerator.GetTileSize();
        
        Debug.Log($"MapRenderer: Генерирую {width}x{height} тайлов (размер {tileSize})");

        int count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                MapTile tile = currentMap[x, y];
                GameObject tileObject = Instantiate(tilePrefab, tilesContainer);
                
                Vector3 position = new Vector3(x * tileSize, y * tileSize, 0);
                tileObject.transform.localPosition = position;

                SpriteRenderer spriteRenderer = tileObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    Debug.LogError($"MapRenderer: Tile префаб не имеет SpriteRenderer!");
                    continue;
                }

                // Устанавливаем цвет в зависимости от типа локации
                spriteRenderer.color = GetColorForLocationType(tile.locationType);
                spriteRenderer.enabled = true;

                tileObject.name = $"Tile_{x}_{y}";
                count++;
            }
        }

        Debug.Log($"MapRenderer: Создано {count} тайлов");
        Debug.Log($"MapRenderer: TilesContainer позиция: {tilesContainer.position}");
        Debug.Log($"MapRenderer: Camera позиция: {Camera.main.transform.position}");
        Debug.Log("MapRenderer: RenderMap() завершено");
    }

    private Color GetColorForLocationType(LocationType type)
    {
        return type switch
        {
            LocationType.Battle => Color.red,
            LocationType.Start => Color.yellow,
            LocationType.Empty => Color.white,
            LocationType.Forest => Color.green,
            LocationType.Mountain => Color.gray,
            LocationType.Cemetery => new Color(0.4f, 0.2f, 0.2f),
            LocationType.Library => Color.blue,
            LocationType.Shop => Color.yellow,
            LocationType.Mansion => new Color(0.8f, 0f, 0.8f),
            LocationType.Path => Color.cyan,
            _ => Color.white
        };
    }

    public MapTile[,] GetCurrentMap() => currentMap;
}
