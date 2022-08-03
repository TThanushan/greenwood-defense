using UnityEngine;

public class PigUnit3 : PigUnit2
{
    [Header("ReduceUnitSpawnCooldown")]
    [Range(0, 100)]
    public float reducedPercentage;
    SpawnBar spawnBar;

    bool alreadyReducedCooldown;

    protected override void Start()
    {
        base.Start();
        spawnBar = SpawnBar.instance;
        OnDeath += DisableAlreadyReducedCooldown;

    }

    protected override void Update()
    {
        if (Disabled)
            return;
        base.Update();
        ReduceUnitSpawnMaxCooldown();
    }

    void DisableAlreadyReducedCooldown()
    {
        alreadyReducedCooldown = false;
        spawnBar.IncreaseMaxCooldown(reducedPercentage);
    }

    //public override void Attack()
    //{
    //    base.Attack();
    //    ReduceUnitSpawnMaxCooldown();
    //}

    //protected override void OnEnable()
    //{
    //    base.OnEnable();
    //    if (spawnBar)
    //        spawnBar.ReduceMaxCooldown(reducedPercentage);
    //}

    void ReduceUnitSpawnMaxCooldown()
    {
        if (!alreadyReducedCooldown)
        {
            spawnBar.ReduceMaxCooldown(reducedPercentage);
            alreadyReducedCooldown = true;
        }
    }
}