using UnityEngine;

public class TweenGameObject : MyTween
{

    [Header("Tween Color")]
    public bool tweenColor = false;
    public float tweenTimeColor;
    public Color beginColor, endColor;
    public LeanTweenType leanTweenTypeColor = LeanTweenType.punch;
    SpriteRenderer spriteRenderer;

    [Header("Tween Size")]
    public bool tweenSize = false;
    public float tweenTimeSize;
    public float maxSize;
    public LeanTweenType leanTweenTypeSize = LeanTweenType.punch;

    Vector3 startScale;

    [Header("Tween Position")]
    public bool tweenPosition = false;
    public float tweenTimePosition;
    public Vector2 newPosition;
    public LeanTweenType leanTweenTypePosition = LeanTweenType.punch;

    public enum AxisOnly { XYAxis, XAxis, YAxis }
    public AxisOnly axisOnly;

    Vector2 startPosition;

    void Start()
    {
        TweenColorInit();
        TweenSizeInit();
        TweenPositionInit();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    LeanTween.cancel(gameObject);
        //    Tween();
        //}
        //if (Input.GetKeyDown(KeyCode.L) && tweenColor)
        //{
        //    LeanTween.cancel(gameObject);
        //    TweenColor();
        //}
    }

    public override void Tween()
    {
        LeanTween.cancel(gameObject);
        if (tweenColor)
            TweenColor();
        if (tweenSize)
            TweenSize();
        if (tweenPosition)
            TweenPosition();
    }

    void TweenColorInit()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void TweenColor()
    {
        if (!spriteRenderer)
            return;
        LeanTween.value(gameObject, beginColor, endColor, tweenTimeColor)
        .setEase(leanTweenTypeColor);
    }

    void TweenSizeInit()
    {
        startScale = transform.localScale;
    }
    void TweenSize()
    {
        transform.localScale = startScale;
        LeanTween.scale(gameObject, startScale * maxSize, tweenTimeSize)
        .setEase(leanTweenTypeSize);

    }

    void TweenPositionInit()
    {
        startPosition = transform.position;
        if (axisOnly == AxisOnly.XAxis)
            newPosition.y = transform.position.y;
        if (axisOnly == AxisOnly.YAxis)
            newPosition.x = transform.position.x;
    }

    void TweenPosition()
    {
        transform.position = startPosition;

        //if (XAxisOnly)
        //    LeanTween.moveX(gameObject, newPosition.x, tweenTimePosition)
        //    .setEasePunch();
        //else if (YAxisOnly)
        //    LeanTween.moveY(gameObject, newPosition.y, tweenTimePosition)
        //    .setEasePunch();
        //else
        LeanTween.move(gameObject, newPosition, tweenTimePosition)
        .setEase(leanTweenTypePosition);
    }
}
