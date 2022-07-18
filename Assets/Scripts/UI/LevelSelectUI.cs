using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    Color autoSaveEnabledColor = new Color(255, 0, 165);
    private void Start()
    {
        transform.Find("LeftGroup/GoldText").GetComponent<TMPro.TextMeshProUGUI>().text = "Money : " + SaveManager.instance.money.ToString();

        SetAutoSaveColor();
    }

    void SetAutoSaveColor()
    {
        Color col = Color.gray;
        if (SaveManager.instance.isAutoSave)
            col = autoSaveEnabledColor;
        transform.Find("MiddleGroup/Buttons/AutoSave/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().color = col;

    }

    public void SavePrefs()
    {
        Debug.LogError("levelSelectUI save pref");
        SaveManager.instance.SavePrefs();
        LoadLastSave();
    }

    public void ReverseIsAutoSave()
    {
        SaveManager saveManager = SaveManager.instance;
        saveManager.isAutoSave = !saveManager.isAutoSave;
        saveManager.SaveIsAutoSave();
        SetAutoSaveColor();
    }


    public void LoadLastSave()
    {
        SaveManager.instance.LoadPrefs();
        MenuScript.instance.LoadCurrentScene();
    }
}
