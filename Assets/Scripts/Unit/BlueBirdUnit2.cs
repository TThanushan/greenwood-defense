using UnityEngine;

public class BlueBirdUnit2 : Unit
{
    [Header("Egg")]
    public float eggExplosionDamage;
    public GameObject eggPrefab;
    public float eggReloadTime;
    public Transform eggSpawnPos;

    float eggCooldown;

    protected override void Update()
    {
        base.Update();
        DoEffect();
    }


    protected virtual void DoEffect()
    {
        if (EnoughRangeToAttackTarget())
            DropEgg();

    }

    protected GameObject DropEgg()
    {
        if (eggCooldown > Time.time)
            return null;
        return InstantiateEgg();
    }


    protected GameObject InstantiateEgg()
    {
        if (!eggPrefab || !Target)
            return null;
        GameObject newEgg = poolObject.GetPoolObject(eggPrefab);
        newEgg.transform.position = eggSpawnPos.transform.position;

        if (newEgg.GetComponent<EggBomb>())
        {
            EggBomb newEggScript = newEgg.GetComponent<EggBomb>();
            newEggScript.SetTargetTag(targetTag);
            newEggScript.explosionDamage = eggExplosionDamage;
        }
        else if (newEgg.GetComponent<EggSpawner>())
        {
            newEgg.GetComponent<EggSpawner>().SetTargetTag(targetTag);
        }
        eggCooldown = eggReloadTime + Time.time;


        return newEgg;
    }
}
