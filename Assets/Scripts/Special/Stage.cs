using UnityEngine;

[CreateAssetMenu(menuName = "Create new Stage")]
public class Stage : ScriptableObject
{
    public EnemyType[] enemyTypes;


    [System.Serializable]
    public class EnemyType
    {
        public GameObject Enemy;
        public float TimeBeforeFirstSpawn;
        public float TimeBetweenSpawn;
        public float RandomTimeBetweenSpawn;
        public int EnemyCount;
        public bool InfiniteSpawning;

        [HideInInspector]
        public int currentEnemyCount;
        [HideInInspector]
        public float nextEnemySpawnTime = 0f;

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
