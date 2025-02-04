using UnityEngine;

public class StageInfosManager : MonoBehaviour
{
    public static StageInfosManager instance;
    public string currentStage = "Stage 1";

    public float goldSpeedTMP = 0.02f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Invoke(nameof(SetCurrentStageToMaxLevelUnlocked), 1);
    }

    void SetCurrentStageToMaxLevelUnlocked()
    {
        currentStage = "Stage " + SaveManager.instance.maxLevelUnlocked.ToString();
    }


    public void SetCurrentStageToNextStage()
    {
        //SetCurrentStageNumber(GetCurrentStageNumber() + 1);
        //currentStage = "Stage " + (GetCurrentStageNumber() + 1).ToString();
    }
    public void SetCurrentStageNumber(int number)
    {
        if (number > Constants.MAX_STAGE_NUMBER)
            return;
        currentStage = "Stage " + number.ToString();
    }
    public int GetCurrentStageNumber()
    {
        return int.Parse(currentStage.Split(' ')[1]);
    }

    public void SetCurrentStageName(string stage)
    {
        currentStage = stage;
    }

    public string GetCurrentStageName()
    {
        return currentStage;
    }

}