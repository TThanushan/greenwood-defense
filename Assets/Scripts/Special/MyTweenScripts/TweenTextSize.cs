using UnityEngine;

public class TweenTextSize : MyTween
{
    public float tweenTime;
    public float maxSize;

    Vector3 startScale;
    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {

    }

    public override void Tween()
    {
        LeanTween.cancel(gameObject);
        transform.localScale = startScale;
        LeanTween.scale(gameObject, startScale * maxSize, tweenTime)
        .setEasePunch();

    }
}
