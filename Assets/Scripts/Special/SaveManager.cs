using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public int maxLevelUnlocked = 1;
    public List<Level> levels;
    public const int levelsCount = 10;

    const string MaxLevelUnlockedKey = "MaxLevelUnlockedKey";
    const string PlayerMoneyKey = "Money";
    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
        if (!SaveExist())
            Init();
        else
            LoadPrefs();


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayerPrefs();
            print("Reset player pref");
        }

        if (Input.GetKeyDown(KeyCode.S))
            SavePrefs();
    }
    public void Init()
    {
        levels = new List<Level>();
        for (int i = 0; i < levelsCount; i++)
        {
            levels.Add(new Level());
        }
        levels[0].unlocked = 1;
        // Player money.
        PlayerPrefs.SetFloat(PlayerMoneyKey, 0);

    }


    void OnApplicationQuit()
    {
        SavePrefs();

    }

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        maxLevelUnlocked = 1;
        Init();
    }
    public int GetLevelScore(int index)
    {
        return levels[index].score;
    }
    public void SaveLevelScore(int index, int score)
    {
        levels[index].score = score;
    }

    public void UnlockLevel(int index)
    {
        levels[index].unlocked = 1;
        if (index >= maxLevelUnlocked)
            maxLevelUnlocked = index;
    }

    public void SetTutorialDone()
    {
        PlayerPrefs.SetString("Tutorial", "Done");
    }


    public bool IsTutorialDone()
    {
        return PlayerPrefs.HasKey("Tutorial") && PlayerPrefs.GetString("Tutorial") == "Done";
    }


    public bool SaveExist()
    {
        return PlayerPrefs.HasKey(MaxLevelUnlockedKey) && PlayerPrefs.HasKey("Level1Unlocked");
    }
    public void SavePrefs()
    {
        //const string MaxLevelUnlockedKey = "MaxLevelUnlocked";

        PlayerPrefs.SetInt(MaxLevelUnlockedKey, maxLevelUnlocked);
        string key;
        for (int i = 0; i < levels.Count - 1; i++)
        {
            key = "Level" + i.ToString();
            // Is level unlocked.
            PlayerPrefs.SetInt(key + "Unlocked", levels[i].unlocked);

            // Level score.
            PlayerPrefs.SetInt(key + "Score", levels[i].score);

        }
        // Player money.
        PlayerPrefs.SetFloat(PlayerMoneyKey, PlayerStatsScript.instance.money);
    }

    public void LoadPrefs()
    {
        int i = 0;
        string key = "Level" + i.ToString();

        levels = new List<Level>();
        maxLevelUnlocked = PlayerPrefs.GetInt(MaxLevelUnlockedKey, 1);
        PlayerStatsScript.instance.money = PlayerPrefs.GetFloat(PlayerMoneyKey, 0);

        while (PlayerPrefs.HasKey(key + "Unlocked"))
        {
            key = "Level" + i.ToString();
            Level newLevel = new Level();
            // Is level unlocked.
            newLevel.unlocked = PlayerPrefs.GetInt(key + "Unlocked");

            // Level score.
            newLevel.score = PlayerPrefs.GetInt(key + "Score");

            levels.Add(newLevel);
            i++;
        }

    }

    public int GetCurrentLevelNumber()
    {
        // - 1 since list start a 0.
        return int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1]) - 1;
    }

    Level GetCurrentLevel()
    {
        return levels[GetCurrentLevelNumber()];
    }

    public bool HasCurrentLevelBeenPlayed()
    {
        return GetCurrentLevel().HasEverBeenPlayed();
    }

    [System.Serializable]
    public class Level
    {
        // 0 for locked, 1 for unlocked.
        public int unlocked;
        public int score;

        public bool HasEverBeenPlayed()
        {
            if (score > 0)
                return true;
            return false;
        }
    }
}

//What I need to save :

//-Which level has been started (to know if tutorial or new turret gif needs to be shown)
//-Unlocked level(from 1 to x level)
//- Score per level

