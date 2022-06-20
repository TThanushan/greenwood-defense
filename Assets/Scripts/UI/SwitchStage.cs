using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchStage : MonoBehaviour
{
    public string currentStage;
    TMPro.TextMeshProUGUI stageTitle;
    TMPro.TextMeshProUGUI completed;


    private void Start()
    {
        stageTitle = transform.Find("TopGroup/Title").GetComponent<TMPro.TextMeshProUGUI>();
        currentStage = "Stage " + SaveManager.instance.maxLevelUnlocked.ToString();
        completed = transform.Find("TopGroup/Completed").GetComponent<TMPro.TextMeshProUGUI>();
        stageTitle.text = currentStage;
    }

    public void LoadCurrentStage()
    {
        MenuScript.instance.LoadScene(currentStage);
    }

    public void SelectPreviousStage()
    {
        string[] splitted = currentStage.Split(' ');
        string newStage = splitted[0] + ' ' + (int.Parse(splitted[1]) - 1);
        SetNewStage(newStage);
        UpdateCompletedText(IsStageCompleted(newStage));

    }
    public void SelectNextStage()
    {
        string[] splitted = currentStage.Split(' ');
        string newStage = splitted[0] + ' ' + (int.Parse(splitted[1]) + 1);
        SetNewStage(newStage);
        UpdateCompletedText(IsStageCompleted(newStage));
    }

    void UpdateCompletedText(bool val)
    {
        string text = "";
        if (val)
            text = "Completed";
        completed.text = text;
    }

    void SetNewStage(string newStage)
    {
        if (!IsStageUnlocked(newStage) || SceneManager.GetSceneByName(newStage) == null)
            return;
        currentStage = newStage;
        stageTitle.text = newStage;


    }

    bool IsStageCompleted(string stage)
    {
        string[] splitted = stage.Split(' ');
        int nb = int.Parse(splitted[1]);
        return nb < SaveManager.instance.maxLevelUnlocked;
    }

    bool IsStageUnlocked(string stage)
    {
        //return stage == SaveManager.instance.GetLatestStageUnlocked();
        string[] splitted = stage.Split(' ');
        int nb = int.Parse(splitted[1]);
        return nb > 0 && nb <= SaveManager.instance.maxLevelUnlocked;
    }
}
