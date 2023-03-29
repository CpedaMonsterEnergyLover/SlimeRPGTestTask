using System;
using System.Collections;
using UnityEngine;

public class PlayerSlime : MonoBehaviour
{
    public static PlayerSlime Instance { get; private set; }

    [SerializeField] private float regenDelay;
    [Header("Starting stats")]
    [SerializeField] private float startingMaxHealth;
    [SerializeField] private float startingAttackDamage;
    [SerializeField] private float startingAttackSpeed;
    [SerializeField] private float startingRegeneration;
    [Header("Per level stats")]
    [SerializeField] private float maxHealthPerLevel;
    [SerializeField] private float attackDamagePerLevel;
    [SerializeField] private float attackSpeedPerLevel;
    [SerializeField] private float regenerationPerLevel;

    [Header("Upgrade buttons")] 
    [SerializeField] private UpgradeButton maxHealthButton;
    [SerializeField] private UpgradeButton attackDamageButton;
    [SerializeField] private UpgradeButton attackSpeedButton;
    [SerializeField] private UpgradeButton regenerationButton;
    
    private float currentHealth;
    private Healthbar healthbar;

    public delegate void PlayerSlimeEvent();
    public static event PlayerSlimeEvent OnPlayerDefeat;
    
    public float MaxHealth { get; private set; }
    public float AttackSpeed { get; private set; }
    public float AttackDamage { get; private set; }
    public float Regeneration { get; private set; }
    public float PosX => transform.position.x;

    
    
    private void Awake()
    {
        Instance = this;
        SetInitialValues();
    }

    private void Start()
    {
        CreateHealthbar();
        currentHealth = MaxHealth;
        UpdateButtons();
        currentHealth = startingMaxHealth;
        StartCoroutine(RegenerationRoutine());
    }

    private void CreateHealthbar()
    {
        healthbar = WorldCanvas.Instance.CreateHealthbar(playerHealthbar: true);
        healthbar.SetHealth(MaxHealth);
        healthbar.SetValue(1f);
        healthbar.UpdatePosition(transform.position);
    }
    
    private void SetInitialValues()
    {
        MaxHealth = startingMaxHealth;
        AttackDamage = startingAttackDamage;
        AttackSpeed = startingAttackSpeed;
        Regeneration = startingRegeneration;
    }

    private void UpdateButtons()
    {
        attackDamageButton.SetValueText(AttackDamage);
        attackSpeedButton.SetValueText(AttackSpeed);
        maxHealthButton.SetValueText(MaxHealth);
        regenerationButton.SetValueText(Regeneration);
    }

    public void LvlUpMaxHealth()
    {
        currentHealth += maxHealthPerLevel;
        MaxHealth += maxHealthPerLevel;
        UpdateHealthbar();
        maxHealthButton.SetValueText(MaxHealth);
    }

    public void LvlUpAttackDamage()
    {
        AttackDamage += attackDamagePerLevel;
        attackDamageButton.SetValueText(AttackDamage);
    }

    public void LvlUpAttackSpeed()
    {
        AttackSpeed += attackSpeedPerLevel;
        attackSpeedButton.SetValueText(AttackSpeed);
    }

    public void LvlUpRegeneration()
    {
        Regeneration += regenerationPerLevel;
        regenerationButton.SetValueText(Regeneration);
    }


    public void Damage()
    {
        if(currentHealth <= 0) return;

        float damage = 1 + EnemySpawner.Instance.LocationCounter * 0.2f;
        WorldCanvas.Instance.CreateDamageParticle(false, damage, transform.position);
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        UpdateHealthbar();
    }

    private IEnumerator RegenerationRoutine()
    {
        while (enabled)
        {
            Regen();
            yield return new WaitForSeconds(regenDelay);
        }
    }

    private void Regen()
    {
        if(currentHealth >= MaxHealth || Regeneration == 0) return;
        
        currentHealth += Regeneration;
        if (currentHealth > MaxHealth) currentHealth = MaxHealth;
        healthbar.SetHealth(currentHealth);
        healthbar.ForceValue(currentHealth / MaxHealth);
        WorldCanvas.Instance.CreateDamageParticle(true, Regeneration, transform.position);
    }

    private void UpdateHealthbar()
    {
        healthbar.SetHealth(currentHealth);
        healthbar.SetValue(currentHealth / MaxHealth);
    }

    private void Die()
    {
        OnPlayerDefeat?.Invoke();
        MoneyManager.Instance.LoseMoney();
        currentHealth = MaxHealth;
        UpdateHealthbar();
    }
}
