using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public SpawnBlock[] spawnBlocks;
    public float timeBetweenSpawn = 2f;
    public Transform spawnPosition;
    public float randomTimeBetweenSpawn;
    private float nextSpawnTime = 0f;
    PoolObject poolObject;
    // Start is called before the first frame update
    void Start()
    {
        poolObject = PoolObject.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextSpawnTime <= Time.time)
        {
            Spawn();
        }
    }

    string GetStageNumber()
    {
        return SceneManager.GetActiveScene().name.Split(' ')[1];
    }

    float GetIncreasedHealthUsingStageNumber(float health)
    {
        return Mathf.FloorToInt(health * (1 + float.Parse(GetStageNumber()) / 100));
    }
    void Spawn()
    {
        if (spawnBlocks.Length == 0)
            return;

        bool isSpawning = false;
        while (!isSpawning)
        {
            int index = Random.Range(0, spawnBlocks.Length);
            SpawnBlock spawnBlock = spawnBlocks[index];
            isSpawning = IsSpawning(spawnBlock);
            if (!isSpawning)
                continue;
            GameObject newEnemy = poolObject.GetPoolObject(spawnBlock.enemyPrefab);
            Unit unit = newEnemy.GetComponent<Unit>();
            unit.maxHealth = GetIncreasedHealthUsingStageNumber(unit.maxHealth);
            unit.currentHealth = GetIncreasedHealthUsingStageNumber(unit.currentHealth);
            newEnemy.transform.position = GetRandomSpawnPosition();

            nextSpawnTime = timeBetweenSpawn + Time.time;
            nextSpawnTime += Random.Range(0, randomTimeBetweenSpawn);

        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        float radius = 0.15f;
        Vector2 randomPos = (Vector2)spawnPosition.position + Random.insideUnitCircle * radius;

        return randomPos;
    }

    bool IsSpawning(SpawnBlock spawnBlock)
    {
        return Random.Range(0, 10) <= spawnBlock.chanceToSpawn;

    }

    [System.Serializable]
    public class SpawnBlock
    {
        public GameObject enemyPrefab;
        // A random int to 0 to 10 will be generated, if smaller than chanceToSpawn,
        // enemy will be spawned, loop will restard to select a random prefab.
        [Range(0, 10)]
        public float chanceToSpawn;
    }

}
