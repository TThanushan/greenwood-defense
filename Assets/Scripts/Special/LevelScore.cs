using UnityEngine;

public class LevelScore : MonoBehaviour
{
    public static LevelScore instance;
    public int score;

    public int threeStar = 100;
    public int twoStar = 50;
    public int oneStar = 30;
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
        score = (int)(decimal.Divide(pScript.life, pScript.StartLife) * 100);

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
