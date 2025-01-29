using UnityEngine;

[CreateAssetMenu(menuName = "Create new Stage")]
public class Stage : ScriptableObject
{
    public EnemyType[] enemyTypes;


    [System.Serializable]
    public class EnemyType
    {
        public GameObject Enemy;
        public float timeBetweenSpawn;
        public float randomTimeBetweenSpawn;

        public float timeBeforeFirstSpawn;
        public bool infiniteSpawning;
        public int enemyCount;

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
            this.timeBetweenSpawn = timeBetweenSpawn;
            this.randomTimeBetweenSpawn = randomTimeBetweenSpawn;
            this.timeBeforeFirstSpawn = timeBeforeFirstSpawn;
            this.infiniteSpawning = infiniteSpawning;
            this.enemyCount = enemyCount;
        }

        public void Init()
        {
            nextEnemySpawnTime = timeBeforeFirstSpawn;
            currentEnemyCount = enemyCount;
        }
        public bool ReadyToSpawn()
        {
            return Time.time >= nextEnemySpawnTime && (infiniteSpawning || currentEnemyCount > 0);
        }

        public void DecreaseCount()
        {
            if (currentEnemyCount > 0)
                currentEnemyCount--;
        }

        public void SetRandomNextEnemySpawnTime()
        {
            float randomSpawnTime = Random.Range(0, randomTimeBetweenSpawn);
            nextEnemySpawnTime = Time.time + timeBetweenSpawn + randomSpawnTime;
        }
    }
}
