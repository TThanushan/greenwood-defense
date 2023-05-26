using System.Collections;
using UnityEngine;

public class MushroomUnit4 : MushroomUnit3
{
    [Header("Damage Taken Effect")]
    [Range(0.0f, 100.0f)]
    public float damageTakenIncreasePercentageEffect;

    [Range(0f, 1f)]
    public float maxHealthPercentageDamage;

    protected override void ApplyEffect(GameObject target)
    {
        base.ApplyEffect(target);
        StartCoroutine(IncreaseDamageTaken(target));
        StartCoroutine(PoisonDamageMaxHealthPercentage(target));
    }

    IEnumerator IncreaseDamageTaken(GameObject target)
    {
        if (!IsTargetEnabled(target))
            yield return null;
        Unit unit = target.GetComponent<Unit>();
        unit.damageTakenIncreasePercentage = damageTakenIncreasePercentageEffect;
        unit.InvokeResetDamageTakenIncreasePercentage(GetPoisonDuration());
    }

    IEnumerator PoisonDamageMaxHealthPercentage(GameObject target)
    {
        if (!IsTargetEnabled(target))
            yield return null;
        for (int i = 0; i < dotCount; i++)
        {

            if (!IsTargetEnabled(target))
                break;
            Unit unit = target.GetComponent<Unit>();
            float damage = (unit.maxHealth * (maxHealthPercentageDamage / 100)) + effectDamage;
            unit.GetDamage(damage, transform);
            yield return new WaitForSeconds(timeBetweenDotDamage);

        }
    }
}
