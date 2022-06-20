using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    private void Start()
    {
        transform.Find("LeftGroup/GoldText").GetComponent<TMPro.TextMeshProUGUI>().text = "Money : " + PlayerStatsScript.instance.money.ToString();
    }


}
