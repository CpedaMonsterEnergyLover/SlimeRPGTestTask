using UnityEngine;

[RequireComponent(typeof(Collider)),
 RequireComponent(typeof(Rigidbody))]
public class Bullet : PoolableObject
{
    [SerializeField] private GameObject shapeGO;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private ParticleSystem trailParticles;
    
    private Rigidbody rb;
    private Collider col;
    
    
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // Прикладывает к rigidbody силу,
    // нужную для попадания снарядом в точку target,
    // запущенным под углом 45 градусов к горизонту
    public void Shoot(Vector3 target)
    {
        if (Pooled) RestoreFromPool();

        Vector3 direction = target - transform.position;
        
        // Чтобы повернуть вектор на 45 градусов по оси X (локальной по направлению движения),
        // Достаточно указать его Y координату как расстояние до цели
        float distance = direction.magnitude;
        direction.y = distance;
        
        // С помощью формулы баллистического движения получаем силу,
        // Необходимую для точного броска в цель
        Vector3 force = direction.normalized * Mathf.Sqrt(-1 * distance * Physics.gravity.y);
        
        rb.AddForce(force, ForceMode.Impulse);
        trailParticles.Play();
    }

    // Объект столкновения нас не интересует т.к. при столкновении с любой поверхностью, указанной в матрице коллизий,
    // Снаряд должен взорваться, а логику соприкосновения с врагом реализует сам враг
    private void OnCollisionEnter(Collision _) => Explode();

    private void OnParticleSystemStopped() => Pool();

    private void Explode()
    {
        col.enabled = false;
        explosionParticles.Play();
        trailParticles.Stop();
        shapeGO.SetActive(false);
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void FixedUpdate()
    {
        if(transform.position.y <= -1f) Explode();
    }


    // PoolableObject
    protected override void RestoreFromPool()
    {
        base.RestoreFromPool();
        transform.localPosition = Vector3.zero;
        rb.velocity = Vector3.zero;
        col.enabled = true;
        shapeGO.SetActive(true);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        gameObject.SetActive(true);
    }
}
