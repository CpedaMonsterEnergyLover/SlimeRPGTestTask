using System.Collections;
using UnityEngine;

public class BulletSpawner : ObjectPool<Bullet>
{
    [SerializeField] private Vector3 aheadOffset;


    
    private void Awake() => EnemySpawner.OnEnemiesSpawn += StartShoot;

    private void OnDestroy() => EnemySpawner.OnEnemiesSpawn -= StartShoot;


    private void StartShoot()
    {
        StopAllCoroutines();
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        while (enabled)
        {
            if(EnemySpawner.GetClosestEnemy(out Vector3 pos))
            {
                ShootBullet(pos + aheadOffset);
                yield return new WaitForSeconds(1 / PlayerSlime.Instance.AttackSpeed);
            }
            else break;
        }
    }

    private void ShootBullet(Vector3 target) => GetPoolable(true).Shoot(target);

}
