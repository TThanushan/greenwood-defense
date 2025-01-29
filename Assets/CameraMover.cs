using UnityEngine;

public class CameraMover : MonoBehaviour
{
    // The target transform you want to move the camera to
    public Transform targetTransform;
    // Duration of the movement
    public float duration = 2f;
    // Optional: Easing type
    public LeanTweenType easeType = LeanTweenType.easeInOutQuad;
    // Store the initial position of the camera
    private Vector3 initialPosition;

    void OnEnable()
    {
        LeanTween.reset();
        // Store the initial position of the camera
        initialPosition = transform.position;
        // Initiate the camera movement
        MoveCamera();
    }

    void MoveCamera()
    {
        if (targetTransform != null)
        {
            // Reset the camera position to the initial position
            transform.position = initialPosition;
            // Use LeanTween to move the camera to the target position smoothly
            _ = LeanTween.move(gameObject, targetTransform.position, duration).setEase(easeType);
        }
        else
        {
            Debug.LogError("Target Transform is not assigned.");
        }
    }
}
