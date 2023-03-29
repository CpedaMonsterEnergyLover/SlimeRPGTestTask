using UnityEngine;

public class WorldCanvas : MonoBehaviour
{
    [SerializeField] private HealthbarPool healthbarPool;
    [SerializeField] private DamageParticlePool damageParticlePool;
    
    public static WorldCanvas Instance { get; private set; }

    
    
    private void Awake() => Instance = this;
    
    public Healthbar CreateHealthbar(bool playerHealthbar = false) => healthbarPool.CreateHealthbar(playerHealthbar);

    public void CreateDamageParticle(bool isHealing, float value, Vector3 pos) 
        => damageParticlePool.CreateDamageParticle(isHealing, value, pos);
}
