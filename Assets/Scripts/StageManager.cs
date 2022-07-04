using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public float levelCompleteMoneyReward = 100;
    public static StageManager instance;
    bool isVictory;
    bool isGameOver;

    GameObject levelCompletePanel;
    GameObject gameOverPanel;
    float goldEarnedInStage;

    PlayerStatsScript playerStatsScript;
    Unit playerCaptain;
    Unit enemyCaptain;
    float rewardPreviouslyGiven;

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
    }

    public void GiveMoneyReward()
    {
        float reward = CalculateRewardAmount();
        PlayerStatsScript.instance.money += reward;
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
        int savedLevelSCore = SaveManager.instance.GetLevelScore();
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
        InitLevelCompleteMoneyReward();
    }

    void InitLevelCompleteMoneyReward()
    {
        float currentStageNb = GetNumbersOnly(StageInfosManager.instance.currentStage);
        if (currentStageNb == 1)
        {
            levelCompleteMoneyReward = Constants.LEVEL_COMPLETE_REWARD + 400;
            return;
        }
        int rewardCoef = (int)currentStageNb / 5;
        levelCompleteMoneyReward = Constants.LEVEL_COMPLETE_REWARD + rewardCoef * 50;
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
        playerStatsScript.money += money;
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
        panel.transform.Find("TotalGoldText").GetComponent<TMPro.TextMeshProUGUI>().text = playerStatsScript.money.ToString();

        panel.transform.Find("GoldEarnedText").GetComponent<TMPro.TextMeshProUGUI>().text = (goldEarnedInStage).ToString();
        if (panel.transform.Find("StageRewardText"))
            panel.transform.Find("StageRewardText").GetComponent<TMPro.TextMeshProUGUI>().text = '+' + (CalculateRewardAmount()).ToString() + '$';
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
        SaveManager.instance.SavePrefs();
        AudioManager.instance.PlaySfx(Constants.VICTORY_SFX_NAME);
        levelCompletePanel.GetComponent<LevelComplete>().enabled = true;
    }

    void DoGameOver()
    {
        Time.timeScale = 1;
        isGameOver = true;
        gameOverPanel.SetActive(true);
        UpdateLevelPanelInfos(gameOverPanel);
        PlayerStatsScript.instance.PauseGame(true);
        gameOverPanel.SetActive(true);

        SaveManager.instance.SavePrefs();
    }

    public void LoadNextLevel()
    {
        //int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StageInfosManager.instance.SetCurrentStageToNextStage();
        //MenuScript.instance.LoadScene(nextSceneIndex);
        MenuScript.instance.LoadCurrentScene();
    }

    void UnlockNextStage()
    {
        int index = SaveManager.instance.GetCurrentLevelNumber();
        SaveManager.instance.UnlockLevel(index + 1);
    }

    bool IsScoreHigherThanPrevious(int index)
    {
        return LevelScore.instance.score > SaveManager.instance.GetLevelScore(index);
    }

    void SaveScoreIfHigher()
    {
        int index = SaveManager.instance.GetCurrentLevelNumber();
        if (IsScoreHigherThanPrevious(index))
            SaveManager.instance.SaveLevelScore(index, LevelScore.instance.score);
    }

}