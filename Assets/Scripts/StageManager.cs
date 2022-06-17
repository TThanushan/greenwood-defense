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
    }



    private void Start()
    {
        InitVal();
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

        panel.transform.Find("GoldEarnedText").GetComponent<TMPro.TextMeshProUGUI>().text = (goldEarnedInStage + CalculateRewardAmount()).ToString();
    }

    void DoVictory()
    {
        isVictory = true;
        levelCompletePanel.SetActive(true);
        rewardPreviouslyGiven = GetRewardPreviouslyGiven();
        LevelScore.instance.CalculateScore();
        GiveMoneyReward();
        UpdateLevelPanelInfos(levelCompletePanel);
        SaveData();
    }

    void DoGameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        UpdateLevelPanelInfos(gameOverPanel);

        Time.timeScale = 1;
        PlayerStatsScript.instance.PauseGame(true);
        gameOverPanel.SetActive(true);
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        MenuScript.instance.LoadScene(nextSceneIndex);
    }

    void SaveData()
    {
        int index = SaveManager.instance.GetCurrentLevelNumber();
        SaveManager.instance.UnlockLevel(index + 1);
        if (LevelScore.instance.score > SaveManager.instance.GetLevelScore(index))
            SaveManager.instance.SaveLevelScore(index, LevelScore.instance.score);
        SaveManager.instance.SavePrefs();
    }

}