using System.Collections.Generic;
using UnityEngine;
using static Stage;

public class StageGenerator : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject[] bossPrefabs;
    public Stage stageWhereToSave;

    int stageNumber;

    private void Update()
    {
        stageNumber = StageInfosManager.instance.GetCurrentStageNumber();
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GenerateStages();

        }
    }

    void GenerateStages()
    {
        int maxPrefabsIndex = GetMaxPrefabIndex();
        List<EnemyType> enemyTypes = new List<EnemyType>(maxPrefabsIndex);
        for (int currentPrefabIndex = 0; currentPrefabIndex < maxPrefabsIndex; currentPrefabIndex++)
        {
            EnemyType enemyType = new()
            {
                Enemy = prefabs[currentPrefabIndex],
                TimeBetweenSpawn = GetTimeBetweenSpawn(currentPrefabIndex),
                RandomTimeBetweenSpawn = GetRandomTimeBetweenSpawn(currentPrefabIndex)
            };

            enemyTypes.Add(enemyType);
        }

        stageWhereToSave.enemyTypes = enemyTypes.ToArray();
    }

    int GetMaxPrefabIndex()
    {
        print("stage number : " + stageNumber);
        return stageNumber / 5 + 1;
    }

    float GetTimeBetweenSpawn(int currentPrefabIndex)
    {
        float minTime = 10f;
        float newTime;
        int coef = stageNumber / 4;
        int coef2 = GetMaxPrefabIndex() - currentPrefabIndex;
        float fullCoef = coef * coef2;
        newTime = minTime - (0.25f * fullCoef);
        //print($"coef : {coef}, fullCoef: {fullCoef}, newTime : {newTime}");
        return newTime;
    }

    float GetRandomTimeBetweenSpawn(int currentPrefabIndex)
    {
        float randomTime = GetTimeBetweenSpawn(currentPrefabIndex);

        return randomTime / 2;
    }


}