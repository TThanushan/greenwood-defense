using UnityEngine;

public class FatFrogUnit : Unit
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
        CreateEffect(spawnEffect);
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
        GameObject newFrog = PoolObject.instance.GetPoolObject(frogPrefab);
        newFrog.transform.position = GetRandomSpawnPosition(transform.position);
        newFrog.GetComponent<Unit>().SetTargetTag(targetTag);
    }

    Vector2 GetRandomSpawnPosition(Vector2 spawnPosition)
    {
        float radius = 0.15f;
        Vector2 randomPos = spawnPosition + Random.insideUnitCircle * radius;

        return randomPos;
    }


}
