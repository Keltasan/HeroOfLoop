using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Контроллер экрана улучшений
/// </summary>
public class UpgradesController : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeElement
    {
        public Button increaseButton;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI costText;
    }

    [SerializeField] private UpgradeElement hpUpgrade;
    [SerializeField] private UpgradeElement armorUpgrade;
    [SerializeField] private UpgradeElement damageUpgrade;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Button backButton;

    private const int UPGRADE_COST = 100; // Стоимость каждого уровня улучшения
    private GameData gameData;

    private void OnEnable()
    {
        gameData = GameManager.Instance.GetGameData();
        
        hpUpgrade.increaseButton.onClick.AddListener(BuyHPUpgrade);
        armorUpgrade.increaseButton.onClick.AddListener(BuyArmorUpgrade);
        damageUpgrade.increaseButton.onClick.AddListener(BuyDamageUpgrade);
        backButton.onClick.AddListener(BackToMainMenu);

        RefreshUI();
    }

    private void OnDisable()
    {
        hpUpgrade.increaseButton.onClick.RemoveListener(BuyHPUpgrade);
        armorUpgrade.increaseButton.onClick.RemoveListener(BuyArmorUpgrade);
        damageUpgrade.increaseButton.onClick.RemoveListener(BuyDamageUpgrade);
        backButton.onClick.RemoveListener(BackToMainMenu);
    }

    private void BuyHPUpgrade() => BuyUpgrade("hp");
    private void BuyArmorUpgrade() => BuyUpgrade("armor");
    private void BuyDamageUpgrade() => BuyUpgrade("damage");
    private void BackToMainMenu() => SceneController.LoadMainMenu();

    private void BuyUpgrade(string upgradeType)
    {
        if (gameData.totalCoins < UPGRADE_COST)
        {
            Debug.Log("Недостаточно монет!");
            return;
        }

        gameData.totalCoins -= UPGRADE_COST;

        switch (upgradeType)
        {
            case "hp":
                gameData.upgrades.hpLevel++;
                break;
            case "armor":
                gameData.upgrades.armorLevel++;
                break;
            case "damage":
                gameData.upgrades.damageLevel++;
                break;
        }

        GameManager.Instance.SaveGame();
        RefreshUI();
    }

    private void RefreshUI()
    {
        hpUpgrade.levelText.text = $"Level: {gameData.upgrades.hpLevel}";
        hpUpgrade.costText.text = $"Cost: {UPGRADE_COST}";

        armorUpgrade.levelText.text = $"Level: {gameData.upgrades.armorLevel}";
        armorUpgrade.costText.text = $"Cost: {UPGRADE_COST}";

        damageUpgrade.levelText.text = $"Level: {gameData.upgrades.damageLevel}";
        damageUpgrade.costText.text = $"Cost: {UPGRADE_COST}";

        coinsText.text = $"Монеты: {gameData.totalCoins}";
    }
}
