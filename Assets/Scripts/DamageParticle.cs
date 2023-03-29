using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageParticle : PoolableObject
{
    [SerializeField] private Text text;
    [SerializeField] private AnimationCurve alphaCurve;

    public void SetDamage(float damage)
    {
        text.text = Mathf.Abs(damage).ToString("0.##");
        text.color = damage < 0 ? Color.white : Color.green;
    }
    
    private void OnEnable()
    {
        if(Pooled) RestoreFromPool();
        StartCoroutine(LifetimeRoutine());
    }

    private IEnumerator LifetimeRoutine()
    {
        float t = 1f;
        while (t > 0)
        {
            float delta = Time.deltaTime;
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, (1 - t) * 2 + 0.5f, pos.z);
            text.color = text.color.WithAlpha(alphaCurve.Evaluate(t));
            t -= delta;
            yield return null;
        }
        
        Pool();
    }
    
    

    protected override void RestoreFromPool()
    {
        base.RestoreFromPool();
        text.color = text.color.WithAlpha(1f);
        gameObject.SetActive(true);
    }
}
