using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;
    bool isVictory;
    bool isGameOver;

    GameObject levelCompletePanel;
    GameObject gameOverPanel;
    float goldEarnedInStage;

    PlayerStatsScript playerStatsScript;
    Unit playerCaptain;
    Unit enemyCaptain;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }



    private void Start()
    {
        InitVal();
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

        panel.transform.Find("GoldEarnedText").GetComponent<TMPro.TextMeshProUGUI>().text = goldEarnedInStage.ToString();
    }

    void DoVictory()
    {
        isVictory = true;
        levelCompletePanel.SetActive(true);
        UpdateLevelPanelInfos(levelCompletePanel);
        LevelScore.instance.CalculateScore();
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