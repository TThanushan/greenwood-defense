using UnityEngine;

public class DevScript : MonoBehaviour
{
    private void Update()
    {

        SpeedManager();

        if (Input.GetKeyDown(KeyCode.C))
        {

            PlayerStatsScript.instance.money = 500;
            SaveManager.instance.SavePrefs();

        }

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
        {
            print("give money");
            PlayerStatsScript.instance.money += 100000;
            SaveManager.instance.SavePrefs();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            print("give money");
            PlayerStatsScript.instance.money += 100;
            SaveManager.instance.SavePrefs();
        }

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
