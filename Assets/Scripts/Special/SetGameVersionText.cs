using UnityEngine;

public class SetGameVersionText : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Made by Wombart v" + Constants.GAME_VERSION;
    }
}
