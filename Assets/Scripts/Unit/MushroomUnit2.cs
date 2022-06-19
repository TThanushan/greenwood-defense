using System.Collections;
using UnityEngine;

public class MushroomUnit2 : UnitAoeAttack
{
    [Header("Poison")]
    public int dotCount = 4;
    public float timeBetweenDotDamage = 1f;
    public GameObject poisonParticlesEffect;
    public Color poisonedColor;

    protected override void DamageEnemiesAroundTarget()
    {
        CreatePoisonEffect();
        GameObject[] enemies = GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            if (!IsTargetEnabled(enemy))
                continue;
            float distance = Vector2.Distance(Target.transform.position, enemy.transform.position);
            if (distance <= effectRange)
                ApplyEffect(enemy);
        }
    }
    void CreatePoisonEffect()
    {
        if (poisonParticlesEffect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(poisonParticlesEffect);
            newEffect.transform.position = transform.position;
        }
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
        Unit unit = target.GetComponent<Unit>();
        for (int i = 0; i < dotCount; i++)
        {

            if (!IsTargetEnabled(target))
                break;
            target.GetComponent<Unit>().GetDamage(effectDamage, transform);
            yield return new WaitForSeconds(timeBetweenDotDamage);
        }
        ChangeTargetSpriteColor(target, false);
    }

    void ChangeTargetSpriteColor(GameObject target, bool poisonOn)
    {
        if (poisonOn)
            target.transform.Find("SpriteBody/Sprite/UnitSprite").GetComponent<SpriteRenderer>().color = poisonedColor;
        else
            target.GetComponent<Unit>().ResetSpriteColor();
    }
}
