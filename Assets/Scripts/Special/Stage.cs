using UnityEngine;

[CreateAssetMenu(menuName = "Create new Stage")]
public class Stage : ScriptableObject
{
    public EnemyType[] enemyTypes;


    [System.Serializable]
    public class EnemyType
    {
        public GameObject Enemy;
        public float TimeBetweenSpawn;
        public float RandomTimeBetweenSpawn;

        public float TimeBeforeFirstSpawn;
        public bool InfiniteSpawning;
        public int EnemyCount;

        [HideInInspector]
        public int currentEnemyCount;
        [HideInInspector]
        public float nextEnemySpawnTime = 0f;

        public EnemyType()
        {

        }

        public EnemyType(GameObject enemy, float timeBetweenSpawn, float randomTimeBetweenSpawn, float timeBeforeFirstSpawn, bool infiniteSpawning, int enemyCount)
        {
            Enemy = enemy;
            TimeBetweenSpawn = timeBetweenSpawn;
            RandomTimeBetweenSpawn = randomTimeBetweenSpawn;
            TimeBeforeFirstSpawn = timeBeforeFirstSpawn;
            InfiniteSpawning = infiniteSpawning;
            EnemyCount = enemyCount;
        }

        public void Init()
        {
            nextEnemySpawnTime = TimeBeforeFirstSpawn;
            currentEnemyCount = EnemyCount;
        }
        public bool ReadyToSpawn()
        {
            return Time.time >= nextEnemySpawnTime && (InfiniteSpawning || currentEnemyCount > 0);
        }

        public void DecreaseCount()
        {
            if (currentEnemyCount > 0)
                currentEnemyCount--;
        }

        public void SetRandomNextEnemySpawnTime()
        {
            float randomSpawnTime = Random.Range(0, RandomTimeBetweenSpawn);
            nextEnemySpawnTime = Time.time + TimeBetweenSpawn + randomSpawnTime;
        }
    }
}
