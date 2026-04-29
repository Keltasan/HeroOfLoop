using UnityEngine;

/// <summary>
/// Карточка локации с конфигурацией вероятности выпадения
/// </summary>
[System.Serializable]
public class LocationCard
{
    public LocationType type;
    public string displayName;
    public Sprite icon;
    public float dropChance = 0.1f;
    public int hpBonus;
    public int damageBonus;
    public int armorBonus;
}
