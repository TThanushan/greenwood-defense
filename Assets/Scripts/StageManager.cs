using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    float levelCompleteMoneyReward = 100;
    public static StageManager instance;
    bool isVictory;
    bool isGameOver;

    GameObject levelCompletePanel;
    GameObject gameOverPanel;
    float goldEarnedInStage;
    SaveManager saveManager;

    PlayerStatsScript playerStatsScript;
    Unit playerCaptain;
    Unit enemyCaptain;
    float rewardPreviouslyGiven;

    float moneyIncomeIncrease = 1f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        InitVal();
        TrackPlayer.instance.PlayMainTheme();
        LoadStatsFromPrefs();
    }

    void LoadStatsFromPrefs()
    {
        foreach (string name in SaveManager.instance.unlockedHeroUpgrades)
        {
            const string MONEY_INCOME_INCREASE = "MoneyIncomeIncrease";
            if (name.Contains(MONEY_INCOME_INCREASE))
            {
                moneyIncomeIncrease = 1 + GetUpgradeNameNumbersOnly(name) / 100;
                return;
            }
        }

    }
    float GetUpgradeNameNumbersOnly(string upgradeName)
    {
        string withoutNumbers = GetUpgradeNameWithoutNumbers(upgradeName);
        withoutNumbers = upgradeName.Replace(withoutNumbers, "");
        return float.Parse(withoutNumbers, CultureInfo.InvariantCulture.NumberFormat);
    }
    string GetUpgradeNameWithoutNumbers(string upgradeName)
    {
        string withoutNumbers = Regex.Replace(upgradeName, @"[\d-]", string.Empty);
        withoutNumbers = withoutNumbers.Replace(".", "");
        return withoutNumbers;
    }

    public void GiveMoneyReward()
    {
        float reward = CalculateRewardAmount();
        saveManager.money += reward;
    }

    float CalculateRewardAmount()
    {
        float reward = levelCompleteMoneyReward;
        int stars = LevelScore.instance.HowManyStar();
        reward = GetRewardAfterPercentage(reward, stars);
        if (reward <= rewardPreviouslyGiven)
            reward = 0f;
        else
            reward -= rewardPreviouslyGiven;
        return reward;
    }

    float GetRewardPreviouslyGiven()
    {
        float reward = levelCompleteMoneyReward;
        int savedLevelSCore = saveManager.GetLevelScore();
        int stars = LevelScore.instance.HowManyStar(savedLevelSCore);
        return GetRewardAfterPercentage(reward, stars);
    }

    float GetRewardAfterPercentage(float reward, int stars)
    {
        if (stars == 2)
            reward *= 0.66f;
        else if (stars == 1)
            reward *= 0.33f;
        else if (stars == 0)
            reward = 0f;
        return reward;
    }
    void InitVal()
    {
        playerStatsScript = PlayerStatsScript.instance;
        playerCaptain = GameObject.Find("PlayerCaptain").GetComponent<Unit>();
        enemyCaptain = GameObject.Find("EnemyCaptain").GetComponent<Unit>();
        gameOverPanel = transform.Find("MiddleGroup/GameOverPanel").gameObject;
        levelCompletePanel = transform.Find("MiddleGroup/LevelCompletePanel").gameObject;
        saveManager = SaveManager.instance;
        InitLevelCompleteMoneyReward();
    }

    void InitLevelCompleteMoneyReward()
    {
        float currentStageNb = GetNumbersOnly(StageInfosManager.instance.currentStage);
        if (currentStageNb == 1)
        {
            levelCompleteMoneyReward = Constants.LEVEL_COMPLETE_REWARD + Constants.FIRST_STAGE_REWARD_BONUS;
            return;
        }
        int rewardCoef = (int)currentStageNb / 5;
        levelCompleteMoneyReward = Constants.LEVEL_COMPLETE_REWARD + rewardCoef * Constants.REWARD_BONUS_EVERY_X_STAGE;
    }

    float GetNumbersOnly(string numberString)
    {
        string withoutNumbers = GetWithoutNumbers(numberString);
        withoutNumbers = numberString.Replace(withoutNumbers, "");
        return float.Parse(withoutNumbers, CultureInfo.InvariantCulture.NumberFormat);
    }
    string GetWithoutNumbers(string numberString)
    {
        string withoutNumbers = Regex.Replace(numberString, @"[\d-]", string.Empty);
        withoutNumbers = withoutNumbers.Replace(".", "");
        return withoutNumbers;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += ResetGoldEarnedInStage;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ResetGoldEarnedInStage;

    }

    void ResetGoldEarnedInStage(Scene scene, LoadSceneMode mode)
    {
        goldEarnedInStage = 0f;
    }

    private void Update()
    {
        CheckIsGameOver();
        UpdatePlayerLife();
    }

    public void GivePlayerMoney(float money)
    {
        money *= moneyIncomeIncrease;
        money = Mathf.Round(money);
        saveManager.money += money;
        goldEarnedInStage += money;
    }

    void UpdatePlayerLife()
    {
        playerStatsScript.life = (int)playerCaptain.currentHealth;
    }

    bool HasPlayerLoose()
    {
        return playerCaptain.Disabled;
    }

    bool HasPlayerWin()
    {
        return enemyCaptain.Disabled;
    }

    void CheckIsGameOver()
    {
        if (isGameOver || isVictory)
            return;

        if (HasPlayerLoose())
            DoGameOver();
        else if (HasPlayerWin())
            DoVictory();
    }

    void UpdateLevelPanelInfos(GameObject panel)
    {
        panel.transform.Find("TotalGoldText").GetComponent<TMPro.TextMeshProUGUI>().text = saveManager.money.ToString();

        panel.transform.Find("GoldEarnedText").GetComponent<TMPro.TextMeshProUGUI>().text = (goldEarnedInStage).ToString();
        if (panel.transform.Find("StageRewardText"))
            panel.transform.Find("StageRewardText").GetComponent<TMPro.TextMeshProUGUI>().text = '+' + Mathf.Round((CalculateRewardAmount())).ToString() + '$';
    }

    void DoVictory()
    {

        Time.timeScale = 1;
        isVictory = true;
        levelCompletePanel.SetActive(true);
        rewardPreviouslyGiven = GetRewardPreviouslyGiven();
        LevelScore.instance.CalculateScore();

        GiveMoneyReward();
        UpdateLevelPanelInfos(levelCompletePanel);
        UnlockNextStage();
        SaveScoreIfHigher();
        saveManager.SavePrefIfAutoSave();
        AudioManager.instance.PlaySfx(Constants.VICTORY_SFX_NAME);
        levelCompletePanel.GetComponent<LevelComplete>().enabled = true;
        GameObject.FindGameObjectWithTag("Spawner").SetActive(false);

    }

    void DoGameOver()
    {
        Time.timeScale = 1;
        isGameOver = true;
        gameOverPanel.SetActive(true);
        UpdateLevelPanelInfos(gameOverPanel);
        gameOverPanel.SetActive(true);

        saveManager.SavePrefIfAutoSave();
    }

    public void LoadNextLevel()
    {
        //int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        int currentStageNumber = StageInfosManager.instance.GetCurrentStageNumber();
        StageInfosManager.instance.SetCurrentStageToNextStage();
        if (currentStageNumber == Constants.MAX_STAGE_NUMBER)
            MenuScript.instance.LoadLevelSelectionScene();
        else
            MenuScript.instance.LoadCurrentScene();
        //MenuScript.instance.LoadScene(nextSceneIndex);
    }
    void UnlockNextStage()
    {
        int index = saveManager.GetCurrentLevelNumber();
        saveManager.UnlockLevel(index + 1);
    }

    bool IsScoreHigherThanPrevious(int index)
    {
        return LevelScore.instance.score > saveManager.GetLevelScore(index);
    }

    void SaveScoreIfHigher()
    {
        int index = saveManager.GetCurrentLevelNumber();
        if (IsScoreHigherThanPrevious(index))
            saveManager.SaveLevelScore(index, LevelScore.instance.score);
    }

}