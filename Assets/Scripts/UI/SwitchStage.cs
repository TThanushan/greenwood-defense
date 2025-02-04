using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchStage : MonoBehaviour
{
    public static SwitchStage instance;
    public string currentStage;
    TMPro.TextMeshProUGUI stageTitle;
    Transform starsPanel;
    SaveManager saveManager;
    LevelScore levelScore;

    Color disabledButtonColor = new Color(255, 0, 0);
    Color enableButtonColor = new Color(0, 255, 0);
    Image imageNext;
    Image imagePrevious;

    UpdateStageBackground updateStageBackground;
    private void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
        updateStageBackground = GameObject.Find("Backgrounds").GetComponent<UpdateStageBackground>();

        saveManager = SaveManager.instance;
        levelScore = LevelScore.instance;
        stageTitle = transform.Find(Constants.LEVEL_SELECT_STAGE_TITLE_TEXT_PATH).GetComponent<TMPro.TextMeshProUGUI>();

        currentStage = "Stage " + saveManager.maxLevelUnlocked.ToString();

        StageInfosManager.instance.SetCurrentStageName(currentStage);

        stageTitle.text = currentStage;
        starsPanel = transform.Find(Constants.LEVEL_SELECT_STARS_PANEL_PATH);
        imageNext = transform.Find(Constants.LEVEL_SELECT_NEXT_LEVEL_IMAGE_PATH).GetComponent<Image>();
        imagePrevious = transform.Find(Constants.LEVEL_SELECT_PREVIOUS_LEVEL_IMAGE_PATH).GetComponent<Image>();
        updateStageBackground.UpdateBackground();

        UpdateButtonColor();
        UpdateStars();
    }


    public void LoadCurrentStage()
    {
        //TrackPlayer.instance.StopTrack();
        MenuScript.instance.LoadStageScene();
        Destroy(GameObject.FindGameObjectWithTag("Backgrounds"));
        //TrackPlayer.instance.StartPlayingStageTracks();
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

        return !IsStageUnlocked(newStage) ? null : newStage;
    }

    public void UpdateButtonColor()
    {
        imageNext.color = DoesStageExist(true) is null ? disabledButtonColor : enableButtonColor;


        imagePrevious.color = DoesStageExist(false) is null ? disabledButtonColor : enableButtonColor;
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
    public void LoadStage(int stageNumber)
    {
        //// Ensure the stage number is valid
        //if (stageNumber <= 0 || stageNumber > Constants.MAX_STAGE_NUMBER)
        //{
        //    Debug.LogError("Invalid stage number: " + stageNumber);
        //    return;
        //}

        //// Set the stage number in StageInfosManager
        //StageInfosManager.instance.SetCurrentStageNumber(stageNumber);

        //// Load the single "Stage" scene
        SceneManager.LoadScene("Stage");
    }

    //public void LoadCurrentStage()
    //{
    //    if (string.IsNullOrEmpty(currentStage))
    //    {
    //        Debug.LogError("Current stage is not set!");
    //        return;
    //    }

    //    // Extract the stage number from "Stage X"
    //    string[] splitStage = currentStage.Split(' ');
    //    if (splitStage.Length < 2 || !int.TryParse(splitStage[1], out int stageNumber))
    //    {
    //        Debug.LogError("Invalid stage format: " + currentStage);
    //        return;
    //    }

    //    // Set the stage number in StageInfosManager
    //    StageInfosManager.instance.SetCurrentStageNumber(stageNumber);
    //    // Load the stage scene
    //    SceneManager.LoadScene("Stage");
    //}

    //void GetStageStarsNumber()
    //{
    //    saveManager.GetLevelScore();

    //}

    void SetNewStage(string newStage)
    {
        currentStage = newStage;
        stageTitle.text = newStage;
        StageInfosManager.instance.SetCurrentStageName(newStage);
        updateStageBackground.UpdateBackground();
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
