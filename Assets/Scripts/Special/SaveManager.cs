﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public int maxLevelUnlocked = 1;
    public List<Level> levels;
    public List<Unit> units;
    public List<string> unlockedUnits;

    public const int levelsCount = 50;

    const string MaxLevelUnlockedKey = "MaxLevelUnlockedKey";
    const string PlayerMoneyKey = "Money";
    const string UnlockedUnitsKey = "UnlockedUnits";
    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
        if (!SaveExist())
        {
            Init();
        }
        else
            LoadPrefs();

        InitUnits();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPlayerPrefs();
            print("Reset player pref");
        }
        if (Input.GetKeyDown(KeyCode.V))
            print(PlayerStatsScript.instance.money);
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
        InitFirstTimeUnlockedUnits();
        SaveUnlockedUnits();
    }



    void OnApplicationQuit()
    {
        SavePrefs();

    }

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerStatsScript.instance.money = 0f;

        maxLevelUnlocked = 1;
        Init();
        InitFirstTimeUnlockedUnits();
    }
    public int GetLevelScore(int index)
    {
        return levels[index].score;
    }

    public int GetLevelScore()
    {
        print(GetCurrentLevelNumber());
        return levels[GetCurrentLevelNumber()].score;
    }

    public void SaveLevelScore(int index, int score)
    {
        levels[index].score = score;
    }

    public void UnlockLevel(int index)
    {
        levels[index].unlocked = 1;
        if (index >= maxLevelUnlocked)
            maxLevelUnlocked = index + 1;
    }

    public string GetLatestStageUnlockedName()
    {
        return "Stage " + (maxLevelUnlocked + 1).ToString();
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


    public void SaveUnlockedUnits()
    {
        PlayerPrefs.SetString(UnlockedUnitsKey, String.Join("|", unlockedUnits));
    }

    public string GetUnlockedUnitsFromPrefs()
    {
        string units = PlayerPrefs.GetString(UnlockedUnitsKey);
        return string.Join("|", units);
    }

    string[] GetUnlockedUnitsArray()
    {
        return GetUnlockedUnitsFromPrefs().Split('|');
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
            Level newLevel = new Level
            {
                unlocked = PlayerPrefs.GetInt(key + "Unlocked"),
                score = PlayerPrefs.GetInt(key + "Score")
            };

            levels.Add(newLevel);
            i++;
        }
        LoadUnlockedUnits();
    }

    void LoadUnlockedUnits()
    {
        string[] units = GetUnlockedUnitsArray();
        foreach (string name in units)
        {
            if (name != "")
                unlockedUnits.Add(name);
        }
    }

    public int GetCurrentLevelNumber()
    {
        return StageInfosManager.instance.GetCurrentStageNumber() - 1;
        //// - 1 since list start a 0.
        //return int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1]) - 1;
    }

    Level GetCurrentLevel()
    {
        return levels[GetCurrentLevelNumber()];
    }

    public bool HasCurrentLevelBeenPlayed()
    {
        return GetCurrentLevel().HasEverBeenPlayed();
    }

    public Unit GetUnit(string unitName)
    {
        foreach (Unit unit in units)
        {
            if (unit.name == unitName)
                return unit;
        }

        return null;
    }

    void InitUnits()
    {
        units = new List<Unit>
        {
            new Unit("Chicken1", 25, 6, 100),
            new Unit("Chicken2", 35, 8, 200, "Every 3 attacks, double hit"),
            new Unit("Chicken3", 50, 10, 500, "Every 3 attacks, triple hit and 50% chance to dodge an attack"),
            new Unit("Chicken4", 70, 12, 1200, "When under 25% health, attack speed is increase by 500%, each hit increase attack damage by 5% until 50%)"),

            new Unit("Duck1", 25, 8, 100),
            new Unit("Duck2", 35, 10, 200, "Every 3 hit received, block next hit"),
            new Unit("Duck3", 45, 12, 400, "Start with a big shield that blocks the 5 next hit, when big shield is down, receive defense bonus for x seconds, big shield reappears after x seconds."),
            new Unit("Duck4", 60, 14, 1100, "Big shield now blocks 10 next hit instead of 5, when big shield is down, receive defense bonus for x seconds, big shield reappears after x seconds. When any shield is up, restore x% max health over time."),

            new Unit("Trunk1", 30, 10, 100),
            new Unit("Trunk2", 45, 12, 250, "Shoot 3 times in row (on every attack)."),
            new Unit("Trunk3", 65, 14, 600, "Increased shooting speed."),
            new Unit("Trunk4", 85, 16, 1500, "Gatling shooting."),

            new Unit("Rock1", 35, 12, 150),
            new Unit("Rock2", 55, 12, 350, "Become super resistant (damage received reduced by a percentage) for an amount of time."),
            new Unit("Rock3", 75, 12, 800, "When super resistant, sends back a percentage of damage received and projectiles are reflected."),
            new Unit("Rock4", 100, 12, 1700, "Become invulnerable for an amount of time."),

            new Unit("Bunny1", 20, 10, 125, "Summon weak rabbit."),
            new Unit("Bunny2", 30, 10, 300, "Instead of weak rabbit, randomly summon unique rabbit (dps or tank)."),
            new Unit("Bunny3", 45, 10, 550, "Instead of unique rabbit (dps or tank), summon an army of weak rabbits."),
            new Unit("Bunny4", 60, 10, 1300, "Instead of an army of weak rabbits, summon an army of unique rabbits (dps or tank)."),

            new Unit("Plant1", 35, 12, 175),
            new Unit("Plant2", 50, 14, 400, "Bullets go through 3 enemies."),
            new Unit("Plant3", 70, 16, 750, "Bullet pierce through all enemies on a distance."),
            new Unit("Plant4", 90, 18, 1600, "Shoot an explosive bullet."),


            new Unit("Mushroom1", 55, 10, 700, "Aoe damage poison."),
            new Unit("Mushroom2", 60, 12, 1400, "Reduce enemies inflicted damage."),
            new Unit("Mushroom3", 85, 14, 2000, "Max health percentage damage, reduced inflicted damage and defense."),
            new Unit("Mushroom4", 130, 16, 3500, "After being poisoned for a certain amount of time, enemy are paralysed for a short amount of time."),

            new Unit("BlueBird1", 25, 10, 125),
            new Unit("BlueBird2", 45, 12, 450, "Summon a bird that will drop an explosive egg on enemies."),
            new Unit("BlueBird3", 65, 14, 900, "Summon a bird that will drop multiple explosive eggs on enemies."),
            new Unit("BlueBird4", 90, 16, 1800, "Summon a bird that will drop multiple eggs that spawn small birds on enemies."),

            new Unit("Snail1", 20, 10, 75, "Give close allies additional health."),
            new Unit("Snail2", 35, 12, 300, "Every certain amount of time, heal close allies."),
            new Unit("Snail3", 50, 14, 600, "Constantly heal close allies."),
            new Unit("Snail4", 65, 16, 1000f, "When an close ally is low life, start a strong global healing.")
        };

    }

    void InitFirstTimeUnlockedUnits()
    {
        unlockedUnits = new List<string>
        {
            "Chicken1",
            "Duck1",
        };
    }

    [System.Serializable]
    public class Unit
    {
        public string name;
        public float cost;
        public float reloadTime;
        public float shopPrice;
        public string effectDescription;

        public Unit(string name, float cost, float reloadTime, float upgradePrice, string effectDescription = "No effect.")
        {
            this.name = name;
            this.cost = cost;
            this.reloadTime = reloadTime;
            this.shopPrice = upgradePrice;
            this.effectDescription = effectDescription;
        }

        public static implicit operator List<object>(Unit v)
        {
            throw new NotImplementedException();
        }
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

