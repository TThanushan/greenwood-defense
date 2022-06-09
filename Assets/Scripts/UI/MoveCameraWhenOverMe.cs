using UnityEngine;

public class MoveCameraWhenOverMe : MonoBehaviour
{

    public float xLimit;
    public float translationSpeed;


    private void OnMouseOver()
    {
        TranslateCamera();
    }
    public void TranslateCamera()
    {
        if ((translationSpeed > 0 && Camera.main.transform.position.x < xLimit) || (translationSpeed < 0 && Camera.main.transform.position.x > xLimit))
            Camera.main.transform.Translate(translationSpeed, 0, 0);
    }
}
