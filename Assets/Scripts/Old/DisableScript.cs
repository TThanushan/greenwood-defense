using UnityEngine;

public class DisableScript : MonoBehaviour
{

    public float disableTime = 1f;
    float disableTimeCount;

    void OnEnable()
    {
        disableTimeCount = disableTime;
    }

    void Update()
    {
        if (disableTimeCount <= 0f)
            Destroy();
        disableTimeCount -= Time.deltaTime;
    }

    void Destroy()
    {
        gameObject.SetActive(false);
    }
}
