using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : PoolableObject
{
    [SerializeField] private Text text;
    [SerializeField] private Image bgImage;
    [SerializeField] private Image mainImage;
    [SerializeField] private Image catchImage;
    [SerializeField] private float catchSpeed;
    [SerializeField] private Vector3 offset;


    private Coroutine catchRoutine;

    public void UpdatePosition(Vector3 pos) => transform.position = pos + offset;

    public void SetHealth(float hp) => text.text = hp.ToString("0.##");

    public void ForceValue(float value)
    {
        mainImage.fillAmount = value;
        catchImage.fillAmount = value;
        if(catchRoutine is not null) StopCoroutine(catchRoutine);
    }

    public void SetValue(float value)
    {
        if(Pooled) RestoreFromPool();
        
        mainImage.fillAmount = value;
        catchRoutine ??= StartCoroutine(CatchRoutine());
    }
    
    private IEnumerator CatchRoutine()
    {
        while (Mathf.Abs(catchImage.fillAmount - mainImage.fillAmount) > 0.01f)
        {
            catchImage.fillAmount = Mathf.MoveTowards(catchImage.fillAmount, mainImage.fillAmount,
                catchSpeed * Time.deltaTime);
            yield return null;
        }

        catchImage.fillAmount = mainImage.fillAmount;
        catchRoutine = null;
    }

    public void FadeOut() => StartCoroutine(FadeRoutine());
    
    private IEnumerator FadeRoutine()
    {
        float t = 1f;
        while (t >= 0)
        {
            SetAlpha(t);   
            t -= Time.deltaTime;
            yield return null;
        }
        SetAlpha(0);
        Pool();
    }
    
    private void SetAlpha(float alpha)
    {
        bgImage.color = bgImage.color.WithAlpha(alpha);
        mainImage.color = mainImage.color.WithAlpha(alpha);
        catchImage.color = catchImage.color.WithAlpha(alpha);
    }
    
    
    // PoolableObject
    protected override void RestoreFromPool()
    {
        base.RestoreFromPool();
        mainImage.fillAmount = 1;
        catchImage.fillAmount = 1;
        SetAlpha(1f);
        gameObject.SetActive(true);
        if(catchRoutine is not null) StopCoroutine(catchRoutine);
    }
}
