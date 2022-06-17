using System.Collections;
using UnityEngine;

public class PoisonFrog : Unit
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
                yield return null;
            target.GetComponent<Unit>().GetDamage(poisonDamage, transform);
            yield return new WaitForSeconds(timeBetweenDotDamage);
        }
        ChangeTargetSpriteColor(target, false);
    }

    void ChangeTargetSpriteColor(GameObject target, bool poisonOn)
    {
        Color color = Color.white;
        if (poisonOn)
            color = poisonedColor;

        GetTargetSpriteRenderer(target).color = color;
    }

    SpriteRenderer GetTargetSpriteRenderer(GameObject target)
    {
        Transform t = target.transform.Find("SpriteBody/Sprite/UnitSprite");
        if (t is null)
            t = target.transform.Find("SpriteBody/Sprite");
        return t.GetComponent<SpriteRenderer>();
    }

}
