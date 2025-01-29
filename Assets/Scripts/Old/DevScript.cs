using UnityEngine;

public class DevScript : MonoBehaviour
{
#if UNITY_EDITOR
    private void Update()
    {
        SpeedManager();

        //if (Input.GetKeyDown(KeyCode.C))
        //{

        //    SaveManager.instance.money = 500;
        //    SaveManager.instance.SavePrefs();

        //}

    }

    void SpeedManager()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int speed = 10;
            Time.timeScale = Time.timeScale == speed ? 1 : speed;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetData();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            SaveManager.instance.money += 100000;
            SaveManager.instance.SavePrefs();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SaveManager.instance.money += 100;
            SaveManager.instance.SavePrefs();
        }

        // Kill Enemy captain.
        if (Input.GetKeyDown(KeyCode.K))
            WinCurrentLevel();

        // Kill player Captain.
        if (Input.GetKeyDown(KeyCode.P))
            PoolObject.instance.playerCaptain.Disabled = true;
    }


    void ResetData()
    {
        SaveManager.instance.money = 0f;

    }
#endif
    /// For testing purposes.
    public void WinCurrentLevel()
    {
        PoolObject.instance.enemyCaptain.Disabled = true;
    }
}
