using UnityEngine;

public class LevelScore : MonoBehaviour
{
    public static LevelScore instance;
    public int score;

    public int threeStar = 75;
    public int twoStar = 50;
    public int oneStar = 25;
    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void CalculateScore()
    {
        PlayerStatsScript pScript = PlayerStatsScript.instance;
        score = (int)(PoolObject.instance.playerCaptain.currentHealth / PoolObject.instance.playerCaptain.maxHealth * 100);
        //score = (int)(decimal.Divide(pScript.life, pScript.StartLife) * 10);

    }

    public int HowManyStar()
    {
        return HowManyStar(score);
    }
    public int HowManyStar(int _score)
    {


        if (_score >= threeStar)
            return 3;
        else if (_score >= twoStar)
            return 2;
        else if (_score >= oneStar)
            return 1;
        return 0;
    }
}
