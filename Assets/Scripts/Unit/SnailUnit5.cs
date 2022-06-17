using UnityEngine;

public class SnailUnit5 : SnailUnit4
{
    [Range(0, 100)]
    public float healthTriggerPercentage;
    //public float globalHealPercentage;
    public float timeBetweenlowLifeGlobalHeal;

    float lowLifeGlobalHealCooldown;

    protected override void Update()
    {
        base.Update();
        if (IsAnyAllyLowLife() && lowLifeGlobalHealCooldown <= Time.time)
            StartLowLifeGlobalHeal();

    }

    bool IsAnyAllyLowLife()
    {
        GameObject[] allies = poolObject.Allies;
        if (allies == null || allies.Length == 0)
            return false;
        foreach (GameObject ally in allies)
        {
            Unit unit = ally.GetComponent<Unit>();
            float percentage = unit.currentHealth / unit.maxHealth * 100;
            if (percentage <= healthTriggerPercentage)
                return true;
        }
        return false;
    }

    void StartLowLifeGlobalHeal()
    {
        print("start global heal");
        GameObject[] allies = poolObject.Allies;
        if (allies == null || allies.Length == 0)
            return;

        foreach (GameObject ally in allies)
        {
            float distance = Vector2.Distance(transform.position, ally.transform.position);
            if (distance <= bonusRange)
            {
                for (int i = 0; i < 8; i++)
                {
                    HealAlly(ally);

                }
            }
        }
        lowLifeGlobalHealCooldown = Time.time + timeBetweenlowLifeGlobalHeal;
    }
}