using UnityEngine;

[System.Serializable]
public class PlayerStatsScript
{

    private static PlayerStatsScript _instance;
    public static PlayerStatsScript instance
    {
        get
        {
            if (_instance == null)
                _instance = new PlayerStatsScript();
            return _instance;
        }
    }

    public int life;
    public float money;
    public bool pause;
    public float timeWhenPaused;
    public bool isTutorialDisable = true;

    public bool IsGamePaused { get { return pause; } }

    public int StartLife { get => startLife; }
    public float StartMoney { get => startMoney; }

    public delegate void PauseAction();
    public static event PauseAction PauseEvent;

    public delegate void UnpauseAction();
    public static event UnpauseAction UnpauseEvent;

    private int startLife = 10;
    private float startMoney = 300;



    public void PauseGame()
    {
        pause = !pause;
        if (pause)
        {
            timeWhenPaused = Time.time;
            PauseEvent?.Invoke();
        }
        else if (!pause && UnpauseEvent != null)
            UnpauseEvent();
    }
    public void PauseGame(bool value)
    {
        pause = value;
        if (pause)
        {
            timeWhenPaused = Time.time;
            PauseEvent?.Invoke();
        }
        else if (!pause && UnpauseEvent != null)
            UnpauseEvent();
    }
    public float GetTimeElapsedSincePause()
    {
        return Time.time - timeWhenPaused;
    }
}
