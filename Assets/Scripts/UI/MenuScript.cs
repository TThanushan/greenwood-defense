using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public static MenuScript instance;
    Animator fadeAnimator;
    // Transform loadingBarTransform;
    Image blackSquareFade;
    const int levelSelectionSceneIndex = 1;

    const string STAGE_SCENE_NAME = "Stage";

    SFXManager audioManager;


    // FOR TESTING PURPOSES
    Color layer0ColorSave;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
#if UNITY_ANDROID
        Application.targetFrameRate = 60;
#endif

        audioManager = SFXManager.instance;
        Init();

    }


    public void SaveIfAutoSave()
    {
        SaveManager.instance.SavePrefIfAutoSave();
    }
    void Init()
    {
        fadeAnimator = transform.Find("MiddleGroup/FadeSceneBody").transform.GetComponent<Animator>();
        if (!fadeAnimator.gameObject.activeSelf)
            fadeAnimator.gameObject.SetActive(true);
        blackSquareFade = transform.Find("MiddleGroup/FadeSceneBody/Canvas/blackSquareFade").GetComponent<Image>();
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;
        print("Time Resumed" + Time.timeScale);
    }

    public void PlaySfx(string sfxName)
    {
        audioManager.Play(sfxName);
    }
    IEnumerator FadeInScene(int sceneIndex)
    {
        //if (fadeAnimator == null)
        //    yield return null;
        fadeAnimator.Play("FadeIn");
        yield return new WaitUntil(() => blackSquareFade.color == Color.black);
        //yield return new WaitUntil(() => loadingBarTransform.localPosition.x == 0);
        SceneManager.LoadScene(sceneIndex);
    }

    public void MuteSFX()
    {
        audioManager.MuteSFX();
    }

    //// METHOD FOR TESTING PURPOSES
    public void EnableDisablePerformanceDisplay()
    {
        GameObject performanceDisplay = GameObject.Find("ShareBetweenScenes");
        performanceDisplay.GetComponent<PerformanceDisplay>().enabled = !performanceDisplay.GetComponent<PerformanceDisplay>().enabled;
    }

    //// METHOD FOR TESTING PURPOSES
    public void SetLayer0ColorWhiteOrGray()
    {
        var allObjects = Object.FindObjectsByType<GameObject>((FindObjectsSortMode)FindObjectsInactive.Exclude);

        List<GameObject> layer0s = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Layer0")
            {
                layer0s.Add(obj);
            }
        }

        Color color = Color.white;
        if (layer0s[0].GetComponent<SpriteRenderer>().color == Color.white)
            //color = new Color(0.517f, 0.517f, 0.517f);
            color = layer0ColorSave;
        foreach (GameObject layer0 in layer0s)
        {

            SpriteRenderer spriteRenderer = layer0.GetComponent<SpriteRenderer>();
            if (spriteRenderer.color != Color.white)
                layer0ColorSave = spriteRenderer.color;
            spriteRenderer.color = color;
        }
    }

    public void SetTargetFramerate(int targetFramerate)
    {
        Application.targetFrameRate = targetFramerate;
    }

    public void LoadMaxLevelUnlockedSave()
    {
        SaveManager.instance.LoadMaxLevelUnlockedSave();
    }

    public void LoadScene(int sceneIndex)
    {
        //if (SceneManager.GetActiveScene().name == "Stage" && SceneManager.GetSceneByBuildIndex(sceneIndex).name != "Stage")
        //    TrackPlayer.instance.PlayMenuTheme();
        ResetTimeScale();
        _ = StartCoroutine(FadeInScene(sceneIndex));
    }

    public void LoadScene(string sceneName)
    {
        int index = SceneUtility.GetBuildIndexByScenePath(sceneName);
        //if (SceneManager.GetActiveScene().name == "Stage" && sceneName != "Stage")
        //    TrackPlayer.instance.PlayMenuTheme();

        LoadScene(index);

    }

    public void LoadStageScene()
    {
        LoadScene(STAGE_SCENE_NAME);
        //TrackPlayer.instance.PlayMainTheme();

    }

    public void LoadLevelSelectionScene()
    {
        LoadScene(levelSelectionSceneIndex);
        //TrackPlayer.instance.PlayMenuTheme();

    }

    public void LoadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void DisablePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
    public void EnablePanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void ReversePanelActive(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }

    void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    public void SpeedGame(GameObject _gameObject)
    {
        float maxSpeed = 2f;
        if (Time.timeScale != maxSpeed)
        {
            Time.timeScale = maxSpeed;
            _gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            _gameObject.SetActive(false);
        }
    }

    public void ResetSave()
    {
        SaveManager.instance.ResetPlayerPrefs();
    }

    public void ChangeMode()
    {
        SaveManager.instance.chosenMode = SaveManager.instance.chosenMode == "ModeFrog" ? "ModeNormal" : "ModeFrog";
    }

}
