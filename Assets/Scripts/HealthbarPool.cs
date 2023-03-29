public class HealthbarPool : ObjectPool<Healthbar>
{
    public Healthbar CreateHealthbar(bool playerHealthbar = false) => GetPoolable(true, playerHealthbar);
}
