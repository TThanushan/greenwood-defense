using UnityEngine;
using static Stage;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPosition;
    Stage stage;
    PoolObject poolObject;

    int stageNumber;
    void Start()
    {
        poolObject = PoolObject.instance;
        //stage = (Stage)Resources.Load("Stages/" + StageInfosManager.instance.GetCurrentStageName());
        stage = (Stage)Resources.Load("Stages/GeneratedStage");
        InitEnemyTypes();
        stageNumber = StageInfosManager.instance.GetCurrentStageNumber();
    }

    void Update()
    {
        TrySpawn();
    }


    float GetIncreasedHealthUsingStageNumber(float health)
    {
        return Mathf.FloorToInt(health * (1 + stageNumber / 100));
    }

    void InitEnemyTypes()
    {
        foreach (EnemyType enemyType in stage.enemyTypes)
        {
            enemyType.Init();
        }
    }

    void TrySpawn()
    {
        EnemyType[] enemyTypes = stage.enemyTypes;

        if (enemyTypes.Length == 0)
            return;
        float TIME_THRESHOLD_TO_SKIP_UNIT = 2f;
        foreach (EnemyType enemyType in enemyTypes)
        {
            if (!enemyType.ReadyToSpawn() || enemyType.TimeBetweenSpawn < TIME_THRESHOLD_TO_SKIP_UNIT)
                continue;
            Spawn(enemyType);
        }
    }

    public void Spawn(EnemyType enemyType)
    {
        GameObject newEnemy = poolObject.GetPoolObject(enemyType.Enemy);
        newEnemy.transform.position = GetRandomSpawnPosition();
        UpdateUnitStatsUsingStageNumber(newEnemy);
        enemyType.DecreaseCount();
        enemyType.SetRandomNextEnemySpawnTime();
    }

    void UpdateUnitStatsUsingStageNumber(GameObject enemy)
    {
        Unit unit = enemy.GetComponent<Unit>();
        unit.maxHealth = GetIncreasedHealthUsingStageNumber(unit.maxHealth);
        unit.currentHealth = GetIncreasedHealthUsingStageNumber(unit.currentHealth);

    }

    Vector2 GetRandomSpawnPosition()
    {
        float radius = 0.15f;
        Vector2 randomPos = (Vector2)spawnPosition.position + Random.insideUnitCircle * radius;

        return randomPos;
    }
}
