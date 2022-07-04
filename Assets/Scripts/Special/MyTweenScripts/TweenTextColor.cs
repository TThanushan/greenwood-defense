using TMPro;
using UnityEngine;
public class TweenTextColor : MyTween
{
    TextMeshProUGUI text;
    //Image text;
    public float tweenTime;
    public Color beginColor, endColor;


    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }


    public override void Tween()
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 0.1f, 1, tweenTime)
            .setEasePunch()
            .setOnUpdate((value) =>
            {
                text.color = Color.Lerp(beginColor, endColor, value);
            });
    }
}
