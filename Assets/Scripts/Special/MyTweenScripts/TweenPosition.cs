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
    void Awake()
    {

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
        startPosition = transform.position;
        if (axisOnly == AxisOnly.XAxis)
            newPosition.y = transform.position.y;
        if (axisOnly == AxisOnly.YAxis)
            newPosition.x = transform.position.x;
        if (startFromNewPosition)
        {
            Vector2 tmp = new Vector2(startPosition.x, startPosition.y);
            startPosition = new Vector2(newPosition.x, newPosition.y);
            newPosition = tmp;
        }
    }

    public override void Tween()
    {
        transform.position = startPosition;

        LeanTween.move(gameObject, newPosition, tweenTimePosition)
        .setEase(leanTweenType);
    }
}
