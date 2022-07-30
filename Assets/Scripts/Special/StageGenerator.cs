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
            print("generate stage " + stageNumber);

        }
    }

    void GenerateStages()
    {
        int maxPrefabsIndex = GetMaxPrefabIndex();
        List<EnemyType> enemyTypes = new List<EnemyType>(maxPrefabsIndex + 1);
        for (int currentPrefabIndex = 0; currentPrefabIndex < maxPrefabsIndex; currentPrefabIndex++)
        {
            //print(prefabs[currentPrefabIndex].name);
            //EnemyType enemyType = new()
            //{
            //    Enemy = prefabs[currentPrefabIndex],
            //    TimeBetweenSpawn = GetTimeBetweenSpawn(currentPrefabIndex),
            //    RandomTimeBetweenSpawn = GetRandomTimeBetweenSpawn(currentPrefabIndex),
            //    EnemyCount = GetSpawnCount(currentPrefabIndex),
            //    InfiniteSpawning = IsSpawningInfinitly(currentPrefabIndex),

            //};

            //enemyTypes.Add(enemyType);
            AddEnemyType(currentPrefabIndex, ref prefabs, ref enemyTypes);
        }

        // Add boss.
        if (stageNumber / 50 >= 1)
        {
            print("bip");
            int index = 0;
            if (stageNumber == 100)
                index = 1;
            print("bip2");
            AddEnemyType(index, ref bossPrefabs, ref enemyTypes);
            //EnemyType enemyType = new()
            //{
            //    Enemy = bossPrefabs[index],
            //    TimeBetweenSpawn = GetTimeBetweenSpawn(index),
            //    RandomTimeBetweenSpawn = GetRandomTimeBetweenSpawn(index),
            //    EnemyCount = GetSpawnCount(index),
            //    InfiniteSpawning = IsSpawningInfinitly(index),

            //};
            //enemyTypes.Add(enemyType);
        }
        stageWhereToSave.enemyTypes = enemyTypes.ToArray();
    }

    void AddEnemyType(int index, ref GameObject[] prefabs, ref List<EnemyType> enemyTypes)
    {
        print(prefabs[index].name);
        EnemyType enemyType = new()
        {
            Enemy = prefabs[index],
            TimeBetweenSpawn = GetTimeBetweenSpawn(index),
            RandomTimeBetweenSpawn = GetRandomTimeBetweenSpawn(index),
            EnemyCount = GetSpawnCount(index),
            InfiniteSpawning = IsSpawningInfinitly(index),

        };

        enemyTypes.Add(enemyType);
    }


    bool IsSpawningInfinitly(int currentPrefabIndex)
    {
        return GetSpawnCount(currentPrefabIndex) <= 0;
    }

    int GetSpawnCount(int currentPrefabIndex)
    {
        int count = 0;
        //print(Mathf.RoundToInt(stageNumber / 5) + " == " + currentPrefabIndex);
        float fiveDiv = stageNumber / 5f;
        print(stageNumber / 5f);
        print("stage " + stageNumber + " " + fiveDiv + "===" + Mathf.RoundToInt(fiveDiv));
        if (fiveDiv == Mathf.RoundToInt(fiveDiv) && Mathf.RoundToInt(fiveDiv) == currentPrefabIndex)
            count = 1;
        return count;
    }

    int GetMaxPrefabIndex()
    {
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