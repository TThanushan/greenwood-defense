using UnityEngine;

public class MoveCameraWhenOverMe : MonoBehaviour
{

    public float xLimit;
    public float translationSpeed;
    Camera camera;
    TweenPosition tweenPosition;
    private void Awake()
    {
        camera = Camera.main;
        //tweenPosition = transform.GetComponent<TweenPosition>();
    }

    private void OnMouseOver()
    {
        TranslateCamera();
    }
    public void TranslateCamera()
    {
        if ((translationSpeed > 0 && Camera.main.transform.position.x < xLimit) || (translationSpeed < 0 && Camera.main.transform.position.x > xLimit))
            camera.transform.Translate(translationSpeed, 0, 0);
        //tweenPosition.Tween();

    }
}
