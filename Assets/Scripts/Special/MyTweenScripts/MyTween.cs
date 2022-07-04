using UnityEngine;

public class MyTween : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    public virtual void Tween()
    { }

    /// <summary>
    /// Call Tween method on all childs MyTween scripts inherited.
    /// </summary>
    public void TweenChilds()
    {
        MyTween[] myTweens = GetComponentsInChildren<MyTween>();
        foreach (MyTween myTween in myTweens)
        {
            myTween.Tween();
        }
    }
}
