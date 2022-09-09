using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelComplete : MonoBehaviour
{
    public float scoreTweenTime = 1f;
    public LeanTweenType leanTweenType;


    Image scoreBar;
    float scoreBarFillAmountMax;
    TweenSize star1;
    TweenSize star2;
    TweenSize star3;
    TextMeshProUGUI scoreText;
    bool animationDone;
    private void Awake()
    {
        scoreBar = transform.Find(Constants.LEVEL_COMPLETE_BAR_FRONT_PATH).GetComponent<Image>();
        star1 = transform.Find(Constants.LEVEL_COMPLETE_STAR1_STAR_UNLOCKED).GetComponent<TweenSize>();
        star2 = transform.Find(Constants.LEVEL_COMPLETE_STAR2_STAR_UNLOCKED).GetComponent<TweenSize>();
        star3 = transform.Find(Constants.LEVEL_COMPLETE_STAR3_STAR_UNLOCKED).GetComponent<TweenSize>();
        scoreText = transform.Find(Constants.LEVEL_COMPLETE_BAR_SCORE_PATH).GetComponent<TextMeshProUGUI>();
        //InvokeRepeating("PlayScoreBarFillingSFX", 0f, 0.1f);

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.I))
        //    StartAnimation();
        //if (Input.GetKeyDown(KeyCode.R))
        //    DelmeReset();
        AnimateStars();
        UpdateScoreText();
        ShowButtons();


        //ShowButtons();
    }
    private void OnEnable()
    {
        StartFillingAnimation();


    }
    //void UpdateTitleText()
    //{
    //    transform.Find("Title/Title Text").GetComponent<TextMeshProUGUI>().text = SpawnerScript.instance.waves.name;
    //}
    void ShowButtons()
    {
        // Don't show while...
        if (animationDone || !Mathf.Approximately(scoreBar.fillAmount, scoreBarFillAmountMax))
            return;
        transform.Find("Popup/Group_Buttons/Button_Home").gameObject.SetActive(true);
        transform.Find("Popup/Group_Buttons/Button_Restart").gameObject.SetActive(true);
        transform.Find("Popup/Group_Buttons/Button_Next").gameObject.SetActive(true);

        animationDone = true;

        //transform.Find("NextLevelButton").GetComponent<TweenSize>().twe
    }
    void UpdateScoreText()
    {
        float val = scoreBar.fillAmount * 10f;
        val = Mathf.RoundToInt(val);
        scoreText.text = val.ToString();

    }
    void PlayScoreBarFillingSFX()
    {
        if (scoreBar.fillAmount < scoreBarFillAmountMax)
            AudioManager.instance.Play("ButtonClick");

    }
    void EnableStar(TweenSize star)
    {
        AudioManager.instance.Play("StarUnlocked");
        AudioManager.instance.Play("StarUnlocked");
        star.gameObject.SetActive(true);
        star.Tween();
    }
    void AnimateStars()
    {
        int score = (int)(scoreBar.fillAmount * 100);
        if (!star1.gameObject.activeSelf && score > LevelScore.instance.oneStar)
        {
            EnableStar(star1);
        }
        else if (!star2.gameObject.activeSelf && score >= LevelScore.instance.twoStar)
        {
            EnableStar(star2);
        }
        else if (!star3.gameObject.activeSelf && score == LevelScore.instance.threeStar)
        {
            EnableStar(star3);
        }

    }


    void StartFillingAnimation()
    {
        scoreBar.fillAmount = 0f;
        // Increase score bar fill amount.
        float value = scoreBar.fillAmount;
        float min = scoreBar.fillAmount;
        //scoreBarFillAmountMax = (float)decimal.Divide(PlayerStatsScript.instance.life, PlayerStatsScript.instance.StartLife);
        scoreBarFillAmountMax = (float)decimal.Divide(LevelScore.instance.score, Constants.MAX_SCORE);
        LeanTween.value(min, scoreBarFillAmountMax, scoreTweenTime)
        .setEase(leanTweenType)
        .setOnUpdate((float val) =>
        {
            scoreBar.fillAmount = val;

        });
        // Tween star1 when having enough for star1.


    }
}
