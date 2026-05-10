using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Система управления бонусами от выставленных карточек
/// </summary>
public class CardBonusSystem : MonoBehaviour
{
    private Dictionary<Vector2Int, LocationCard> placedCards = new Dictionary<Vector2Int, LocationCard>();
    
    /// <summary>
    /// Регистрирует выставленную карточку и её бонусы
    /// </summary>
    public void RegisterPlacedCard(Vector2Int position, LocationCard card)
    {
        if (card == null)
        {
            Debug.LogWarning("CardBonusSystem: Попытка зарегистрировать null карточку");
            return;
        }
        
        placedCards[position] = card;
        Debug.Log($"CardBonusSystem: Карточка '{card.displayName}' зарегистрирована на позиции ({position.x}, {position.y})");
        Debug.Log($"CardBonusSystem: Активные бонусы - HP: +{GetTotalHPBonus()}, DMG: +{GetTotalDamageBonus()}, ARM: +{GetTotalArmorBonus()}");
    }
    
    /// <summary>
    /// Получает общий HP бонус от всех выставленных карточек
    /// </summary>
    public int GetTotalHPBonus()
    {
        int total = 0;
        foreach (var card in placedCards.Values)
        {
            total += card.hpBonus;
        }
        return total;
    }
    
    /// <summary>
    /// Получает общий Damage бонус от всех выставленных карточек
    /// </summary>
    public int GetTotalDamageBonus()
    {
        int total = 0;
        foreach (var card in placedCards.Values)
        {
            total += card.damageBonus;
        }
        return total;
    }
    
    /// <summary>
    /// Получает общий Armor бонус от всех выставленных карточек
    /// </summary>
    public int GetTotalArmorBonus()
    {
        int total = 0;
        foreach (var card in placedCards.Values)
        {
            total += card.armorBonus;
        }
        return total;
    }
    
    /// <summary>
    /// Применяет все активные бонусы к персонажу
    /// </summary>
    public void ApplyBonusesToCharacter(Character character)
    {
        if (character == null)
        {
            Debug.LogWarning("CardBonusSystem: Попытка применить бонусы к null персонажу");
            return;
        }
        
        int hpBonus = GetTotalHPBonus();
        int damageBonus = GetTotalDamageBonus();
        int armorBonus = GetTotalArmorBonus();
        
        character.maxHealth += hpBonus;
        character.damage += damageBonus;
        character.armor += armorBonus;
        
        // Если игрок исцеляет себя при применении HP бонуса
        if (hpBonus > 0)
        {
            character.currentHealth += hpBonus;
        }
        
        Debug.Log($"CardBonusSystem: Бонусы применены к {character.characterName}");
        Debug.Log($"  HP: +{hpBonus} (всего {character.maxHealth})");
        Debug.Log($"  DMG: +{damageBonus} (всего {character.damage})");
        Debug.Log($"  ARM: +{armorBonus} (всего {character.armor})");
    }
    
    /// <summary>
    /// Получает список всех выставленных карточек
    /// </summary>
    public Dictionary<Vector2Int, LocationCard> GetPlacedCards() => new Dictionary<Vector2Int, LocationCard>(placedCards);
    
    /// <summary>
    /// Очищает список выставленных карточек (для новой игры/уровня)
    /// </summary>
    public void ClearPlacedCards()
    {
        placedCards.Clear();
        Debug.Log("CardBonusSystem: Выставленные карточки очищены");
    }
}
