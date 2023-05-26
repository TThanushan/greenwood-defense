using UnityEngine;

public class EnemyCaptainStage1Stats : MonoBehaviour
{

    void Start()
    {
        Invoke(nameof(InitStats), 0.1f);
    }

    void InitStats()
    {
        if (StageInfosManager.instance.GetCurrentStageNumber() == 1)
        {
            GetComponent<Unit>().maxHealth = 5;
            GetComponent<Unit>().currentHealth = 5;
        }
    }

    void Update()
    {

    }
}
