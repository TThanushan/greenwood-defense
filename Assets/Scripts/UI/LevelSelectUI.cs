using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    Color autoSaveEnabledColor = new Color(0f, 1f, 0.5292978f);
    private void Start()
    {
        transform.Find(Constants.LEVEL_SELECT_GOLD_TEXT_PATH).GetComponent<TMPro.TextMeshProUGUI>().text = ((int)SaveManager.instance.money).ToString();
        SetAutoSaveColor();
    }

    void SetAutoSaveColor()
    {
        Color col = Color.gray;
        if (SaveManager.instance.isAutoSave)
            col = autoSaveEnabledColor;
        transform.Find(Constants.LEVEL_SELECT_AUTO_SAVE_TEXT_PATH).GetComponent<TMPro.TextMeshProUGUI>().color = col;

    }

    public void SavePrefs()
    {
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
