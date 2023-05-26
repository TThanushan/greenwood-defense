using UnityEngine;

public class Chameleon1234 : Unit
{
    public float cycleMax;

    [Header("AttackSpeedReductionOnHit")]
    [Range(0f, 100f)]
    public float attackSpeedReduction = 40f;
    public float effectDuration = 2f;
    public float cycleDuration0;
    protected int cycleIndex;

    [Header("RageEffect")]
    public float attackSpeedBuff = 40f;
    public float attackDamageBuff = 40f;
    public float cycleDuration1;

    [Header("LifeSteal")]
    public float lifeStealPercentage = 40f;
    public float cycleDuration2;
    float nextCycleTime;
    public override void Attack()
    {
        base.Attack();
        if (cycleIndex == 1 || (cycleMax == 3 && cycleIndex == 2))
            SlowTargetAttackSpeed();
        if (cycleIndex == 3 || (cycleMax == 3 && cycleIndex == 1))
            LifeSteal(attackDamage);
    }

    protected override void Update()
    {
        base.Update();
        if (nextCycleTime <= Time.time)
            RunCycle();
    }
    protected virtual void RunCycle()
    {

        if (cycleIndex >= cycleMax)
            cycleIndex = 0;
        float cycleTime = cycleDuration0;

        if (cycleIndex == 0 || cycleIndex == 1)
        {
            if (cycleIndex == 0)
                GetUnitSpriteRenderer().color = Color.white;
            if (cycleIndex != 1)
                DisableRageEffect();
        }

        if (cycleIndex == 1 && cycleMax >= 1)
        {
            if (cycleIndex == 1)
                GetUnitSpriteRenderer().color = Color.red;
            EnableRageEffect();
            cycleTime = cycleDuration1;
        }
        if (cycleIndex == 2 && cycleMax >= 2)
        {
            if (cycleIndex == 2)
                GetUnitSpriteRenderer().color = Color.yellow;

            if (cycleMax == 3)
            {
                EnableRageEffect();

            }
            else
                DisableRageEffect();

            cycleTime = cycleDuration2;
        }

        cycleIndex++;

        nextCycleTime = Time.time + cycleTime;
    }


    void EnableRageEffect()
    {
        if (attackDamage != initialAttackDamage)
            return;
        nextAttackTime = 0f;
        attackSpeed *= 1 - (attackSpeedBuff / 100);
        attackDamage *= 1 + (attackDamageBuff / 100);
    }

    void DisableRageEffect()
    {
        InvokeResetAttackDamage(0.01f);
        InvokeResetAttackSpeed(0.01f);

    }

    void LifeSteal(float damageDone)
    {
        currentHealth += damageDone * lifeStealPercentage / 100;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    void SlowTargetAttackSpeed()
    {
        Unit unit = Target.GetComponent<Unit>();
        if (unit.attackSpeed == unit.GetInitialAttackSpeed())
            unit.attackSpeed *= 1 + (attackSpeedReduction / 100f);
        unit.InvokeResetAttackSpeed(effectDuration);
    }

}
