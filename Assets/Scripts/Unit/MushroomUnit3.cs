using System.Collections;
using UnityEngine;

public class MushroomUnit3 : MushroomUnit2
{

    [Header("Reduced Damage Effect")]
    [Range(0.0f, 100.0f)]
    public float damageReductionPercentage;

    protected override void ApplyEffect(GameObject target)
    {
        base.ApplyEffect(target);
        StartCoroutine(ReduceDamage(target));

    }

    IEnumerator ReduceDamage(GameObject target)
    {
        if (!IsTargetEnabled(target))
            yield return null;
        Unit unit = target.GetComponent<Unit>();
        float initialDamage = unit.attackDamage;
        unit.attackDamage *= 1 - (damageReductionPercentage / 100f);
        yield return new WaitForSeconds(GetPoisonDuration());
        unit.attackDamage = initialDamage;
    }

    protected float GetPoisonDuration()
    {
        return timeBetweenDotDamage * dotCount;
    }
}
