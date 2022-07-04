using UnityEngine;

public class DestroyInTime : MonoBehaviour
{

    public float destroyTime = 1f;
    float destroyTimeCount;

    void OnEnable()
    {
        destroyTimeCount = destroyTime;
    }

    void Update()
    {
        if (destroyTimeCount <= 0f)
            Destroy();
        destroyTimeCount -= Time.deltaTime;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
