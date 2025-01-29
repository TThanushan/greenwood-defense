using UnityEngine;

public class PerformanceDisplay : MonoBehaviour
{
    public static PerformanceDisplay instance;
    private float deltaTime = 0.0f;
    public float fps = 0.0f;
    private string memoryUsage = "";
    private float frameTime = 0.0f;
    private float maxFPS = 0.0f;
    private float minFPS = float.MaxValue;
    private float totalFPS = 0.0f;
    private int frameCount = 0;
    private float startTime; // Variable to store the start time

    // Android specific variables
    private AndroidJavaObject activity;
    private AndroidJavaObject activityManager;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        startTime = Time.time; // Record the start time at the beginning of the scene
        InvokeRepeating("Reset", 0.0f, 10.0f); // Reset min/max/avg FPS every 10 seconds

        // Initialize Android specific objects
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                activityManager = activity.Call<AndroidJavaObject>("getSystemService", "activity");

            }
        }

    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
        frameTime = deltaTime * 1000.0f; // Convert to milliseconds
        memoryUsage = $"Memory Usage: {System.GC.GetTotalMemory(false) / (1024 * 1024)} MB";

        // Track maximum, minimum, and average FPS
        if (fps > maxFPS)
        {
            maxFPS = fps;
        }
        if (fps < minFPS)
        {
            minFPS = fps;
        }
        totalFPS += fps;
        frameCount++;

    }

    private void Reset()
    {
        maxFPS = 0.0f;
        minFPS = float.MaxValue;
        totalFPS = 0.0f;
        frameCount = 0;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();
        Rect rect = new Rect(10, 5, w - 20, 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        style.normal.background = MakeTexture(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.5f));

        float timeSinceStart = Time.time - startTime; // Calculate time elapsed since start
        float avgFPS = totalFPS / frameCount;
        string temperature = GetTemperature();
        string text = $"FPS: {fps:0.} Max FPS: {maxFPS:0.} Min FPS: {minFPS:0.}\nAvg FPS: {avgFPS:0.0} ms {memoryUsage}\nTemperature: {temperature}\nTime Elapsed: {timeSinceStart:0.0} seconds";

        GUI.Label(rect, text, style);

        // Log for debugging
    }


    string GetTemperature()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaObject batteryIntent = activity.Call<AndroidJavaObject>("registerReceiver", null, new AndroidJavaObject("android.content.IntentFilter", "android.intent.action.BATTERY_CHANGED")))
                {
                    int temperature = batteryIntent.Call<int>("getIntExtra", "temperature", -1);
                    return $"{temperature / 10.0f}°C";
                }
            }
            catch (System.Exception)
            {

                return "Error";
            }
        }
        return "N/A";
    }

    Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;

        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
