using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public int maxLevelUnlocked = 1;
    public List<Level> levels;
    public List<Unit> units;
    public List<HeroUpgrade> heroUpgrades;
    public List<string> unlockedHeroUpgrades;
    public List<string> unlockedUnits;
    int isTutorialDone;
    public const int levelsCount = 50;
    const string MAX_LEVEL_UNLOCKED_KEY = "MaxLevelUnlockedKey";
    const string PLAYER_MONEY_KEY = "Money";
    const string UNLOCKED_UNITS_KEY = "UnlockedUnits";
    const string UNLOCKED_HERO_UPGRADES_KEY = "UnlockedHeroUpgrades";
    const string IS_TUTORIAL_DONE_KEY = "IsTutorialDone";
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
        InitsHeroUpgrades();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    ResetPlayerPrefs();
        //    print("Reset player pref");
        //}
        //if (Input.GetKeyDown(KeyCode.V))
        //    print(PlayerStatsScript.instance.money);
        //if (Input.GetKeyDown(KeyCode.S))
        //    SavePrefs();
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
        PlayerPrefs.SetFloat(PLAYER_MONEY_KEY, 0);
        InitFirstTimeUnlockedUnits();
        InitFirstTimeUnlockedHeroUpgrades();
        SaveUnlockedUnits();
        SaveUnlockedHeroUpgrades();
        PlayerPrefs.SetInt(IS_TUTORIAL_DONE_KEY, 0);
    }

    void OnApplicationQuit()
    {
        SavePrefs();

    }

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerStatsScript.instance.money = 0f;
        isTutorialDone = 0;
        maxLevelUnlocked = 1;
        Init();
        InitFirstTimeUnlockedUnits();
        InitFirstTimeUnlockedHeroUpgrades();
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
        if (index >= levels.Count)
            return;
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
        isTutorialDone = 1;
    }

    public bool IsTutorialDone()
    {
        return isTutorialDone == 1;
    }


    public bool SaveExist()
    {
        return PlayerPrefs.HasKey(MAX_LEVEL_UNLOCKED_KEY) && PlayerPrefs.HasKey("Level1Unlocked");
    }
    public void SavePrefs()
    {
        //const string MaxLevelUnlockedKey = "MaxLevelUnlocked";

        PlayerPrefs.SetInt(MAX_LEVEL_UNLOCKED_KEY, maxLevelUnlocked);
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
        PlayerPrefs.SetFloat(PLAYER_MONEY_KEY, PlayerStatsScript.instance.money);

        PlayerPrefs.SetInt(IS_TUTORIAL_DONE_KEY, isTutorialDone);
        SaveUnlockedUnits();
    }


    public void SaveUnlockedUnits()
    {
        PlayerPrefs.SetString(UNLOCKED_UNITS_KEY, String.Join("|", unlockedUnits));
    }

    public string GetUnlockedUnitsFromPrefs()
    {
        string units = PlayerPrefs.GetString(UNLOCKED_UNITS_KEY);
        return string.Join("|", units);
    }

    public void SaveUnlockedHeroUpgrades()
    {
        PlayerPrefs.SetString(UNLOCKED_HERO_UPGRADES_KEY, String.Join("|", unlockedHeroUpgrades));
    }

    public string GetUnlockedHeroUpgradesFromPrefs()
    {
        string units = PlayerPrefs.GetString(UNLOCKED_HERO_UPGRADES_KEY);
        return string.Join("|", units);
    }
    string[] GetUnlockedHeroUpgradesArray()
    {
        return GetUnlockedHeroUpgradesFromPrefs().Split('|');
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
        maxLevelUnlocked = PlayerPrefs.GetInt(MAX_LEVEL_UNLOCKED_KEY, 1);
        PlayerStatsScript.instance.money = PlayerPrefs.GetFloat(PLAYER_MONEY_KEY, 0);

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
        isTutorialDone = PlayerPrefs.GetInt(IS_TUTORIAL_DONE_KEY);
        LoadUnlockedUnits();
        LoadUnlockedHeroUpgrades();
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

    void LoadUnlockedHeroUpgrades()
    {
        string[] units = GetUnlockedHeroUpgradesArray();
        foreach (string name in units)
        {
            if (name != "")
                unlockedHeroUpgrades.Add(name);
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

    public HeroUpgrade GetHeroUpgrade(string heroUpgradeName)
    {
        foreach (HeroUpgrade heroUpgrade in heroUpgrades)
        {
            if (heroUpgrade.name == heroUpgradeName)
                return heroUpgrade;
        }
        return null;
    }

    // When adding new upgrades, don't forget to reset player prefs to init new first level upgrades.
    void InitsHeroUpgrades()
    {
        heroUpgrades = new List<HeroUpgrade>
        {
            new HeroUpgrade("ManaMax100", "Increase maximum mana to 110",0),
            new HeroUpgrade("ManaMax110", "Increase maximum mana to 110",100),
            new HeroUpgrade("ManaMax120", "Increase maximum mana to 120",120),
            new HeroUpgrade("ManaMax130", "Increase maximum mana to 130",140),
            new HeroUpgrade("ManaMax140", "Increase maximum mana to 140",160),
            new HeroUpgrade("ManaMax150", "Increase maximum mana to 150",180),
            new HeroUpgrade("ManaMax160", "Increase maximum mana to 160",200),
            new HeroUpgrade("ManaMax170", "Increase maximum mana to 170",220),
            new HeroUpgrade("ManaMax180", "Increase maximum mana to 180",240),
            new HeroUpgrade("ManaMax190", "Increase maximum mana to 190",260),
            new HeroUpgrade("ManaMax200", "Increase maximum mana to 200",280),
            new HeroUpgrade("ManaMax210", "Increase maximum mana to 210",300),
            new HeroUpgrade("ManaMax220", "Increase maximum mana to 220",320),
            new HeroUpgrade("ManaMax230", "Increase maximum mana to 230",340),
            new HeroUpgrade("ManaMax240", "Increase maximum mana to 240",360),
            new HeroUpgrade("ManaMax250", "Increase maximum mana to 250",380),
            new HeroUpgrade("StartMana0", "Start stage with 0 mana.",0),
            new HeroUpgrade("StartMana25", "Start stage with 25% of maximum mana.",250),
            new HeroUpgrade("StartMana50", "Start stage with 50% of maximum mana.",500),
            new HeroUpgrade("StartMana75", "Start stage with 75% of maximum mana.",750),
            new HeroUpgrade("StartMana100", "Start stage with 100% of maximum mana.",1000),
            new HeroUpgrade("ManaRegen0.0", "Increase mana regeneration of 0.0 per second",0),
            new HeroUpgrade("ManaRegen0.5", "Increase mana regeneration of 0.5 per second",100),
            new HeroUpgrade("ManaRegen1.0", "Increase mana regeneration of 1.0 per second",150),
            new HeroUpgrade("ManaRegen1.5", "Increase mana regeneration of 1.5 per second",200),
            new HeroUpgrade("ManaRegen2.0", "Increase mana regeneration of 2.0 per second",250),
            new HeroUpgrade("ManaRegen2.5", "Increase mana regeneration of 2.5 per second",300),
            new HeroUpgrade("ManaRegen3.0", "Increase mana regeneration of 3.0 per second",350),
            new HeroUpgrade("ManaRegen3.5", "Increase mana regeneration of 3.5 per second",400),
            new HeroUpgrade("ManaRegen4.0", "Increase mana regeneration of 4.0 per second",450),
            new HeroUpgrade("ManaRegen4.5", "Increase mana regeneration of 4.5 per second",500),
            new HeroUpgrade("ManaRegen5.0", "Increase mana regeneration of 5.0 per second",550),
            new HeroUpgrade("UnitCooldownReduction0", "Reduce unit spawn cooldown by 0%",0),
            new HeroUpgrade("UnitCooldownReduction10", "Reduce unit spawn cooldown by 10%",200),
            new HeroUpgrade("UnitCooldownReduction20", "Reduce unit spawn cooldown by 20%",400),
            new HeroUpgrade("UnitCooldownReduction30", "Reduce unit spawn cooldown by 30%",600),
            new HeroUpgrade("UnitCooldownReduction40", "Reduce unit spawn cooldown by 40%",800),
            new HeroUpgrade("UnitCooldownReduction50", "Reduce unit spawn cooldown by 50%",1000),
        };
    }

    void InitUnits()
    {
        units = new List<Unit>
        {
            new Unit("Chicken1", 15, 6, 100, "Fast dps unit."),
            new Unit("Chicken2", 30, 8, 200, "Every 3 attacks, double hit"),
            new Unit("Chicken3", 60, 10, 500, "Every 3 attacks, triple hit and 50% chance to dodge an attack"),
            new Unit("Chicken4", 100, 12, 1200, "When under 25% health, attack speed is increase by 500%, each hit increase attack damage by 5% until 50%)"),

            new Unit("Duck1", 20, 8, 100, "Tank unit, high health."),
            new Unit("Duck2", 35, 10, 200, "Every 3 hit received, block next hit"),
            new Unit("Duck3", 55, 12, 400, "Start with a big shield that blocks the 5 next hit, when big shield is down, receive defense bonus for x seconds, big shield reappears after x seconds."),
            new Unit("Duck4", 85, 14, 1100, "Big shield now blocks 10 next hit instead of 5, when big shield is down, receive defense bonus for x seconds, big shield reappears after x seconds. When any shield is up, restore x% max health over time."),

            new Unit("Trunk1", 25, 10, 150, "Shoot bullet."),
            new Unit("Trunk2", 50, 12, 250, "Shoot 3 times in row (on every attack)."),
            new Unit("Trunk3", 80, 14, 600, "Increased shooting speed."),
            new Unit("Trunk4", 115, 16, 1500, "Gatling shooting."),

            new Unit("Rock1", 25, 12, 150, "Tank unit, high health."),
            new Unit("Rock2", 40, 12, 350, "Become super resistant (damage received reduced by a percentage) for an amount of time."),
            new Unit("Rock3", 75, 12, 800, "When super resistant, sends back a percentage of damage received and projectiles are reflected."),
            new Unit("Rock4", 95, 12, 1700, "Become invulnerable for an amount of time."),

            new Unit("Bunny1", 20, 10, 100, "Summon weak rabbit."),
            new Unit("Bunny2", 35, 10, 300, "Instead of weak rabbit, randomly summon unique rabbit (dps or tank)."),
            new Unit("Bunny3", 60, 10, 550, "Instead of unique rabbit (dps or tank), summon an army of weak rabbits."),
            new Unit("Bunny4", 105, 10, 1300, "Instead of an army of weak rabbits, summon an army of unique rabbits (dps or tank)."),

            new Unit("Plant1", 30, 15, 175, "Shoot bullet at long distance."),
            new Unit("Plant2", 55, 17, 400, "Bullets go through 3 enemies."),
            new Unit("Plant3", 90, 19, 750, "Bullet pierce through all enemies on a distance."),
            new Unit("Plant4", 140, 20, 1600, "Shoot an explosive bullet."),


            new Unit("Mushroom1", 50, 10, 700, "Aoe damage poison."),
            new Unit("Mushroom2", 75, 12, 1400, "Reduce enemies inflicted damage."),
            new Unit("Mushroom3", 110, 14, 2000, "Max health percentage damage, reduced inflicted damage and defense."),
            new Unit("Mushroom4", 200, 16, 3500, "After being poisoned for a certain amount of time, enemy are paralysed for a short amount of time."),

            new Unit("BlueBird1", 20, 10, 125, "Dps unit."),
            new Unit("BlueBird2", 45, 12, 450, "Summon a bird that will drop an explosive egg on enemies."),
            new Unit("BlueBird3", 95, 14, 900, "Summon a bird that will drop multiple explosive eggs on enemies."),
            new Unit("BlueBird4", 150, 16, 1800, "Summon a bird that will drop multiple eggs that spawn small birds on enemies."),

            new Unit("Snail1", 15, 10, 75, "Give close allies additional health."),
            new Unit("Snail2", 30, 12, 300, "Every certain amount of time, heal close allies."),
            new Unit("Snail3", 50, 14, 600, "Constantly heal close allies."),
            new Unit("Snail4", 75, 16, 1000f, "When an close ally is low life, start a strong global healing.")
        };

    }

    [System.Serializable]
    public class HeroUpgrade
    {
        public string name;
        public string description;
        public float shopPrice;
        public HeroUpgrade(string name, string description, float shopPrice)
        {
            this.name = name;
            this.shopPrice = shopPrice;
            this.description = description;
        }
    }

    void InitFirstTimeUnlockedUnits()
    {
        unlockedUnits = new List<string>
        {
            "Chicken1",
            "Duck1",
        };
    }

    void InitFirstTimeUnlockedHeroUpgrades()
    {
        unlockedHeroUpgrades = new List<string>
        {
            "ManaMax100",
            "ManaRegen0.0",
            "UnitCooldownReduction0",
            "StartMana0",

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

