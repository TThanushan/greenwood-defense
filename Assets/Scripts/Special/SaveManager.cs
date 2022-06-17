using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Level newLevel = new Level();
            // Is level unlocked.
            newLevel.unlocked = PlayerPrefs.GetInt(key + "Unlocked");

            // Level score.
            newLevel.score = PlayerPrefs.GetInt(key + "Score");

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
            new Unit("Chicken1", 25, 6, 100f),
            new Unit("Chicken2", 35, 8, 200f, "Every 3 attacks, double hit"),
            new Unit("Chicken3", 45, 6, 400, "Every 3 attacks, triple hit and 50% chance to dodge an attack"),
            new Unit("Chicken4", 55, 6, 800f, "When under 25% health, attack speed is increase by 500%, each hit increase attack damage by 5% until 50%)"),

            new Unit("Duck1", 30, 8, 100f),
            new Unit("Duck2", 40, 10, 200f, "Every 3 hit received, block next hit"),
            new Unit("Duck3", 50, 12, 400f, "Start with a big shield that blocks the 5 next hit, when big shield is down, receive defense bonus for x seconds, big shield reappears after x seconds."),
            new Unit("Duck4", 60, 14, 800f, "Big shield now blocks 10 next hit instead of 5, when big shield is down, receive defense bonus for x seconds, big shield reappears after x seconds. When any shield is up, restore x% max health over time."),

            new Unit("Trunk1", 30, 10, 100f),
            new Unit("Trunk2", 45, 12, 200f, "Shoot 3 times in row (on every attack)."),
            new Unit("Trunk3", 60, 14, 400f, "Increased shooting speed."),
            new Unit("Trunk4", 75, 16, 800f, "Gatling shooting."),

            new Unit("Rock1", 35, 12, 150),
            new Unit("Rock2", 35, 12, 300, "Become super resistant (damage received reduced by a percentage) for an amount of time."),
            new Unit("Rock3", 35, 12, 450, "When super resistant, sends back a percentage of damage received and projectiles are reflected."),
            new Unit("Rock4", 35, 12, 1000f, "Become invulnerable for an amount of time."),

            new Unit("Bunny1", 35, 10, 150, "Summon weak rabbit."),
            new Unit("Bunny2", 35, 10, 300, "Instead of weak rabbit, randomly summon unique rabbit (dps or tank)."),
            new Unit("Bunny3", 35, 10, 450, "Instead of unique rabbit (dps or tank), summon an army of weak rabbits."),
            new Unit("Bunny4", 35, 10, 1000f, "Instead of an army of weak rabbits, summon an army of unique rabbits (dps or tank)."),


            new Unit("Plant1", 35, 12, 150),
            new Unit("Plant2", 50, 14, 300, "Bullets go through 3 enemies."),
            new Unit("Plant3", 70, 16, 500, "Bullet pierce through all enemies on a distance."),
            new Unit("Plant4", 90, 18, 1000f, "Shoot an explosive bullet."),


            new Unit("Mushroom1", 35, 10, 700, "Aoe damage poison."),
            new Unit("Mushroom2", 45, 12, 900, "Reduce enemies inflicted damage."),
            new Unit("Mushroom3", 60, 14, 1200, "Max health percentage damage, reduced inflicted damage and defense."),
            new Unit("Mushroom4", 75, 16, 1000f, "After being poisoned for a certain amount of time, enemy are paralysed for a short amount of time."),

            new Unit("BlueBird1", 35, 10, 700),
            new Unit("BlueBird2", 45, 12, 900, "Summon a bird that will drop an explosive egg on enemies."),
            new Unit("BlueBird3", 60, 14, 1200, "Summon a bird that will drop multiple explosive eggs on enemies."),
            new Unit("BlueBird4", 75, 16, 1000f, "Summon a bird that will drop multiple eggs that spawn small birds on enemies."),

            new Unit("Snail1", 35, 10, 700, "Give close allies additional health."),
            new Unit("Snail2", 45, 12, 900, "Every certain amount of time, heal close allies."),
            new Unit("Snail3", 60, 14, 1200, "Constantly heal close allies."),
            new Unit("Snail4", 75, 16, 1000f, "When an close ally is low life, start a strong global healing.")
        };

    }

    void InitFirstTimeUnlockedUnits()
    {
        print("InitFirstTimeUnlockedUnits");
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

