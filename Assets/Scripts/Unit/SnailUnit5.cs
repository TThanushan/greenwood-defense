using UnityEngine;

public class SnailUnit5 : SnailUnit4
{
    [Range(0, 100)]
    public float healthTriggerPercentage;
    //public float globalHealPercentage;
    public float timeBetweenlowLifeGlobalHeal;
    public GameObject globalHealEffect;
    public float globalHealPercentage = 100f;
    float lowLifeGlobalHealCooldown;

    protected override void Update()
    {
        if (IsInRangeWithAlly() && IsAnyAllyLowLife() && lowLifeGlobalHealCooldown <= Time.time)
            StartLowLifeGlobalHeal();
        base.Update();

    }

    bool IsAnyAllyLowLife()
    {
        GameObject[] allies = poolObject.Allies;
        if (allies == null || allies.Length == 0)
            return false;
        foreach (GameObject ally in allies)
        {
            if (ally.GetComponent<Unit>().Disabled || ally.name.Contains("ReallySmallSlime") || ally.name.Contains("WeakLittleBunny"))
                continue;
            Unit unit = ally.GetComponent<Unit>();
            float percentage = unit.currentHealth / unit.maxHealth * 100;
            if (percentage <= healthTriggerPercentage)
                return true;
        }
        return false;
    }

    void StartLowLifeGlobalHeal()
    {
        GameObject[] allies = poolObject.Allies;
        if (allies == null || allies.Length == 0)
            return;

        foreach (GameObject ally in allies)
        {
            float distance = Vector2.Distance(transform.position, ally.transform.position);
            if (distance <= bonusRange)
            {
                HealAlly(ally, globalHealPercentage);
            }
        }
        CreateEffect();
        lowLifeGlobalHealCooldown = Time.time + timeBetweenlowLifeGlobalHeal;
    }

    bool IsInRangeWithAlly()
    {
        GameObject[] allies = poolObject.Allies;
        if (allies == null || allies.Length == 0)
            return false;
        foreach (GameObject ally in allies)
        {
            float distance = Vector2.Distance(transform.position, ally.transform.position);
            if (distance <= bonusRange)
            {
                return true;
            }
        }
        return false;
    }

    void CreateEffect()
    {
        if (!globalHealEffect)
            return;
        GameObject newEgg = PoolObject.instance.GetPoolObject(globalHealEffect);
        newEgg.transform.position = transform.position;
    }
}