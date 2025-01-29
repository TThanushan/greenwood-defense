using UnityEngine;

public class DisplayFPS : MonoBehaviour
{
    public int fontSize = 24;
    public Color textColor = Color.white;

    private float deltaTime;
    private GUIStyle style;

    void Start()
    {
        style = new GUIStyle
        {
            fontSize = fontSize
        };
        style.normal.textColor = textColor;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

        int w = Screen.width, h = Screen.height;

        Rect rect = new(0, 0, w, h * 2 / 100);

        GUI.Label(rect, text, style);
    }
}
