using UnityEngine;

public class TweenOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<MyTween>().Tween();
    }
}
