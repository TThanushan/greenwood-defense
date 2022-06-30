using UnityEngine;

public class FatFrog : Unit
{
    public int spawnNumber;
    public GameObject frogPrefab;
    public GameObject spawnEffect;

    public override void Disable()
    {
        base.Disable();
        SpawnFrogs();
    }


    void SpawnFrogs()
    {
        CreateEffect();
        for (int i = 0; i < spawnNumber; i++)
        {
            SpawnFrog();
        }

        //ShakeCamera.instance.Shake(0.025f, 0.25f);
    }
    void SpawnFrog()
    {

        if (!frogPrefab)
            return;
        GameObject newBirdPrefab = PoolObject.instance.GetPoolObject(frogPrefab);
        newBirdPrefab.transform.position = GetRandomSpawnPosition(transform.position);
        newBirdPrefab.GetComponent<Unit>().SetTargetTag(targetTag);
    }

    Vector2 GetRandomSpawnPosition(Vector2 spawnPosition)
    {
        float radius = 0.15f;
        Vector2 randomPos = spawnPosition + Random.insideUnitCircle * radius;

        return randomPos;
    }

    void CreateEffect()
    {
        if (!spawnEffect)
            return;
        GameObject newEgg = PoolObject.instance.GetPoolObject(spawnEffect);
        newEgg.transform.position = transform.position;
    }
}
