using UnityEngine;

public class ForceAspectRatio : MonoBehaviour
{
    private float targetAspect = 2.0f; // 18:9 (2:1 aspect ratio)
    private Camera mainCam;
    private Camera uiCam;

    void Start()
    {
        mainCam = GetComponent<Camera>();
        uiCam = transform.Find("UICamera")?.GetComponent<Camera>(); // Get the UI Camera

        ApplyAspectRatio(mainCam);
        if (uiCam != null) ApplyAspectRatio(uiCam);
    }

    void ApplyAspectRatio(Camera cam)
    {
        if (cam == null) return;

        float screenAspect = Screen.width / (float)Screen.height;
        float scaleHeight = screenAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Letterboxing (black bars on top and bottom)
            cam.rect = new Rect(0, (1.0f - scaleHeight) / 2.0f, 1.0f, scaleHeight);
        }
        else
        {
            // Pillarboxing (black bars on the sides)
            float scaleWidth = 1.0f / scaleHeight;
            cam.rect = new Rect((1.0f - scaleWidth) / 2.0f, 0, scaleWidth, 1.0f);
        }
    }
}
