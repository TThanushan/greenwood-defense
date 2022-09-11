using TMPro;
using UnityEngine;

public class DisplayFPS : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public float deltaTime;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        transform.Find("Canvas/FPSText").gameObject.SetActive(true);
        fpsText = transform.Find("Canvas/FPSText/TextValue").GetComponent<TextMeshProUGUI>();

    }
    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString();
    }
}
