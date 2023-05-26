using System.Collections;
using UnityEngine;

public class RhinocerosUnit4 : RhinocerosUnit3
{
    [Header("DefenseBonusAfterCharge")]
    public float damageReductionPercentage;
    public float defenseBonusDuration;
    //public float defenseBonusRespawnTime;
    protected bool isDefenseBonusEnabled;
    public Color defenseColor;

    //float defenseBonusCooldown;
    GameObject defenseBonusEffect;

    protected override void Awake()
    {
        base.Awake();
        OnChargeStart += ResetDefenseBonus;
        OnChargeEnd += ResetDefenseBonus;
        defenseBonusEffect = transform.Find("SpriteBody/Sprite").gameObject;

    }



    void ResetDefenseBonus()
    {
        isDefenseBonusEnabled = true;
        EnableDefenseBonusEffect(true);
        StartCoroutine(DisableDefenseBonus());
    }

    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        if (isDefenseBonusEnabled)
            damage *= 1 - (damageReductionPercentage / 100);
        base.GetDamage(damage, caller, HitSoundName);
    }

    IEnumerator DisableDefenseBonus()
    {
        yield return new WaitForSeconds(defenseBonusDuration);
        if (Disabled)
            yield return null;
        isDefenseBonusEnabled = false;
        DisableDefenseBonusEffect();
    }

    protected virtual void DisableDefenseBonusEffect()
    {
        EnableDefenseBonusEffect(false);
    }
    void EnableDefenseBonusEffect(bool val)
    {
        Color col = Color.white;
        if (val)
            col = defenseColor;
        defenseBonusEffect.gameObject.GetComponent<SpriteRenderer>().color = col;
    }
}
