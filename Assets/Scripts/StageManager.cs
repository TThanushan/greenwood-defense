using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public GameObject playerCaptain;
    public GameObject enemyCaptain;
    public bool isVictory;
    public bool isGameOver;

    public GameObject levelCompletePanel;
    public GameObject gameOverPanel;

    PlayerStatsScript playerStatsScript;
    Unit playerCaptainUnit;
    Unit enemyCaptainUnit;

    private void Start()
    {
        playerStatsScript = PlayerStatsScript.instance;
        playerCaptainUnit = GameObject.Find("PlayerCaptain").GetComponent<Unit>();
        enemyCaptainUnit = GameObject.Find("EnemyCaptain").GetComponent<Unit>();
    }

    private void Update()
    {
        CheckIsGameOver();
        UpdatePlayerLife();
    }

    void UpdatePlayerLife()
    {
        playerStatsScript.life = (int)playerCaptainUnit.currentHealth;
    }

    bool HasPlayerLoose()
    {
        return playerCaptainUnit.Disabled;
    }

    bool HasPlayerWin()
    {
        return enemyCaptainUnit.Disabled;
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

    void DoVictory()
    {
        isVictory = true;
        levelCompletePanel.SetActive(true);

        LevelScore.instance.CalculateScore();

        SaveData();
    }

    void DoGameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);

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