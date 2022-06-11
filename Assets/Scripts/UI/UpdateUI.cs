using TMPro;
using UnityEngine;

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
    }
    private void Update()
    {
        moneyText.text = playerStatsScript.money.ToString();
        timeText.text = GetFormatedTime();
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
