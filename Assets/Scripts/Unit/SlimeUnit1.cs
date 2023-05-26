using UnityEngine;

public class SlimeUnit1 : Unit
{
    [Header("ChanceToSpawnSlimeWhenHit")]
    [Range(0, 100)]
    public float spawnChance;
    public GameObject reallySmallSlime;
    public GameObject spawnEffect;

    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        base.GetDamage(damage, caller, HitSoundName);
        if (!caller.GetComponent<Trap>() && !IsCallerPoisoning(caller) && IsSpawn())
        {
            SpawnSlime(reallySmallSlime);
            CreateEffect(spawnEffect);
        }
    }

    bool IsSpawn()
    {
        return Random.Range(0, 100) < spawnChance;
    }

    protected void SpawnSlime(GameObject slime)
    {

        if (!slime)
            return;
        GameObject newUnit = PoolObject.instance.GetPoolObject(slime);
        newUnit.transform.position = GetRandomSpawnPosition(transform.position);
        newUnit.GetComponent<Unit>().SetTargetTag(targetTag);
    }

    Vector2 GetRandomSpawnPosition(Vector2 spawnPosition)
    {
        float radius = 0.15f;
        Vector2 randomPos = spawnPosition + (Random.insideUnitCircle * radius);

        return randomPos;
    }


}
