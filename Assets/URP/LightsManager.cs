using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class LightsManager : MonoBehaviour
{
    [Tooltip("The percentage by which to change the light intensity (e.g., 10 for 10%).")]
    [Range(-100, 100)]
    public float intensityChangePercentage = 10.0f;

    // Method to change the intensity of all Light2D components in children
    public void ChangeIntensity()
    {
        float multiplier = 1 + (intensityChangePercentage / 100.0f);
        Light2D[] lights = GetComponentsInChildren<Light2D>();
        foreach (Light2D light in lights)
        {
            light.intensity *= multiplier;
        }
    }

    // Example method to reset lights for testing in the editor
    public void ResetIntensity()
    {
        Light2D[] lights = GetComponentsInChildren<Light2D>();
        foreach (Light2D light in lights)
        {
            light.intensity = 1.0f; // Reset to default intensity
        }
    }

    // This method is called when the script is loaded or a value is changed in the inspector
    private void OnValidate()
    {
        ChangeIntensity();
    }
}
