using System.Collections;
using UnityEngine;

public class EnemySlime : PoolableObject
{
    [SerializeField] private Animator animator;
    [SerializeField] private float walkSpeed;

    private static readonly int RunAnimHash = Animator.StringToHash("RunFWD");
    private static readonly int WalkParamHash = Animator.StringToHash("Walk");
    private static readonly int AttackAnimHash = Animator.StringToHash("Attack01");
    private static readonly int HitParamHash = Animator.StringToHash("Hit");
    private static readonly int DieAnimHash = Animator.StringToHash("Die");

    private float health;
    private float maxHealth;
    private bool dead;
    private Healthbar healthbar;


    
    private void OnEnable()
    {
        if(Pooled) RestoreFromPool();
    }
    
    private void Start()
    {
        maxHealth = EnemySpawner.Instance.GetCurrentUnitHealth();
        health = maxHealth;
        healthbar = WorldCanvas.Instance.CreateHealthbar();
        healthbar.SetValue(1f);
        healthbar.SetHealth(health);
        healthbar.UpdatePosition(transform.position);
        StartCoroutine(WalkRoutine());
    }

    private IEnumerator WalkRoutine()
    {
        float finalPos = PlayerSlime.Instance.PosX + 1;
        animator.SetBool(WalkParamHash, true);
        animator.Play(RunAnimHash);
        Vector3 pos = transform.position;
        
        while (pos.x > finalPos)
        {
            pos.x -= walkSpeed * Time.deltaTime;
            transform.position = pos;
            healthbar.UpdatePosition(pos);
            yield return null;
        }

        animator.SetBool(WalkParamHash, false);
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        animator.Play(AttackAnimHash);
        while (!dead)
        {
            yield return new WaitForSeconds(8 / 25f);
            PlayerSlime.Instance.Damage();
            yield return new WaitForSeconds(16 / 25f);
        }
    }

    private void Die()
    {
        dead = true;
        animator.Play(DieAnimHash);
        healthbar.FadeOut();
        healthbar = null;
        StopAllCoroutines();
        EnemySpawner.Instance.RemoveEnemy(this);
        MoneyManager.Instance.AddMoney(EnemySpawner.Instance.GetMoneyReward(maxHealth));
        StartCoroutine(DespawnRoutine());
    }

    private void OnCollisionEnter(Collision _)
    {
        if(dead) return;
        animator.SetTrigger(HitParamHash);
        Damage();
    }

    private void Damage()
    {
        if(health <= 0) return;

        float damage = PlayerSlime.Instance.AttackDamage;
        WorldCanvas.Instance.CreateDamageParticle(false, damage, transform.position);
        health -= damage;
        if (health <= 0)
        {
            healthbar.SetHealth(0);
            healthbar.SetValue(0);
            Die();
        }
        else
        {
            healthbar.SetHealth(health);
            healthbar.SetValue(Mathf.Clamp01(health / maxHealth));
        }
    }

    private IEnumerator DespawnRoutine()
    {
        Vector3 pos = transform.position;
        float t = 0;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            pos.y = -t;
            transform.position = pos;
            yield return null;
        }

        Pool();
    }


    // PoolableObject
    protected override void RestoreFromPool()
    {
        base.RestoreFromPool();
        dead = false;
        maxHealth = EnemySpawner.Instance.GetCurrentUnitHealth();
        health = maxHealth;
        healthbar = WorldCanvas.Instance.CreateHealthbar();
        healthbar.SetValue(1f);
        healthbar.SetHealth(health);
        StartCoroutine(WalkRoutine());
    }
}
