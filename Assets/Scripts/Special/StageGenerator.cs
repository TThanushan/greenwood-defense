using System.Collections.Generic;
using UnityEngine;
using static Stage;

public class StageGenerator : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject[] bossPrefabs;
    public Stage stageWhereToSave;
    public GameObject meleeFrog;

    //Test
    public int tmpStageNumber;
    public bool generate;
    //Test

    int maxPrefabIndex;
    int stageNumber;
    private void Awake()
    {
        stageNumber = StageInfosManager.instance.GetCurrentStageNumber();
        //stageNumber = tmpStageNumber;
        maxPrefabIndex = GetMaxPrefabIndex();
        GenerateStages();

    }


    private void Update()
    {
        //Test();
    }
    void Test()
    {
        if (!generate)
            return;
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //stageNumber = StageInfosManager.instance.GetCurrentStageNumber();
        stageNumber = tmpStageNumber;
        GenerateStages();
        print("generate stage " + stageNumber);
        //}
    }

    void GenerateStages()
    {

        List<EnemyType> enemyTypes = new List<EnemyType>(maxPrefabIndex + 1);

        // Add melee frog for early game.
        if (stageNumber < 10)
        {
            EnemyType enemyType = new()
            {
                Enemy = meleeFrog,
                TimeBetweenSpawn = GetTimeBetweenSpawn(0),
                RandomTimeBetweenSpawn = GetRandomTimeBetweenSpawn(0),
                EnemyCount = GetSpawnCount(0),
                InfiniteSpawning = IsSpawningInfinitly(0),
            };

            enemyTypes.Add(enemyType);
        }


        for (int currentPrefabIndex = 0; currentPrefabIndex < maxPrefabIndex; currentPrefabIndex++)
        {
            AddEnemyType(currentPrefabIndex, ref prefabs, ref enemyTypes);
        }

        float stageDivided = stageNumber / 50f;
        // Add boss.
        if (stageDivided == 1 || stageDivided == 2)
        {
            print("stageDi" + stageDivided);
            int index = 0;
            if (stageDivided == 2)
                index = 1;
            EnemyType enemyType = GetEnemyType(index, ref bossPrefabs);
            enemyType.InfiniteSpawning = false;
            enemyType.EnemyCount = 1;
            enemyTypes.Add(enemyType);
            maxPrefabIndex++;
        }
        stageWhereToSave.enemyTypes = enemyTypes.ToArray();
    }

    EnemyType GetEnemyType(int index, ref GameObject[] prefabs)
    {
        EnemyType enemyType = new()
        {
            Enemy = prefabs[index],
            TimeBetweenSpawn = GetTimeBetweenSpawn(index),
            RandomTimeBetweenSpawn = GetRandomTimeBetweenSpawn(index),
            EnemyCount = GetSpawnCount(index),
            InfiniteSpawning = IsSpawningInfinitly(index),
            TimeBeforeFirstSpawn = index / 2f

        };
        return enemyType;
    }

    void AddEnemyType(int index, ref GameObject[] prefabs, ref List<EnemyType> enemyTypes)
    {
        enemyTypes.Add(GetEnemyType(index, ref prefabs));
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
        //print("stage " + stageNumber + " " + fiveDiv + "===" + Mathf.RoundToInt(fiveDiv));
        if (fiveDiv == Mathf.RoundToInt(fiveDiv) && Mathf.RoundToInt(fiveDiv) == currentPrefabIndex)
            count = 3;
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
        int coef = stageNumber / 5;
        int coef2 = maxPrefabIndex - currentPrefabIndex;
        float fullCoef = coef * coef2;
        newTime = minTime - (0.075f * fullCoef);
        //print($"coef : {coef}, coef2 : {coef2}, fullCoef: {fullCoef}, newTime : {newTime}");
        return newTime;
    }

    float GetRandomTimeBetweenSpawn(int currentPrefabIndex)
    {
        float[] randomTimeRange = { 3.5f, 4f, 4.5f, 5f };
        int count = stageNumber;
        // index can also start at 0, 2 allow to match the old stages values.
        int index = randomTimeRange.Length;
        for (int i = 0; i < count; i++)
        {
            index--;
            if (index < 0)
                index = randomTimeRange.Length - 1;
        }
        return randomTimeRange[index];
    }
}