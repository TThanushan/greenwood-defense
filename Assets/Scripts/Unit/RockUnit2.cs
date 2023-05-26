using System.Collections;
using UnityEngine;

public class RockUnit2 : Unit
{
    public float damageReductionPercentage;
    public float defenseBonusDuration;
    public float defenseBonusRespawnTime;
    protected bool isDefenseBonusEnabled;
    public Color defenseColor;
    float defenseBonusCooldown;
    GameObject defenseBonusEffect;

    protected override void Awake()
    {
        base.Awake();
        defenseBonusEffect = transform.Find("SpriteBody/Sprite").gameObject;
    }


    protected override void Update()
    {
        base.Update();
        if (defenseBonusCooldown <= Time.time)
            ResetDefenseBonus();
    }

    protected virtual void ResetDefenseBonus()
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
        defenseBonusCooldown = Time.time + defenseBonusDuration + defenseBonusRespawnTime;
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
        //col = new Color(0, 0.41f, 1);
        defenseBonusEffect.gameObject.GetComponent<SpriteRenderer>().color = col;
    }
}
