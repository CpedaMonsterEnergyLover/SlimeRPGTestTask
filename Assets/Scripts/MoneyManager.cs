using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; set; }
    
    [SerializeField] private Text moneyText;
    
    public delegate void MoneyManagerEvent();
    public static MoneyManagerEvent OnMoneyChanged;
    
    private int currentMoney;

    
    private void Awake() => Instance = this;
    public bool CanAfford(int money) => money <= currentMoney;

    public void LoseMoney()
    {
        currentMoney = 0;
        OnMoneyChanged?.Invoke();
        UpdateMoneyText();
    }
    
    public void AddMoney(int money)
    {
        currentMoney += money;
        if (currentMoney <= 0) currentMoney = 0;
        OnMoneyChanged?.Invoke();
        UpdateMoneyText();
    }

    private void UpdateMoneyText() => moneyText.text = currentMoney.ToString();
}
