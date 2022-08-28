using UnityEngine;

public class TweenSize : MyTween
{
    public float tweenTimeSize;
    public float maxSize;
    public bool useNewStartSize;
    public Vector3 newStartSize;
    public LeanTweenType leanTweenType;
    Vector3 startScale;

    protected override void Awake()
    {
        base.Awake();
        if (useNewStartSize)
            startScale = newStartSize;
        else
            startScale = tweenedObject.transform.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            LeanTween.cancel(tweenedObject);
            Tween();
        }
    }
    public override void Tween()
    {
        tweenedObject.transform.localScale = startScale;
        LeanTween.scale(tweenedObject, startScale * maxSize, tweenTimeSize)
        .setEase(leanTweenType);

    }
}
