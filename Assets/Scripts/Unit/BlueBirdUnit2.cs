using UnityEngine;

public class BlueBirdUnit2 : Unit
{
    [Header("Egg")]
    public float eggExplosionDamage;
    public float birdSpawnReloadTime;
    public GameObject birdEffectPrefab;
    public Transform spawnPos;

    protected float birdSpawnCooldown;

    protected override void Update()
    {
        base.Update();
        if (EnoughRangeToAttackTarget() && birdSpawnCooldown <= Time.time)
            DoEffect();
    }


    protected virtual void DoEffect()
    {
        DropEgg();

    }

    protected virtual void DropEgg()
    {
        SummonEffectBird();
    }

    protected virtual void SummonEffectBird()
    {
        if (!birdEffectPrefab)
            return;
        GameObject newBird = poolObject.GetPoolObject(birdEffectPrefab);
        newBird.transform.position = spawnPos.transform.position;
        newBird.GetComponent<BlueBirdUnit2Effect>().SetStats(eggExplosionDamage);
        birdSpawnCooldown = Time.time + birdSpawnReloadTime;
    }
}
