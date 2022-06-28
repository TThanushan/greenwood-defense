using UnityEngine;

public class SnailUnit4 : SnailUnit3
{
    //[Header("Slime Spit")]
    //public float spitDamage;
    //public float spitDamageReloadTime;
    [Header("Constant Healing")]
    public float constantHealingPercentage;
    public float timeBetweenConstantHealing;
    float constantHealingCooldown;

    protected override void Update()
    {
        base.Update();
        if (constantHealingCooldown <= Time.time)
            HealAllies();
    }

    void HealAllies()
    {
        GameObject[] allies = poolObject.Allies;
        if (allies == null || allies.Length == 0)
            return;

        foreach (GameObject ally in allies)
        {
            float distance = Vector2.Distance(transform.position, ally.transform.position);
            if (distance <= bonusRange)
            {
                HealAlly(ally, constantHealingPercentage);
            }
        }

        constantHealingCooldown = Time.time + timeBetweenConstantHealing;
    }

}
