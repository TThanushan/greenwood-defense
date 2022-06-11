using UnityEngine;

public class EggSpawner : MonoBehaviour
{
    public float timeBeforeExplosion;
    public GameObject explosionEffect;
    public GameObject birdPrefab;

    public string targetTag = "Enemy";
    public int spawnNumber;

    float currentTimeBeforeExplosion;
    private void Update()
    {
        if (currentTimeBeforeExplosion <= Time.time)
            SpawnBirds();

    }
    private void OnEnable()
    {
        currentTimeBeforeExplosion = Time.time + timeBeforeExplosion;

    }


    Vector2 GetRandomSpawnPosition(Vector2 spawnPosition)
    {
        float radius = 0.1f;
        Vector2 randomPos = spawnPosition + Random.insideUnitCircle * radius;

        return randomPos;
    }

    void SpawnBirds()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            SpawnBird();
            print("bip3");
        }
        CreateEffect();
        gameObject.SetActive(false);
    }

    void SpawnBird()
    {
        print("bip1");
        if (!birdPrefab)
            return;
        print("bip2");
        GameObject newBirdPrefab = PoolObject.instance.GetPoolObject(birdPrefab);
        newBirdPrefab.transform.position = GetRandomSpawnPosition(transform.position);
        newBirdPrefab.GetComponent<Unit>().SetTargetTag(targetTag);
    }


    void CreateEffect()
    {
        if (!explosionEffect)
            return;
        GameObject newEgg = PoolObject.instance.GetPoolObject(explosionEffect);
        newEgg.transform.position = transform.position;
    }
    public void SetTargetTag(string tag)
    {
        targetTag = tag;
    }


}
