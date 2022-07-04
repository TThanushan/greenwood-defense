using TMPro;
using UnityEngine;
public class TweenText : MyTween
{
    TextMeshProUGUI text;

    [Header("Tween Color")]
    public bool tweenColor = false;
    public float tweenTimeColor;
    public Color beginColor, endColor;

    [Header("Tween Size")]
    public bool tweenSize = false;
    public float tweenTimeSize;
    public float maxSize;

    Vector3 startScale;



    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        TweenColorInit();
        TweenSizeInit();
    }

    void Update()
    {

    }

    public override void Tween()
    {
        LeanTween.cancel(gameObject);
        if (tweenColor)
            TweenColor();
        if (tweenSize)
            TweenSize();
    }

    void TweenColorInit()
    {

    }
    void TweenColor()
    {
        LeanTween.value(gameObject, 0.1f, 1, tweenTimeColor)
            .setEasePunch()
            .setOnUpdate((value) =>
            {
                text.color = Color.Lerp(beginColor, endColor, value);
            });
    }
    void TweenSizeInit()
    {
        startScale = transform.localScale;
    }
    void TweenSize()
    {
        transform.localScale = startScale;
        LeanTween.scale(gameObject, startScale * maxSize, tweenTimeSize)
        .setEasePunch();

    }
}
