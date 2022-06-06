using System.Collections;
using UnityEngine;

public class MushroomUnit5 : MushroomUnit4
{
    [Header("Paralyse Effect")]
    [Range(0f, 5f)]
    public float timeBeforeParalyse;
    [Range(0f, 5f)]
    public float paralyseDuration;


    protected override void ApplyEffect(GameObject target)
    {
        base.ApplyEffect(target);
        StartCoroutine(IncreaseDamageTaken(target));
    }
    IEnumerator IncreaseDamageTaken(GameObject target)
    {
        if (!IsTargetEnabled(target))
            yield return null;
        yield return new WaitForSeconds(timeBeforeParalyse);
        Unit unit = target.GetComponent<Unit>();
        unit.ParalyseEffect(true);
        yield return new WaitForSeconds(paralyseDuration);
        unit.ParalyseEffect(false);
    }
}
