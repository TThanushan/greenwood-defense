using UnityEngine;

public class TweenSize : MyTween
{
    public float tweenTimeSize;
    public float maxSize;
    public bool useNewStartSize;
    public Vector3 newStartSize;
    public LeanTweenType leanTweenType;
    Vector3 startScale;

    void Awake()
    {
        if (useNewStartSize)
            startScale = newStartSize;
        else
            startScale = transform.localScale;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    LeanTween.cancel(gameObject);
        //    Tween();
        //}
    }
    public override void Tween()
    {
        transform.localScale = startScale;
        LeanTween.scale(gameObject, startScale * maxSize, tweenTimeSize)
        .setEase(leanTweenType);

    }
}
