using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour, IObjectPool where T : PoolableObject
{
    [SerializeField] private T poolablePrefab;
    
    private readonly Stack<T> pool = new();

    
    
    protected T GetPoolable(bool attachToSelf, bool ignorePlayerLifetime = false) 
        => pool.TryPop(out T o) ? o : CreatePoolable(attachToSelf, ignorePlayerLifetime);

    private T CreatePoolable(bool attachToSelf, bool ignorePlayerLifetime)
    {
        var inst = Instantiate(poolablePrefab);
        inst.IgnorePlayerLifetime = ignorePlayerLifetime;
        inst.Init(this);
        if (attachToSelf) inst.AttachTo(transform);
        return inst;
    }

    public void PoolObject(PoolableObject o)
    {
        pool.Push((T) o);
    }
}
