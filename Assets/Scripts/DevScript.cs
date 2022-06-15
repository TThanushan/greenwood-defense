using UnityEngine;

public class DevScript : MonoBehaviour
{
    private void Update()
    {

        SpeedManager();
    }

    void SpeedManager()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int speed = 4;
            if (Time.timeScale == speed)
                Time.timeScale = 1;
            else
                Time.timeScale = speed;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetData();
        }

        if (Input.GetKeyDown(KeyCode.M))
            PlayerStatsScript.instance.money += 100000;

        // Kill Enemy captain.
        if (Input.GetKeyDown(KeyCode.K))
            PoolObject.instance.enemyCaptain.Disabled = true;

        // Kill player Captain.
        if (Input.GetKeyDown(KeyCode.P))
            PoolObject.instance.playerCaptain.Disabled = true;

    }

    void ResetData()
    {
        PlayerStatsScript.instance.money = 0f;

    }
}
