using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private int upgradeCost;
    [SerializeField] private Text costText;
    [SerializeField] private Text valueText;
    [SerializeField] private Text levelText;
    [SerializeField] private Button button;

    private int currentUpgradeCost;
    private int currentLevel;

    
    
    private void Awake() => MoneyManager.OnMoneyChanged += UpdateAffordable;
    private void OnDestroy() => MoneyManager.OnMoneyChanged -= UpdateAffordable;

    private void Start()
    {
        currentUpgradeCost = upgradeCost;
        UpdateAffordable();
        UpdateCostText();
    }

    public void SetValueText(float value) => valueText.text = value.ToString("0.##");

    public void LevelUp()
    {
        if(!MoneyManager.Instance.CanAfford(currentUpgradeCost)) return;
        MoneyManager.Instance.AddMoney(-currentUpgradeCost);
        currentUpgradeCost += upgradeCost + Mathf.RoundToInt(upgradeCost * currentLevel * 0.075f);
        currentLevel++;
        UpdateLevelText();
        UpdateCostText();
        UpdateAffordable();
    }

    private void UpdateAffordable() => button.interactable = MoneyManager.Instance.CanAfford(currentUpgradeCost);
    private void UpdateCostText() => costText.text = currentUpgradeCost.ToString();
    private void UpdateLevelText() => levelText.text = $"Lv{currentLevel}";
}
