using UnityEngine;
using UnityEngine.UI;

public class TweenLevelSelectionLevelButton : MonoBehaviour
{
    public Scrollbar scrollBar;
    public float valueAdded;
    public float swipeTime;
    public LeanTweenType leanTweenType;

    public void SwipeRight()
    {
        float min = scrollBar.value;
        float max = min + valueAdded;
        LeanTween.value(min, max, swipeTime)
        .setEase(leanTweenType)
        .setOnUpdate((float val) =>
        {
            scrollBar.value = val;
        });
    }

    public void SwipeLeft()
    {
        float min = scrollBar.value;
        float max = min - valueAdded;
        LeanTween.value(min, max, swipeTime).setEase(leanTweenType).setOnUpdate((float val) =>
        {
            scrollBar.value = val;
        });
    }
}
