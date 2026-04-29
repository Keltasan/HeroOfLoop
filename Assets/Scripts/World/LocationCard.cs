using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Конфиг карточек локаций с настройками вероятности
/// </summary>
[CreateAssetMenu(fileName = "LocationCardConfig", menuName = "Game/Location Card Config")]
public class LocationCardConfig : ScriptableObject
{
    public List<LocationCard> cards = new List<LocationCard>();

    public LocationCard GetRandomCard()
    {
        if (cards == null || cards.Count == 0)
        {
            Debug.LogError("LocationCardConfig: Нет карточек в списке!");
            return null;
        }

        float totalChance = 0f;
        foreach (var card in cards)
        {
            totalChance += card.dropChance;
        }

        float randomValue = Random.value * totalChance;
        float currentChance = 0f;

        foreach (var card in cards)
        {
            currentChance += card.dropChance;
            if (randomValue <= currentChance)
                return card;
        }

        return cards[cards.Count - 1];
    }

    public LocationCard GetCard(LocationType type)
    {
        return cards.Find(card => card.type == type);
    }

    public List<LocationCard> GetAllCards() => new List<LocationCard>(cards);
}

