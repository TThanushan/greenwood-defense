using UnityEngine;

public class TweenPosition : MyTween
{
    public float tweenTimePosition;
    public Vector2 newPosition;
    public bool startFromNewPosition;
    public LeanTweenType leanTweenType;
    public enum AxisOnly { XYAxis, XAxis, YAxis }
    public AxisOnly axisOnly;


    Vector2 startPosition;

    protected override void Awake()
    {
        base.Awake();
        TweenPositionInit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            LeanTween.cancel(gameObject);
            Tween();
        }
    }

    void TweenPositionInit()
    {
        startPosition = tweenedObject.transform.localPosition;
        if (axisOnly == AxisOnly.XAxis)
            newPosition.y = tweenedObject.transform.localPosition.y;
        if (axisOnly == AxisOnly.YAxis)
            newPosition.x = tweenedObject.transform.localPosition.x;
        if (startFromNewPosition)
        {
            Vector2 tmp = new Vector2(startPosition.x, startPosition.y);
            startPosition = new Vector2(newPosition.x, newPosition.y);
            newPosition = tmp;
        }
    }

    public override void Tween()
    {
        LeanTween.cancel(gameObject);

        tweenedObject.transform.localPosition = startPosition;

        LeanTween.move(tweenedObject, newPosition, tweenTimePosition)
        .setEase(leanTweenType);
    }
}
