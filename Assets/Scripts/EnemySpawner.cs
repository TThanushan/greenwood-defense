using UnityEngine;
using UnityEngine.SceneManagement;
using static Stage;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPosition;
    Stage stage;
    PoolObject poolObject;
    void Start()
    {
        poolObject = PoolObject.instance;
        stage = (Stage)Resources.Load("Stages/" + SceneManager.GetActiveScene().name);
        InitEnemyTypes();
    }

    void Update()
    {
        Spawn();
    }

    string GetStageNumber()
    {
        return SceneManager.GetActiveScene().name.Split(' ')[1];
    }

    float GetIncreasedHealthUsingStageNumber(float health)
    {
        return Mathf.FloorToInt(health * (1 + float.Parse(GetStageNumber()) / 100));
    }

    void InitEnemyTypes()
    {
        foreach (EnemyType enemyType in stage.enemyTypes)
        {
            enemyType.Init();
        }
    }

    void Spawn()
    {
        EnemyType[] enemyTypes = stage.enemyTypes;

        if (enemyTypes.Length == 0)
            return;

        foreach (EnemyType enemyType in enemyTypes)
        {
            if (!enemyType.ReadyToSpawn())
                continue;
            GameObject newEnemy = poolObject.GetPoolObject(enemyType.Enemy);
            newEnemy.transform.position = GetRandomSpawnPosition();

            UpdateUnitStatsUsingStageNumber(newEnemy);
            enemyType.DecreaseCount();
            enemyType.SetRandomNextEnemySpawnTime();
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
        Vector2 randomPos = (Vector2)spawnPosition.position + Random.insideUnitCircle * radius;

        return randomPos;
    }
}
