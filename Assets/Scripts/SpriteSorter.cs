using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR

[InitializeOnLoad]
#endif
public class SpriteSorter
{
    static SpriteSorter()
    {
        Initialize();
    }

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(0.0f, 1.0f, 1.0f);
    }
}