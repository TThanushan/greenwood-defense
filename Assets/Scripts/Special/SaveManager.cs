using System;
using System.Collections.Generic;
using UnityEngine;
using static ShowNewEnemyDescriptionCard;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    public int maxLevelUnlocked = 1;

    public bool isAutoSave;
    public List<Level> levels;
    public List<Unit> units;
    public List<HeroUpgrade> heroUpgrades;
    public List<string> unlockedHeroUpgrades;
    public List<string> unlockedUnits;
    public List<UnitDescription> unitDescriptions;
    int isTutorialDone;
    const string MAX_LEVEL_UNLOCKED_KEY = "MaxLevelUnlockedKey";
    const string PLAYER_MONEY_KEY = "Money";
    const string UNLOCKED_UNITS_KEY = "UnlockedUnits";
    const string UNLOCKED_HERO_UPGRADES_KEY = "UnlockedHeroUpgrades";
    const string IS_TUTORIAL_DONE_KEY = "IsTutorialDone";
    const string AUTO_SAVE_KEY = "IsAutoSave";
    const string NEW_ENEMY_CARD_DESCRIPTION_SHOWNED_INDEX_KEY = "newEnemyCardShownedIndex";

    public int newEnemyCardDescriptionShownedIndex;
    public float money;
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

        InitFrogDescription();
        InitUnits();
        InitsHeroUpgrades();
    }
    public bool SaveExist()
    {
        return PlayerPrefs.HasKey(MAX_LEVEL_UNLOCKED_KEY) && PlayerPrefs.HasKey("Level1Unlocked");
    }

    public void Init()
    {
        levels = new List<Level>();
        for (int i = 0; i <= Constants.MAX_STAGE_NUMBER; i++)
        {
            levels.Add(new Level());
        }
        newEnemyCardDescriptionShownedIndex = 0;
        levels[0].unlocked = 1;
        // Player money.
        maxLevelUnlocked = 1;
        PlayerPrefs.SetFloat(PLAYER_MONEY_KEY, 0);
        InitFirstTimeUnlockedUnits();
        InitFirstTimeUnlockedHeroUpgrades();
        SaveUnlockedUnits();
        SaveUnlockedHeroUpgrades();
        isTutorialDone = 0;
        isAutoSave = true;
        SaveLevels();
        SaveIsAutoSave();
        money = 0f;
        PlayerPrefs.SetInt(IS_TUTORIAL_DONE_KEY, 0);
        PlayerPrefs.SetInt(NEW_ENEMY_CARD_DESCRIPTION_SHOWNED_INDEX_KEY, 0);
    }

    void OnApplicationQuit()
    {
        SavePrefIfAutoSave();
    }

    public void SavePrefIfAutoSave()
    {
        if (isAutoSave)
            SavePrefs();
    }

    void InitFrogDescription()
    {
        unitDescriptions = new List<UnitDescription>
        {
            new UnitDescription("SwordFrog", "Basic frog, spawn frequently."),
            new UnitDescription("ShooterFrog", "Range frog, low health."),
            new UnitDescription("TankFrog", "High health but low damage."),
            new UnitDescription("SpearFrog", "Damage all enemies in front of him."),
            new UnitDescription("FatFrog", "Really high health, explode on death and spawn several small frogs."),
            new UnitDescription("TrapFrog", "Drop traps when walking, damage units that walk on them."),
            new UnitDescription("PoisonFrog", "Poison his target."),
            new UnitDescription("HealerFrog", "Heal his allies."),
            new UnitDescription("AxeFrog", "Low attack speed, high damage on all enemies in front of him."),
            new UnitDescription("DoubleSwordFrog", "High move speed and attack speed."),
            new UnitDescription("ShotgunFrog", "High range damage."),
            new UnitDescription("TeleportFrog", "Every certain time, teleport behind his enemy."),
            new UnitDescription("RocketLauncherFrog", "Fire a rocket that does splash damage."),
            new UnitDescription("NinjaFrog", "\"KAGE BUNSHIN NO JUTSU!\""),
            new UnitDescription("SpeedFrog", "Move and atack super fast."),
            new UnitDescription("MageFrog", "Shoot fire ball with splash damage."),
            new UnitDescription("KamikazeFrog", "Explode when being hit."),
            new UnitDescription("ZombieFrog", "Instead of dying, come back to life with increased damage and movement speed."),
            new UnitDescription("ScytheFrog", "Each of his attacks send a shock wave in front of him."),
            new UnitDescription("SummonerFrog", "Summon other frogs."),
        };
    }

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Init();

        //Old
        //PlayerPrefs.DeleteAll();
        //SaveManager.instance.money = 0f;
        //isTutorialDone = 0;
        //maxLevelUnlocked = 1;
        //Init();
        //InitFirstTimeUnlockedUnits();
        //InitFirstTimeUnlockedHeroUpgrades();
    }
    public int GetLevelScore(int index)
    {
        return levels[index].score;
    }

    public int GetLevelScore()
    {
        return levels[GetCurrentLevelNumber()].score;
    }

    public void SaveLevelScoreIfAutoSave(int index, int score)
    {
        if (isAutoSave)
            SaveLevelScore(index, score);
    }
    public void SaveLevelScore(int index, int score)
    {
        levels[index].score = score;
    }

    public void UnlockLevel(int index)
    {
        if (index >= levels.Count)
        {
            levels[levels.Count - 1].unlocked = 1;
            return;
        }
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


    void SaveLevels()
    {
        string key;
        for (int i = 0; i < levels.Count; i++)
        {
            key = "Level" + i.ToString();
            if (i > maxLevelUnlocked)
            {
                PlayerPrefs.SetInt(key + "Unlocked", 0);
                PlayerPrefs.SetInt(key + "Score", 0);
                continue;
            }

            // Is level unlocked.
            PlayerPrefs.SetInt(key + "Unlocked", levels[i].unlocked);
            // Level score.
            PlayerPrefs.SetInt(key + "Score", levels[i].score);
        }
    }
    public void SavePrefs()
    {

        SaveLevels();
        SaveIsAutoSave();
        SaveUnlockedUnits();
        SaveUnlockedHeroUpgrades();


        PlayerPrefs.SetInt(MAX_LEVEL_UNLOCKED_KEY, maxLevelUnlocked);
        PlayerPrefs.SetFloat(PLAYER_MONEY_KEY, money);
        PlayerPrefs.SetInt(IS_TUTORIAL_DONE_KEY, isTutorialDone);
        PlayerPrefs.SetInt(NEW_ENEMY_CARD_DESCRIPTION_SHOWNED_INDEX_KEY, newEnemyCardDescriptionShownedIndex);

        PlayerPrefs.Save();
    }

    public void SaveIsAutoSave()
    {
        int autoSaveInt = (isAutoSave == true) ? 1 : 0;
        PlayerPrefs.SetInt(AUTO_SAVE_KEY, autoSaveInt);
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
        levels = new List<Level>();
        maxLevelUnlocked = PlayerPrefs.GetInt(MAX_LEVEL_UNLOCKED_KEY, 1);
        money = PlayerPrefs.GetFloat(PLAYER_MONEY_KEY, 0);

        for (int i = 0; i < Constants.MAX_STAGE_NUMBER; i++)
        {
            string key = "Level" + i.ToString();
            Level newLevel = new()
            {
                unlocked = PlayerPrefs.GetInt(key + "Unlocked"),
                score = PlayerPrefs.GetInt(key + "Score")
            };
            levels.Add(newLevel);
        }
        isTutorialDone = PlayerPrefs.GetInt(IS_TUTORIAL_DONE_KEY);
        isAutoSave = PlayerPrefs.GetInt(AUTO_SAVE_KEY) == 1;
        newEnemyCardDescriptionShownedIndex = PlayerPrefs.GetInt(NEW_ENEMY_CARD_DESCRIPTION_SHOWNED_INDEX_KEY);

        LoadUnlockedUnits();
        LoadUnlockedHeroUpgrades();
    }

    void LoadUnlockedUnits()
    {
        string[] units = GetUnlockedUnitsArray();
        unlockedUnits.Clear();
        foreach (string name in units)
        {
            if (name != "")
                unlockedUnits.Add(name);
        }
    }

    void LoadUnlockedHeroUpgrades()
    {
        string[] units = GetUnlockedHeroUpgradesArray();
        unlockedHeroUpgrades.Clear();
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
            new HeroUpgrade("ManaMax100"),
            new HeroUpgrade("ManaMax110", "Increase maximum mana from 110 to 110",100),
            new HeroUpgrade("ManaMax120", "Increase maximum mana from 110 to 120",120),
            new HeroUpgrade("ManaMax130", "Increase maximum mana from 120 to 130",140),
            new HeroUpgrade("ManaMax140", "Increase maximum mana from 130 to 140",160),
            new HeroUpgrade("ManaMax150", "Increase maximum mana from 140 to 150",180),
            new HeroUpgrade("ManaMax160", "Increase maximum mana from 150 to 160",200),
            new HeroUpgrade("ManaMax170", "Increase maximum mana from 160 to 170",220),
            new HeroUpgrade("ManaMax180", "Increase maximum mana from 170 to 180",240),
            new HeroUpgrade("ManaMax190", "Increase maximum mana from 180 to 190",260),
            new HeroUpgrade("ManaMax200", "Increase maximum mana from 190 to 200",280),
            new HeroUpgrade("ManaMax210", "Increase maximum mana from 200 to 210",300),
            new HeroUpgrade("ManaMax220", "Increase maximum mana from 210 to 220",320),
            new HeroUpgrade("ManaMax230", "Increase maximum mana from 220 to 230",340),
            new HeroUpgrade("ManaMax240", "Increase maximum mana from 230 to 240",360),
            new HeroUpgrade("ManaMax250", "Increase maximum mana from 240 to 250",380),
            new HeroUpgrade("ManaMax260", "Increase maximum mana from 250 to 260",400),
            new HeroUpgrade("ManaMax270", "Increase maximum mana from 260 to 270",420),
            new HeroUpgrade("ManaMax280", "Increase maximum mana from 270 to 280",440),
            new HeroUpgrade("ManaMax290", "Increase maximum mana from 280 to 290",460),
            new HeroUpgrade("ManaMax300", "Increase maximum mana from 290 to 300",480),
            new HeroUpgrade("ManaMax310", "Increase maximum mana from 300 to 310",500),
            new HeroUpgrade("ManaMax320", "Increase maximum mana from 310 to 320",520),
            new HeroUpgrade("ManaMax330", "Increase maximum mana from 320 to 330",540),
            new HeroUpgrade("ManaMax340", "Increase maximum mana from 330 to 340",560),
            new HeroUpgrade("ManaMax350", "Increase maximum mana from 340 to 350",580),

            new HeroUpgrade("StartMana0"),
            new HeroUpgrade("StartMana25", "Start stage with 25% of maximum mana.",500),
            new HeroUpgrade("StartMana50", "Start stage with 50% of maximum mana.",1000),
            new HeroUpgrade("StartMana75", "Start stage with 75% of maximum mana.",1500),
            new HeroUpgrade("StartMana100", "Start stage with 100% of maximum mana.",2000),


            new HeroUpgrade("ManaRegen0.0"),
            new HeroUpgrade("ManaRegen0.5", "Increase mana regeneration by 0.5 per second)",100),
            new HeroUpgrade("ManaRegen1.0", "Increase mana regeneration (0.5 => 1.0 per second)",150),
            new HeroUpgrade("ManaRegen1.5", "Increase mana regeneration (1.0 => 1.5 per second)",200),
            new HeroUpgrade("ManaRegen2.0", "Increase mana regeneration (1.5 => 2.0 per second)",250),
            new HeroUpgrade("ManaRegen2.5", "Increase mana regeneration (2.0 => 2.5 per second)",300),
            new HeroUpgrade("ManaRegen3.0", "Increase mana regeneration (2.5 => 3.0 per second)",350),
            new HeroUpgrade("ManaRegen3.5", "Increase mana regeneration (3.0 => 3.5 per second)",400),
            new HeroUpgrade("ManaRegen4.0", "Increase mana regeneration (3.5 => 4.0 per second)",450),
            new HeroUpgrade("ManaRegen4.5", "Increase mana regeneration (4.0 => 4.5 per second)",500),
            new HeroUpgrade("ManaRegen5.0", "Increase mana regeneration (4.5 => 5.0 per second)",550),
            new HeroUpgrade("ManaRegen5.5", "Increase mana regeneration (5.0 => 5.5 per second)",600),
            new HeroUpgrade("ManaRegen6.0", "Increase mana regeneration (5.5 => 6.0 per second)",650),
            new HeroUpgrade("ManaRegen6.5", "Increase mana regeneration (6.0 => 6.5 per second)",700),
            new HeroUpgrade("ManaRegen7.0", "Increase mana regeneration (6.5 => 7.0 per second)",750),
            new HeroUpgrade("ManaRegen7.5", "Increase mana regeneration (7.0 => 7.5 per second)",800),
            new HeroUpgrade("ManaRegen8.0", "Increase mana regeneration (7.5 => 8.0 per second)",850),
            new HeroUpgrade("ManaRegen8.5", "Increase mana regeneration (8.0 => 8.5 per second)",900),
            new HeroUpgrade("ManaRegen9.0", "Increase mana regeneration (8.5 => 9.0 per second)",950),
            new HeroUpgrade("ManaRegen9.5", "Increase mana regeneration (9.0 => 9.5 per second)",1000),
            new HeroUpgrade("ManaRegen10.0", "Increase mana regeneration (9.5 => 10.0 per second)",1050),

            new HeroUpgrade("UnitMaxHealthBonus0"),
            new HeroUpgrade("UnitMaxHealthBonus15", "Increase units max health by 15%", 750),
            new HeroUpgrade("UnitMaxHealthBonus30", "Increase units max health (15% => 30%)", 1500),
            new HeroUpgrade("UnitMaxHealthBonus45", "Increase units max health (30% => 45%)", 2250),
            new HeroUpgrade("UnitMaxHealthBonus60", "Increase units max health (45% => 60%)", 3000),
            new HeroUpgrade("UnitMaxHealthBonus75", "Increase units max health (60% => 75%)", 3750),
            new HeroUpgrade("UnitMaxHealthBonus90", "Increase units max health (75% => 90%)", 4500),
            new HeroUpgrade("UnitMaxHealthBonus105", "Increase units max health (90% => 105%)", 5250),
            new HeroUpgrade("UnitMaxHealthBonus120", "Increase units max health (105% => 120%)", 6000),
            new HeroUpgrade("UnitMaxHealthBonus135", "Increase units max health (120% => 135%)", 6750),
            new HeroUpgrade("UnitMaxHealthBonus150", "Increase units max health (135% => 150%)", 7500),

            new HeroUpgrade("UnitDamageBonus0"),
            new HeroUpgrade("UnitDamageBonus7.5", "Increase units attack damage by 7.5%", 750),
            new HeroUpgrade("UnitDamageBonus15", "Increase units attack damage (7.5 => 15%)", 1500),
            new HeroUpgrade("UnitDamageBonus22.5", "Increase units attack damage (15 => 22.5%)", 2250),
            new HeroUpgrade("UnitDamageBonus30", "Increase units attack damage (22.5 => 30%)", 3000),
            new HeroUpgrade("UnitDamageBonus37.5", "Increase units attack damage (30 => 37.5%)", 3750),
            new HeroUpgrade("UnitDamageBonus45", "Increase units attack damage (37.5 => 45%)", 4500),
            new HeroUpgrade("UnitDamageBonus52.5", "Increase units attack damage (45 => 52.5%)", 5250),
            new HeroUpgrade("UnitDamageBonus60", "Increase units attack damage (52.5 => 60%)", 6000),
            new HeroUpgrade("UnitDamageBonus67.5", "Increase units attack damage (60 => 67.5%)", 6750),
            new HeroUpgrade("UnitDamageBonus75", "Increase units attack damage (67.5 => 75%)", 7500),

            new HeroUpgrade("UnitDamageReduction0"),
            new HeroUpgrade("UnitDamageReduction7.5", "Units damage received reduced by 7.5%", 750),
            new HeroUpgrade("UnitDamageReduction15", "Units damage received reduced (7.5 => 15%)", 1500),
            new HeroUpgrade("UnitDamageReduction22.5", "Units damage received reduced (15 => 22.5%)", 2250),
            new HeroUpgrade("UnitDamageReduction30", "Units damage received reduced (22.5 => 30%)", 3000),
            new HeroUpgrade("UnitDamageReduction37.5", "Units damage received reduced (30 => 37.5%)", 3750),
            new HeroUpgrade("UnitDamageReduction45", "Units damage received reduced (37.5 => 45%)", 4500),
            new HeroUpgrade("UnitDamageReduction52.5", "Units damage received reduced (45 => 52.5%)", 5250),
            new HeroUpgrade("UnitDamageReduction60", "Units damage received reduced (52.5 => 60%)", 6000),
            new HeroUpgrade("UnitDamageReduction67.5", "Units damage received reduced (60 => 67.5%)", 6750),
            new HeroUpgrade("UnitDamageReduction75", "Units damage received reduced (67.5 => 75%)", 7500),

            new HeroUpgrade("UnitCooldownReduction0"),
            new HeroUpgrade("UnitCooldownReduction7.5", "Reduce unit spawn cooldown by 5%",200),
            new HeroUpgrade("UnitCooldownReduction15", "Reduce unit spawn cooldown (7.5 => 15%)",400),
            new HeroUpgrade("UnitCooldownReduction22.5", "Reduce unit spawn cooldown (15 => 22.5%)",600),
            new HeroUpgrade("UnitCooldownReduction30", "Reduce unit spawn cooldown (22.5 => 30%)",800),
            new HeroUpgrade("UnitCooldownReduction37.5", "Reduce unit spawn cooldown (30 => 37.5%)",800),
            new HeroUpgrade("UnitCooldownReduction45", "Reduce unit spawn cooldown (37.5 => 45%)",1000),
            new HeroUpgrade("UnitCooldownReduction52.5", "Reduce unit spawn cooldown (45 => 52.5%)",1200),
            new HeroUpgrade("UnitCooldownReduction60", "Reduce unit spawn cooldown (52.5 => 60%)",1400),
            new HeroUpgrade("UnitCooldownReduction67.5", "Reduce unit spawn cooldown (60 => 67.5%)",1600),
            new HeroUpgrade("UnitCooldownReduction75", "Reduce unit spawn cooldown (67.5 => 75%)",1800),

            new HeroUpgrade("MaxHealth100"),
            new HeroUpgrade("MaxHealth150", "Increase your captain max health to 150", 400),
            new HeroUpgrade("MaxHealth200", "Increase your captain max health to 200", 800),
            new HeroUpgrade("MaxHealth250", "Increase your captain max health to 250", 1200),
            new HeroUpgrade("MaxHealth300", "Increase your captain max health to 300", 1600),
            new HeroUpgrade("MaxHealth350", "Increase your captain max health to 350", 2000),
            new HeroUpgrade("MaxHealth400", "Increase your captain max health to 400", 2400),
            new HeroUpgrade("MaxHealth450", "Increase your captain max health to 450", 2800),
            new HeroUpgrade("MaxHealth500", "Increase your captain max health to 500", 3200),


            new HeroUpgrade("DamageReduction0"),
            new HeroUpgrade("DamageReduction5", "Reduce the damage taken by your captain by 5%", 200),
            new HeroUpgrade("DamageReduction10", "Reduce the damage taken by your captain by 10%", 400),
            new HeroUpgrade("DamageReduction15", "Reduce the damage taken by your captain by 15%", 600),
            new HeroUpgrade("DamageReduction20", "Reduce the damage taken by your captain by 20%", 800),
            new HeroUpgrade("DamageReduction25", "Reduce the damage taken by your captain by 25%", 1000),
            new HeroUpgrade("DamageReduction30", "Reduce the damage taken by your captain by 30%", 1200),
            new HeroUpgrade("DamageReduction35", "Reduce the damage taken by your captain by 35%", 1400),
            new HeroUpgrade("DamageReduction40", "Reduce the damage taken by your captain by 40%", 1600),
            new HeroUpgrade("DamageReduction45", "Reduce the damage taken by your captain by 45%", 1800),
            new HeroUpgrade("DamageReduction50", "Reduce the damage taken by your captain by 50%", 2000),

            new HeroUpgrade("MoneyIncomeIncrease0"),
            new HeroUpgrade("MoneyIncomeIncrease10", "Increase the gold dropped by units by 10%", 300),
            new HeroUpgrade("MoneyIncomeIncrease20", "Increase the gold dropped by units by 20%", 600),
            new HeroUpgrade("MoneyIncomeIncrease30", "Increase the gold dropped by units by 30%", 900),
            new HeroUpgrade("MoneyIncomeIncrease40", "Increase the gold dropped by units by 40%", 1200),
            new HeroUpgrade("MoneyIncomeIncrease50", "Increase the gold dropped by units by 50%", 1500),
            new HeroUpgrade("MoneyIncomeIncrease60", "Increase the gold dropped by units by 60%", 1800),
            new HeroUpgrade("MoneyIncomeIncrease70", "Increase the gold dropped by units by 70%", 2100),
            new HeroUpgrade("MoneyIncomeIncrease80", "Increase the gold dropped by units by 80%", 2400),
            new HeroUpgrade("MoneyIncomeIncrease90", "Increase the gold dropped by units by 90%", 2700),
            new HeroUpgrade("MoneyIncomeIncrease100", "Increase the gold dropped by units by 100%", 3000),


            new HeroUpgrade("Shield0"),
            new HeroUpgrade("Shield5", "Your captain gains a shield that blocks the first 5 hit", 400),
            new HeroUpgrade("Shield10", "Your captain gains a shield that blocks the first 10 hit", 800),
            new HeroUpgrade("Shield15", "Your captain gains a shield that blocks the first 15 hit", 1200),
            new HeroUpgrade("Shield20", "Your captain gains a shield that blocks the first 20 hit", 1600),
            new HeroUpgrade("Shield25", "Your captain gains a shield that blocks the first 25 hit", 2000),
            new HeroUpgrade("Shield30", "Your captain gains a shield that blocks the first 30 hit", 2400),


            new HeroUpgrade("Phoenix0", "Your captain comeback to life once per stage (Must be bought again after using it)"),
            new HeroUpgrade("Phoenix1", "Your captain comeback to life once per stage (Must be bought again after using it)", 2000),

            new HeroUpgrade("AbilityRandomSpawn0", "Spawn for free 0 random unit"),
            new HeroUpgrade("AbilityRandomSpawn1", "Spawn for free 1 random unit (Cooldown: 25 seconds)", 400, 0, 25),
            new HeroUpgrade("AbilityRandomSpawn2", "Spawn for free 2 random unit (Cooldown: 25 => 35 seconds)", 900, 0, 35),
            new HeroUpgrade("AbilityRandomSpawn3", "Spawn for free 3 random unit (Cooldown: 35 => 50 seconds)", 1500, 0, 50),
            new HeroUpgrade("AbilityRandomSpawn4", "Spawn for free 4 random unit (Cooldown: 50 => 65 seconds)", 2200, 0, 65),
            new HeroUpgrade("AbilityDamageBuff0", "Increase units damage by x for 5 seconds"),
            new HeroUpgrade("AbilityDamageBuff1", "Increase units damage by 2.5 for 5 seconds (Cooldown: 30 seconds)", 500, 0, 30),
            new HeroUpgrade("AbilityDamageBuff2", "Increase units damage by 5 for 5 seconds (Cooldown: 30 => 40 seconds)", 1000, 0, 40),
            new HeroUpgrade("AbilityDamageBuff3", "Increase units damage by 7.5 for 5 seconds (Cooldown: 40 => 50 seconds)", 1500, 0, 50),
            new HeroUpgrade("AbilityDamageBuff4", "Increase units damage by 9 for 5 seconds (Cooldown: 50 => 60 seconds)", 2000, 0, 60),
            new HeroUpgrade("AbilityParalysis0", "Paralyse enemies for x seconds"),
            new HeroUpgrade("AbilityParalysis1", "Paralyse enemies for 1.5 seconds (Cooldown: 70 seconds)", 600, 0, 70),
            new HeroUpgrade("AbilityParalysis2", "Paralyse enemies for 3 seconds (Cooldown: 70 => 85 seconds)", 1200, 0, 85),
            new HeroUpgrade("AbilityParalysis3", "Paralyse enemies for 4.5 seconds (Cooldown: 85 => 100 seconds)", 1800, 0, 100),
            new HeroUpgrade("AbilityParalysis4", "Paralyse enemies for 6 seconds (Cooldown: 100 => 115 seconds)", 2000, 0, 115),
            new HeroUpgrade("AbilityLightning0", "Makes the lightning fall all enemies and makes them take x amount of damage"),
            new HeroUpgrade("AbilityLightning1", "Makes the lightning fall all enemies and makes them take 10 amount of damage (Cooldown: 30 seconds)", 500, 0, 30),
            new HeroUpgrade("AbilityLightning2", "Makes the lightning fall all enemies and makes them take 20 amount of damage (Cooldown: 30 => 40 seconds)", 1000, 0, 40),
            new HeroUpgrade("AbilityLightning3", "Makes the lightning fall all enemies and makes them take 30 amount of damage (Cooldown: 40 => 50 seconds)", 1500, 0, 50),
            new HeroUpgrade("AbilityLightning4", "Makes the lightning fall all enemies and makes them take 40 amount of damage (Cooldown: 50 => 60 seconds)", 2000, 0, 60),

        };
    }
    void InitFirstTimeUnlockedHeroUpgrades()
    {
        unlockedHeroUpgrades = new List<string>
        {
            "ManaMax100",
            "ManaRegen0.0",
            "StartMana0",
            "UnitCooldownReduction0",
            "MaxHealth100",
            "DamageReduction0",
            "MoneyIncomeIncrease0",
            "Shield0",
            "Phoenix0",
            "AbilityRandomSpawn0",
            "AbilityDamageBuff0",
            "AbilityParalysis0",
            "AbilityLightning0",
            "UnitDamageReduction0",
            "UnitDamageBonus0",
            "UnitMaxHealthBonus0",
        };
    }

    void InitUnits()
    {
        units = new List<Unit>
        {
            new Unit("Chicken1", 20, 6, 100, "Fast dps unit."),
            new Unit("Chicken2", 30, 8, 350, "Every 3 attacks, double hit"),
            new Unit("Chicken3", 70, 10, 1500, "Every 3 attacks, triple hit and 40% chance to dodge an attack"),
            new Unit("Chicken4", 120, 12, 4000, "When under 25% health, attack speed is increase by 500%, each hit increase attack damage by 5% until 50%)"),

            new Unit("Duck1", 20, 8, 100, "Tank unit, high health."),
            new Unit("Duck2", 35, 10, 300, "Every 3 hit received, block next hit"),
            new Unit("Duck3", 60, 12, 600, "Start with a big shield that blocks the 5 next hit, when big shield is down, receive defense bonus for x seconds, big shield reappears after x seconds."),
            new Unit("Duck4", 105, 14, 1800, "Big shield now blocks 10 next hit instead of 5, when big shield is down, receive defense bonus for x seconds, big shield reappears after x seconds. When any shield is up, restore x% max health over time."),

            new Unit("Trunk1", 25, 10, 150, "Shoot bullet."),
            new Unit("Trunk2", 50, 12, 450, "Shoot 3 times in row (on every attack)."),
            new Unit("Trunk3", 95, 14, 1100, "Increased shooting speed."),
            new Unit("Trunk4", 135, 16, 3300, "Gatling shooting."),

            new Unit("Rock1", 25, 10, 150, "Tank unit, high health."),
            new Unit("Rock2", 40, 12, 400, "Become super resistant (damage received reduced by a percentage) for an amount of time."),
            new Unit("Rock3", 100, 14, 1300, "When super resistant, sends back a percentage of damage received and projectiles are reflected."),
            new Unit("Rock4", 165, 16, 4500, "Become invulnerable for an amount of time."),

            new Unit("Bunny1", 20, 10, 130, "Summon weak rabbit."),
            new Unit("Bunny2", 35, 13, 350, "Instead of weak rabbit, randomly summon unique rabbit (dps or tank)."),
            new Unit("Bunny3", 65, 16, 900, "Instead of unique rabbit (dps or tank), summon an army of weak rabbits."),
            new Unit("Bunny4", 125, 19, 2200, "Instead of an army of weak rabbits, summon an army of unique rabbits (dps or tank)."),

            new Unit("Plant1", 30, 15, 200, "Shoot bullet at long distance."),
            new Unit("Plant2", 55, 17, 600, "Bullets go through 3 enemies."),
            new Unit("Plant3", 100, 19, 1300, "Bullet pierce through all enemies on a distance."),
            new Unit("Plant4", 170, 22, 3800, "Shoot an explosive bullet."),

            new Unit("Mushroom1", 55, 10, 900, "Aoe damage poison."),
            new Unit("Mushroom2", 90, 12, 1800, "Reduce enemies inflicted damage."),
            new Unit("Mushroom3", 150, 14, 3000, "Max health percentage damage, reduced inflicted damage and defense."),
            new Unit("Mushroom4", 250, 16, 6500, "After being poisoned for a certain amount of time, enemy are paralysed for a short amount of time."),

            new Unit("Toucan1", 20, 10, 140, "Dps unit."),
            new Unit("Toucan2", 50, 12, 500, "Summon a bird that will drop an explosive egg on enemies."),
            new Unit("Toucan3", 95, 14, 1600, "Summon a bird that will drop multiple explosive eggs on enemies."),
            new Unit("Toucan4", 180, 16, 4700, "Summon a bird that will drop multiple eggs that spawn small birds on enemies."),

            new Unit("Snail1", 15, 10, 100, "Give close allies additional health."),
            new Unit("Snail2", 30, 12, 300, "Every certain amount of time, heal close allies."),
            new Unit("Snail3", 60, 14, 800, "Constantly heal close allies."),
            new Unit("Snail4", 100, 16, 2000, "When an close ally is low life, start a strong global healing."),

            new Unit("FireSkull1", 75, 13, 1500, "Shoot fireball that explode (small radius)."),
            new Unit("FireSkull2", 100, 17, 3500, "Spawn fireball around him, after x seconds fireballs rush toward the closest enemies."),
            new Unit("FireSkull3", 200, 22, 8000, "Summon a meteor on enemies (aoe damage explosion)."),
            new Unit("FireSkull4", 300, 26, 20000, "Explosion that do enormous amount of damage then leave for x seconds magma on ground that damage enemies."),

            new Unit("Chameleon1", 20, 10, 150, "Each hit reduced 40% attack speed of the enemy touched (non cumulative)."),
            new Unit("Chameleon2", 35, 12, 450, "(Color red) : Rage effect, attack speed and damage increased."),
            new Unit("Chameleon3", 60, 14, 950, "(color:yellow) life steal on each hit and gain defense bonus."),
            new Unit("Chameleon4", 110, 16, 2300, "All previous state is kept for x second."),

            new Unit("Rhinoceros1", 30, 12, 200f, "More hp and faster than rock, charge: when up run faster, next attack is stronger, reload every x seconds (spawn with effect on)."),
            new Unit("Rhinoceros2", 50, 15, 550, "Charge now also knock back enemy."),
            new Unit("Rhinoceros3", 90, 19, 1200, "Charge is now aoe, stop after running x seconds. (keep hitting enemies on his way)."),
            new Unit("Rhinoceros4", 140, 22, 2900, "After charge is done, gain high defense bonus (ex: 80% damage reduction)."),

            new Unit("Pig1", 20, 16, 150, "Generate money every x seconds."),
            new Unit("Pig2", 40, 20, 400, "Generate mana every x seconds."),
            new Unit("Pig3", 75, 25, 1050, "Reduce units spawn cooldown."),
            new Unit("Pig4", 130, 30, 2600, "Increase 10% max mana (when pig is killed, mana max return to initial and current max over the initial is lost)."),

            new Unit("Slime1", 30, 10, 175, "When taking damage, chance to spawn a really small slime (chance rate scale with level)."),
            new Unit("Slime2", 50, 13, 600, "When killed, spawn two medium slime."),
            new Unit("Slime3", 85, 16, 1300, "When medium slime are killed, the spawn two small slime."),
            new Unit("Slime4", 135, 20, 3700, "Each spawn a specialized slime (dps, tank etc)."),

            new Unit("Ghost1", 35, 12, 250, "When far from enemy units, cannot be touched."),
            new Unit("Ghost2", 55, 15, 750, "Invincible for x seconds, every x seconds (scale with level)."),
            new Unit("Ghost3", 110, 18, 1800, "First enemy hit join ally camp, every x seconds."),
            new Unit("Ghost4", 190, 25, 5000, "Aoe attacks : inflict 20% max health, heal for 20% of damage inflict, reload every x seconds."),
        };

    }

    [System.Serializable]
    public class HeroUpgrade
    {
        public string name;
        public string description;
        public float shopPrice;
        public float cost;
        public float reloadTime;
        public HeroUpgrade(string name, string description = "", float shopPrice = 0, float cost = 0, float reloadTime = 0)
        {
            this.name = name;
            this.shopPrice = shopPrice;
            this.description = description;
            this.cost = cost;
            this.reloadTime = reloadTime;
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

