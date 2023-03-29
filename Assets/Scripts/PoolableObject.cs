using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    protected bool Pooled { get; private set; }
    private IObjectPool objectPool;
    public bool IgnorePlayerLifetime { get; set; }
    
    
    public void AttachTo(Transform t)
    {
        transform.SetParent(t);
        transform.localPosition = Vector3.zero;
    }
    
    protected void Pool()
    {
        if(!IgnorePlayerLifetime) PlayerSlime.OnPlayerDefeat -= Pool;
        gameObject.SetActive(false);
        Pooled = true;
        objectPool.PoolObject(this);
    }

    protected virtual void RestoreFromPool()
    {
        Pooled = false;
        if(!IgnorePlayerLifetime) PlayerSlime.OnPlayerDefeat += Pool;
    }

    public void Init(IObjectPool pool)
    {
        objectPool = pool;
        if(!IgnorePlayerLifetime) PlayerSlime.OnPlayerDefeat += Pool;
    }

    private void OnDestroy()
    {
        if(!IgnorePlayerLifetime) PlayerSlime.OnPlayerDefeat -= Pool;
    }
}
