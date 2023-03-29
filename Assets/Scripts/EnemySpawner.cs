using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : ObjectPool<EnemySlime>
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private Text waveCounterText;
    [SerializeField] private float spawnAreaHeight;
    [SerializeField] private float spawnAreaRadius;
    [SerializeField] private Vector2Int enemyAmountRandom;

    [Header("Enemy definitions")] 
    [SerializeField] private float moneyPerHP;
    [SerializeField] private float startingHealth;
    [SerializeField] private float healthPerLocation;
    [SerializeField] private Vector2 healthRandomThreshold;
    

    public delegate void EnemySpawnerEvent();
    public static event EnemySpawnerEvent OnEnemiesSpawn;
    public static event EnemySpawnerEvent OnEnemiesDefeated;
    private static readonly List<EnemySlime> Enemies = new();

    private float currentUnitHealth;
    public int LocationCounter { get; private set; }


    private void Awake()
    {
        Instance = this;
        MapWalker.OnLocationChanged += SpawnEnemies;
        PlayerSlime.OnPlayerDefeat += OnPlayerDefeat;
    }

    private void OnDestroy()
    {
        MapWalker.OnLocationChanged -= SpawnEnemies;
        PlayerSlime.OnPlayerDefeat -= OnPlayerDefeat;
    }

    private void Start() => SpawnEnemies();

    public static bool GetClosestEnemy(out Vector3 pos)
    {
        pos = Vector3.negativeInfinity;
        int count = Enemies.Count;
        if (count == 0) return false;
        
        var enemySlime = count == 1 ? 
            Enemies[0] : 
            Enemies.Aggregate((i1, i2) => i1.transform.position.x < i2.transform.position.x ? i1 : i2);
        
        pos = enemySlime.transform.position;
        return true;
    }
    
    private void SpawnEnemies()
    {
        float offset = spawnAreaHeight / 2f;
        float enemyAmount = Random.Range(enemyAmountRandom.x, enemyAmountRandom.y + 1);
        currentUnitHealth =  (startingHealth + healthPerLocation * LocationCounter) / enemyAmount;
        for (int i = 0; i < enemyAmount; i++)
        {
            EnemySlime enemy = CreateEnemy(i * spawnAreaHeight / enemyAmount - offset);
            Enemies.Add(enemy);
        }

        OnEnemiesSpawn?.Invoke();
    }

    private EnemySlime CreateEnemy(float zPos)
    {
        EnemySlime enemy = GetPoolable(false);
        Vector3 pos = transform.position;
        pos.z = zPos;
        pos.x += Random.Range(-spawnAreaRadius, spawnAreaRadius);
        enemy.transform.position = pos;
        enemy.gameObject.SetActive(true);
        return enemy;
    }

    public void RemoveEnemy(EnemySlime enemySlime)
    {
        if(Enemies.Contains(enemySlime)) Enemies.Remove(enemySlime);
        if(Enemies.Count == 0) CountLocation();
    }

    private void CountLocation()
    {
        OnEnemiesDefeated?.Invoke();
        LocationCounter++;
        UpdateWaveCounter();
    }

    private void OnPlayerDefeat()
    {
        LocationCounter = 0;
        UpdateWaveCounter();
        Enemies.Clear();
        SpawnEnemies();
    }

    private void UpdateWaveCounter() => waveCounterText.text = $"Wave {LocationCounter + 1}";

    public int GetMoneyReward(float health) => Mathf.RoundToInt(health * moneyPerHP);

    public float GetCurrentUnitHealth() => currentUnitHealth * Random.Range(healthRandomThreshold.x, healthRandomThreshold.y);


    // Рисует зону спавна врагов для дебага
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 size = new Vector3(spawnAreaRadius * 2, 0, spawnAreaHeight);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
