using UnityEngine;

public class StageInfosManager : MonoBehaviour
{
    public static StageInfosManager instance;
    public string currentStage = "Stage 1";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetCurrentStageToNextStage()
    {
        currentStage = "Stage " + (GetCurrentStageNumber() + 1).ToString();
    }

    public void SetCurrentStageName(string stage)
    {
        currentStage = stage;
    }
    public void SetCurrentStageNumber(int number)
    {
        if (number > Constants.MAX_STAGE_NUMBER)
            return;
        currentStage = "Stage " + number.ToString();
    }

    public string GetCurrentStageName()
    {
        return currentStage;
    }

    public int GetCurrentStageNumber()
    {
        return int.Parse(currentStage.Split(' ')[1]);
    }
}
