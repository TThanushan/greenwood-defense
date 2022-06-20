using System.Collections;
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

    void Start()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
        Init();

    }

    void Init()
    {
        fadeAnimator = transform.Find("MiddleGroup/FadeSceneBody").transform.GetComponent<Animator>();
        blackSquareFade = transform.Find("MiddleGroup/FadeSceneBody/Canvas/blackSquareFade").GetComponent<Image>();
    }

    IEnumerator FadeInScene(int sceneIndex)
    {
        fadeAnimator.Play("FadeIn");
        yield return new WaitUntil(() => blackSquareFade.color == Color.black);
        //yield return new WaitUntil(() => loadingBarTransform.localPosition.x == 0);
        SceneManager.LoadScene(sceneIndex);
    }
    public void LoadScene(int sceneIndex)
    {
        ResetTimeScale();
        StartCoroutine(FadeInScene(sceneIndex));
    }

    public void LoadScene(string sceneName)
    {
        int index = SceneUtility.GetBuildIndexByScenePath(sceneName);
        LoadScene(index);
    }

    public void LoadLevelSelectionScene()
    {
        LoadScene(levelSelectionSceneIndex);
    }

    public void LoadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }
}
