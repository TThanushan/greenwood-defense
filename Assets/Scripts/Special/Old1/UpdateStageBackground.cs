using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class UpdateStageBackground : MonoBehaviour
{
    static UpdateStageBackground instance;
    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        if (!instance)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    private void Start()
    {
        UpdateBackground();

    }

    private void SceneLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name.Equals("Menu"))
        {
            Destroy(gameObject);
        }
    }

    public void EnableCorrectStageBackground()
    {
        int currentStage = StageInfosManager.instance.GetCurrentStageNumber();
        if (currentStage < 10)
            transform.Find("VastForest").gameObject.SetActive(true);
        else if (currentStage < 20)
            transform.Find("SnowyCanyon").gameObject.SetActive(true);
        else if (currentStage < 30)
            transform.Find("CrystalCave").gameObject.SetActive(true);
        else if (currentStage < 40)
            transform.Find("TreasureCave").gameObject.SetActive(true);
        else if (currentStage < 50)
            transform.Find("ChurchInAutumn").gameObject.SetActive(true);
        else if (currentStage < 60)
            transform.Find("SunnyBeach").gameObject.SetActive(true);
        else if (currentStage < 70)
            transform.Find("Desert").gameObject.SetActive(true);
        else if (currentStage < 80)
            transform.Find("SunshineForest").gameObject.SetActive(true);
        else if (currentStage < 90)
            transform.Find("PoisonCave").gameObject.SetActive(true);
        else if (currentStage <= 100)
            transform.Find("Purgatory").gameObject.SetActive(true);

    }

    public void DisableAllStageBackground()
    {
        foreach (Transform child in transform)
        {
            // Disable the child object
            child.gameObject.SetActive(false);
        }
    }

    // Make a method that will find a gameobject named Global Volume, and set the Bloom component tint color to the given color in the parameter
    public void SetBloomColor(Color color)
    {
        GameObject globalVolume = GameObject.Find("Global Volume");
        Bloom bloom;

        if (globalVolume && globalVolume.GetComponent<Volume>().profile.TryGet(out bloom))
        {
            bloom.tint.value = color;
            //UnityEngine.Rendering.Universal.Bloom bloom = globalVolume.GetComponent<UnityEngine.Rendering.Universal.Bloom>();
            //if (bloom)
            //{
            //    bloom.tint.value = color;
            //}
        }
    }

    // Make a method that will set the bloom tint color to the color of the current stage
    public void SetBloomColorToCurrentStage()
    {
        int currentStage = StageInfosManager.instance.GetCurrentStageNumber();
        if (currentStage < 10)
            SetBloomColor(new Color(0.7541463f, 0.5431266f, 1f));
        else if (currentStage < 20)
            SetBloomColor(new Color(0.7749326f, 0.828716f, 1f));
        else if (currentStage < 30)
            SetBloomColor(new Color(1f, 0.6320754f, 0.9869336f));
        else if (currentStage < 40)
            SetBloomColor(new Color(1f, 0.8268844f, 0.6933962f));
        else if (currentStage < 50)
            SetBloomColor(new Color(1f, 0.7955664f, 0.7857142f));
        else if (currentStage < 60)
            SetBloomColor(new Color(1f, 1f, 1f));
        else if (currentStage < 70)
            SetBloomColor(new Color(0.7311321f, 0.7691063f, 1f));
        else if (currentStage < 80)
            SetBloomColor(new Color(0.5176471f, 0.3882353f, 1f));
        else if (currentStage < 90)
            SetBloomColor(new Color(0.1496544f, 0f, 1f));
        else if (currentStage <= 100)
            SetBloomColor(new Color(1f, 0.6455525f, 0.6665052f));
    }

    public void UpdateBackground()
    {
        DisableAllStageBackground();
        EnableCorrectStageBackground();
        SetBloomColorToCurrentStage();
    }
}
