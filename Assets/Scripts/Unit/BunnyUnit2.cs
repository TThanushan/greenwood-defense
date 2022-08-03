using UnityEngine;

public class BunnyUnit2 : Unit
{
    [Header("Spawn Effect")]
    public float timeBetweenSpawn;
    public GameObject[] bunnies;
    public GameObject spawnEffect;
    public int spawnNumber = 1;
    public bool spawnAll;

    protected override void Start()
    {
        base.Start();
        attackSpeed = timeBetweenSpawn;
    }
    public override void Attack()
    {
        SpawnBunnies();
    }

    void SpawnBunnies()
    {
        if (bunnies.Length <= 0 || !Target)
            return;
        for (int i = 0; i < spawnNumber; i++)
        {
            if (spawnAll)
            {
                for (int y = 0; y < bunnies.Length; y++)
                {
                    Spawn(bunnies[y]);
                }
            }
            else
            {
                int index = Random.Range(0, bunnies.Length);
                //GameObject newBunny = poolObject.GetPoolObject(bunnies[index]);
                //newBunny.transform.position = GetRandomPosition(transform.position, -0.08f, 0.08f, -0.08f, 0.08f);
                Spawn(bunnies[index]);

            }

        }
        CreateEffect();
    }

    protected void Spawn(GameObject prefab)
    {
        GameObject newBunny = poolObject.GetPoolObject(prefab);
        newBunny.transform.position = GetRandomPosition(transform.position, -0.08f, 0.08f, -0.08f, 0.08f);

    }

    Vector2 GetRandomPosition(Vector2 pos, float xRangeA, float xRangeB, float yRangeA, float yRangeB)
    {
        pos.x += Random.Range(xRangeA, xRangeB);
        pos.y += Random.Range(yRangeA, yRangeB);
        return pos;
    }

    void CreateEffect()
    {
        if (spawnEffect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(spawnEffect);
            newEffect.transform.position = transform.position;
        }
    }

}
