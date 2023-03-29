using System.Collections;
using UnityEngine;

public class MapWalker : MonoBehaviour
{
    [SerializeField] private PlayerSlimeAnimator playerSlimeAnimator;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float minX;
    [SerializeField] private AnimationCurve speedCurve;

    public delegate void MapWalkerEvent();
    public static event MapWalkerEvent OnLocationChanged;

    private Coroutine walkRoutine;

    private void Awake() => EnemySpawner.OnEnemiesDefeated += ChangeLocation;
    private void OnDestroy() => EnemySpawner.OnEnemiesDefeated -= ChangeLocation;


    private void ChangeLocation()
    {
        if(walkRoutine is not null) return;
        walkRoutine = StartCoroutine(WalkRoutine());
    }
    
    private IEnumerator WalkRoutine()
    {
        yield return new WaitForSeconds(1f);
        Vector3 pos = Vector3.zero;
        playerSlimeAnimator.StartWalk();
        float x = 0;

        while (x > minX)
        {
            x -= Time.deltaTime * movementSpeed * speedCurve.Evaluate(Mathf.Clamp01(x / minX));

            pos.x = x;
            transform.position = pos;
            yield return null;
        }
        
        playerSlimeAnimator.StopWalk();
        ResetPosition();
        walkRoutine = null;

        yield return new WaitForSeconds(1f);
        OnLocationChanged?.Invoke();
    }
    
    private void ResetPosition() => transform.position = Vector3.zero;
}
