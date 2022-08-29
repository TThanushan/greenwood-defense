using UnityEngine;
using UnityEngine.UI;

public class SwitchStage : MonoBehaviour
{
    public string currentStage;
    TMPro.TextMeshProUGUI stageTitle;
    TMPro.TextMeshProUGUI completed;
    Transform starsPanel;
    SaveManager saveManager;
    LevelScore levelScore;

    Color disabledButtonColor = new Color(255, 0, 0);
    Color enableButtonColor = new Color(0, 255, 0);
    Image imageNext;
    Image imagePrevious;

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
        imageNext = transform.Find("TopGroup/NextLevel/Image").GetComponent<Image>();
        imagePrevious = transform.Find("TopGroup/PreviousLevel/Image").GetComponent<Image>();

        SelectPreviousStage();
        UpdateButtonColor();
    }

    public void LoadCurrentStage()
    {
        MenuScript.instance.LoadStageScene();
    }

    public void SelectPreviousStage()
    {
        //string[] splitted = currentStage.Split(' ');
        //string newStage = splitted[0] + ' ' + (int.Parse(splitted[1]) - 1);
        //if (!IsStageUnlocked(newStage) || !DoesStageExist(newStage))
        //    return;
        string newStage = DoesStageExist(false);
        if (newStage is null)
            return;
        SetNewStage(newStage);
        UpdateStars();
        UpdateButtonColor();
    }

    public void SelectNextStage()
    {
        //string[] splitted = currentStage.Split(' ');
        //string newStage = splitted[0] + ' ' + (int.Parse(splitted[1]) + 1);
        //if (!IsStageUnlocked(newStage) || !DoesStageExist(newStage))
        //    return;
        string newStage = DoesStageExist(true);
        if (newStage is null)
            return;

        SetNewStage(newStage);
        UpdateStars();
        UpdateButtonColor();
    }

    string DoesStageExist(bool nextStage)
    {
        int i = -1;
        if (nextStage)
            i = 1;
        string[] splitted = currentStage.Split(' ');
        string newStage = splitted[0] + ' ' + (int.Parse(splitted[1]) + i);

        if (!IsStageUnlocked(newStage))
            return null;
        return newStage;
    }

    public void UpdateButtonColor()
    {
        if (DoesStageExist(true) is null)
            imageNext.color = disabledButtonColor;
        else
            imageNext.color = enableButtonColor;


        if (DoesStageExist(false) is null)
            imagePrevious.color = disabledButtonColor;
        else
            imagePrevious.color = enableButtonColor;
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

    //void GetStageStarsNumber()
    //{
    //    saveManager.GetLevelScore();

    //}

    void SetNewStage(string newStage)
    {
        currentStage = newStage;
        stageTitle.text = newStage;
        StageInfosManager.instance.SetCurrentStageName(newStage);
    }

    //bool IsStageCompleted(string stage)
    //{
    //    string[] splitted = stage.Split(' ');
    //    int nb = int.Parse(splitted[1]);
    //    return nb < saveManager.maxLevelUnlocked;
    //}

    bool IsStageUnlocked(string stage)
    {
        //return stage == SaveManager.instance.GetLatestStageUnlocked();
        string[] splitted = stage.Split(' ');
        int nb = int.Parse(splitted[1]);
        return nb > 0 && nb <= saveManager.maxLevelUnlocked;
    }
}
