using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateUI : MonoBehaviour
{
    TextMeshProUGUI moneyText;
    TextMeshProUGUI timeText;

    float timer;
    PlayerStatsScript playerStatsScript;

    private void Start()
    {
        playerStatsScript = PlayerStatsScript.instance;
        moneyText = transform.Find("RightGroup/MoneyTextGroup/MoneyText").GetComponent<TextMeshProUGUI>();
        timeText = transform.Find("TopGroup/Time/TimeText").GetComponent<TextMeshProUGUI>();
        transform.Find("TopGroup/Title/TitleText").GetComponent<TextMeshProUGUI>().text = GetStageTitle();
    }

    private void Update()
    {
        moneyText.text = playerStatsScript.money.ToString();
        timeText.text = GetFormatedTime();

    }
    string GetStageNumber()
    {
        return SceneManager.GetActiveScene().name.Split(' ')[1];
    }

    string GetStageTitle()
    {
        return "Stage " + GetStageNumber();
    }

    public Vector2 GetMoneyTextPosition()
    {
        return moneyText.transform.position;
    }

    string GetFormatedTime()
    {
        timer += Time.deltaTime;

        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");

        //print(string.Format("{0}:{1}", minutes, seconds));
        return string.Format("{0}:{1}", minutes, seconds);
    }



}
