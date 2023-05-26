using UnityEngine;
using static Stage;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPosition;
    public Stage stage1;
    public static EnemySpawner instance;

    Stage stage;
    PoolObject poolObject;

    int stageNumber;
    ShowNewEnemyDescriptionCard showNewEnemyDescriptionCard;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        poolObject = PoolObject.instance;
        //stage = (Stage)Resources.Load("Stages/" + StageInfosManager.instance.GetCurrentStageName());
        stageNumber = StageInfosManager.instance.GetCurrentStageNumber();
        if (stageNumber == 1)
            stage = stage1;
        else
            stage = (Stage)Resources.Load("Stages/GeneratedStage");

        InitEnemyTypes();
        showNewEnemyDescriptionCard = ShowNewEnemyDescriptionCard.instance;
    }


    void Update()
    {
        TrySpawn();
    }

    public Stage GetStage()
    {
        return stage;
    }

    float GetIncreasedHealthUsingStageNumber(float health)
    {
        return Mathf.FloorToInt(health * (1 + (stageNumber / 100)));
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
        if (Time.timeSinceLevelLoad < Constants.TIME_BEFORE_FIRST_ENEMY_SPAWN || showNewEnemyDescriptionCard.IsDescriptionCardEnabled())
            return;


        EnemyType[] enemyTypes = stage.enemyTypes;
        if (enemyTypes.Length == 0)
            return;
        foreach (EnemyType enemyType in enemyTypes)
        {
            if (!enemyType.ReadyToSpawn() || (enemyType.TimeBetweenSpawn < Constants.TIME_THRESHOLD_TO_SKIP_UNIT && enemyType.InfiniteSpawning))
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
        if (newEnemy.name.Contains("Ultimate"))
            UltimateHealthBar.instance.SetUltimateReference(newEnemy.GetComponent<Unit>());
        if (!newEnemy.name.Contains("Frog"))
        {
            newEnemy.GetComponent<Unit>().SetTargetTag("Ally");
            newEnemy.GetComponent<Unit>().RotateSprite();
        }
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
        Vector2 randomPos = (Vector2)spawnPosition.position + (Random.insideUnitCircle * radius);

        return randomPos;
    }
}
