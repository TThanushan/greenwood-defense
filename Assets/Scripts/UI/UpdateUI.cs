using TMPro;
using UnityEngine;

public class UpdateUI : MonoBehaviour
{
    TextMeshProUGUI moneyText;
    TextMeshProUGUI timeText;

    float timer;

    private void Start()
    {
        moneyText = transform.Find("RightGroup/MoneyTextGroup/MoneyText").GetComponent<TextMeshProUGUI>();
        timeText = transform.Find("LeftGroup/Time/TimeText").GetComponent<TextMeshProUGUI>();
        transform.Find("TopGroup/Title/TitleText").GetComponent<TextMeshProUGUI>().text = GetStageTitle();
    }

    private void Update()
    {
        int money = (int)SaveManager.instance.money;
        moneyText.text = money.ToString();
        timeText.text = GetFormatedTime();
    }

    string GetStageTitle()
    {
        return StageInfosManager.instance.GetCurrentStageName();
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
