using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Система выпадения карточек локаций после боя
/// </summary>
public class CardDropSystem : MonoBehaviour
{
    [SerializeField] private LocationCardConfig locationCardConfig;
    
    private LocationCard droppedCard;

    public LocationCard DropRandomCard()
    {
        if (locationCardConfig == null)
        {
            Debug.LogError("CardDropSystem: LocationCardConfig не назначен!");
            return null;
        }

        droppedCard = locationCardConfig.GetRandomCard();
        Debug.Log($"CardDropSystem: Выпала карточка '{droppedCard.displayName}' (шанс {droppedCard.dropChance * 100:F0}%)");
        return droppedCard;
    }

    public LocationCard GetDroppedCard() => droppedCard;

    public void ClearDroppedCard() => droppedCard = null;
}
