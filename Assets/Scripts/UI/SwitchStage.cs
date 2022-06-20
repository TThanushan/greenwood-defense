using UnityEngine;

public class SwitchStage : MonoBehaviour
{
    public string currentStage;
    TMPro.TextMeshProUGUI stageTitle;
    TMPro.TextMeshProUGUI completed;
    Transform starsPanel;
    SaveManager saveManager;
    LevelScore levelScore;
    private void Start()
    {
        saveManager = SaveManager.instance;
        levelScore = LevelScore.instance;
        stageTitle = transform.Find("TopGroup/Title").GetComponent<TMPro.TextMeshProUGUI>();
        currentStage = "Stage " + saveManager.maxLevelUnlocked.ToString();
        StageInfosManager.instance.SetCurrentStageName(currentStage);
        completed = transform.Find("TopGroup/Completed").GetComponent<TMPro.TextMeshProUGUI>();
        stageTitle.text = currentStage;
        starsPanel = transform.Find("TopGroup/Stars/Panel");

    }

    public void LoadCurrentStage()
    {
        MenuScript.instance.LoadStageScene();
    }

    public void SelectPreviousStage()
    {
        string[] splitted = currentStage.Split(' ');
        string newStage = splitted[0] + ' ' + (int.Parse(splitted[1]) - 1);
        if (!IsStageUnlocked(newStage) || !DoesStageExist(newStage))
            return;
        SetNewStage(newStage);
        UpdateStars();
    }
    public void SelectNextStage()
    {
        string[] splitted = currentStage.Split(' ');
        string newStage = splitted[0] + ' ' + (int.Parse(splitted[1]) + 1);
        if (!IsStageUnlocked(newStage) || !DoesStageExist(newStage))
            return;

        SetNewStage(newStage);
        UpdateStars();
    }


    bool DoesStageExist(string stage)
    {
        return Resources.Load("Stages/" + stage) != null;
    }
    void UpdateStars()
    {
        float score = saveManager.GetLevelScore();
        int starsNumber = levelScore.HowManyStar((int)score);


        starsPanel.Find("Star1/In").gameObject.SetActive(false);
        starsPanel.Find("Star2/In").gameObject.SetActive(false);
        starsPanel.Find("Star3/In").gameObject.SetActive(false);

        if (starsNumber == 1)
        {
            starsPanel.Find("Star1/In").gameObject.SetActive(true);
        }
        if (starsNumber == 2)
        {
            starsPanel.Find("Star1/In").gameObject.SetActive(true);
            starsPanel.Find("Star2/In").gameObject.SetActive(true);
        }
        if (starsNumber == 3)
        {
            starsPanel.Find("Star1/In").gameObject.SetActive(true);
            starsPanel.Find("Star2/In").gameObject.SetActive(true);
            starsPanel.Find("Star3/In").gameObject.SetActive(true);
        }
    }

    void GetStageStarsNumber()
    {
        saveManager.GetLevelScore();

    }

    void SetNewStage(string newStage)
    {
        currentStage = newStage;
        stageTitle.text = newStage;
        StageInfosManager.instance.SetCurrentStageName(newStage);
    }

    bool IsStageCompleted(string stage)
    {
        string[] splitted = stage.Split(' ');
        int nb = int.Parse(splitted[1]);
        return nb < saveManager.maxLevelUnlocked;
    }

    bool IsStageUnlocked(string stage)
    {
        //return stage == SaveManager.instance.GetLatestStageUnlocked();
        string[] splitted = stage.Split(' ');
        int nb = int.Parse(splitted[1]);
        return nb > 0 && nb <= saveManager.maxLevelUnlocked;
    }
}
