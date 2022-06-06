using UnityEngine;

public class BunnyUnit2 : Unit
{
    [Header("Spawn Effect")]
    public float timeBetweenSpawn;
    public GameObject[] bunnies;
    public GameObject spawnEffect;
    public int spawnNumber = 1;

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
            int index = Random.Range(0, bunnies.Length);
            GameObject newBunny = poolObject.GetPoolObject(bunnies[index]);
            Vector3 spawnPosition = transform.position;
            spawnPosition.x += Random.Range(-0.08f, 0.08f);
            spawnPosition.y += Random.Range(-0.08f, 0.08f);

            newBunny.transform.position = spawnPosition;
        }
        CreateEffect();
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
