using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemies;
    public float timeBetweenSpawn = 2f;
    public Transform spawnPosition;
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

    void Spawn()
    {
        if (enemies.Length == 0)
            return;
        int index = Random.Range(0, enemies.Length - 1);
        GameObject newEnemy = poolObject.GetPoolObject(enemies[index]);
        newEnemy.transform.position = spawnPosition.position;
        nextSpawnTime = timeBetweenSpawn + Time.time;

    }
}
