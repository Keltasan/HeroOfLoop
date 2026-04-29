using UnityEngine;

/// <summary>
/// Персонаж в бою - может быть игроком или врагом
/// </summary>
public class Character
{
    public int maxHealth;
    public int currentHealth;
    public int armor;
    public int damage;
    public string characterName;

    public Character(string name, int maxHealth, int armor, int damage)
    {
        characterName = name;
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
        this.armor = armor;
        this.damage = damage;
    }

    public void TakeDamage(int incomingDamage)
    {
        int actualDamage = Mathf.Max(1, incomingDamage - armor);
        currentHealth -= actualDamage;
        // Логирование теперь делается в CombatSystem, не здесь
    }

    public int GetDamage()
    {
        // Добавляем рандомное варьирование урона: ±20%
        float variation = Random.Range(0.8f, 1.2f);
        return Mathf.Max(1, (int)(damage * variation));
    }

    public bool IsAlive() => currentHealth > 0;

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
}

/// <summary>
/// Враг с рандомной сложностью
/// </summary>
public class Enemy : Character
{
    public int difficultyLevel;
    public int goldReward;

    public Enemy(int difficultyLevel) : base("Enemy", 0, 0, 0)
    {
        this.difficultyLevel = difficultyLevel;
        
        // Враги становятся сильнее с каждым уровнем
        maxHealth = 50 + (difficultyLevel * 10);
        currentHealth = maxHealth;
        armor = difficultyLevel * 2;
        damage = 5 + (difficultyLevel * 2);
        goldReward = 50 + (difficultyLevel * 10);
    }
}
