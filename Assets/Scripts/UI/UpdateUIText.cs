using TMPro;
using UnityEngine;

public class UpdateUIText : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    PlayerStatsScript playerStatsScript;


    private void Start()
    {
        playerStatsScript = PlayerStatsScript.instance;
    }
    private void Update()
    {
        moneyText.text = playerStatsScript.money.ToString();
    }
}
