using System.Collections;
using UnityEngine;

public class PoisonFrogUnit : Unit
{
    [Header("Poison")]
    public float poisonDamage;
    public int dotCount = 4;
    public float timeBetweenDotDamage = 1f;
    public Color poisonedColor;

    public override void Attack()
    {
        base.Attack();
        ApplyEffect(Target);

    }

    protected virtual void ApplyEffect(GameObject target)
    {
        if (!IsTargetEnabled(target))
            return;
        StartCoroutine(PoisonDamageTarget(target));
    }

    IEnumerator PoisonDamageTarget(GameObject target)
    {
        if (!IsTargetEnabled(target))
            yield return null;
        ChangeTargetSpriteColor(target, true);
        for (int i = 0; i < dotCount; i++)
        {
            if (!IsTargetEnabled(target))
                break;
            target.GetComponent<Unit>().GetDamage(poisonDamage, transform);
            yield return new WaitForSeconds(timeBetweenDotDamage);
        }
        ChangeTargetSpriteColor(target, false);
    }

    void ChangeTargetSpriteColor(GameObject target, bool poisonOn)
    {
        if (poisonOn)
        {

            target.GetComponent<Unit>().ChangeSpriteColor(poisonedColor);
        }
        else
            target.GetComponent<Unit>().ResetSpriteColor();
    }

}
