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
    }
}
