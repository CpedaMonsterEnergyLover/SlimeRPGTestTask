using UnityEngine;

public class DamageParticlePool : ObjectPool<DamageParticle>
{
    private static readonly Vector3 Offset = new Vector3(0, 0.25f, 0);
    
    public void CreateDamageParticle(bool isHealing, float value, Vector3 pos)
    {
        var particle = GetPoolable(true);
        particle.SetDamage(value * (isHealing ? 1 : -1));
        particle.transform.position = pos + Offset;
        particle.gameObject.SetActive(true);
    }
}
