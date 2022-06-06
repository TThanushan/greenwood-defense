using System.Collections;
using UnityEngine;

public class RockUnit2 : Unit
{
    public float damageReductionPercentage;
    public float defenseBonusDuration;
    public float defenseBonusRespawnTime;
    protected bool isDefenseBonusEnabled;
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

    void ResetDefenseBonus()
    {
        isDefenseBonusEnabled = true;
        EnableDefenseBonusEffect(true);
        StartCoroutine(DisableDefenseBonus());
    }


    public override void GetDamage(float damage, Transform caller = null)
    {
        if (isDefenseBonusEnabled)
            damage *= 1 - damageReductionPercentage / 100;
        base.GetDamage(damage);
    }

    IEnumerator DisableDefenseBonus()
    {
        defenseBonusCooldown = Time.time + defenseBonusDuration + defenseBonusRespawnTime;
        yield return new WaitForSeconds(defenseBonusDuration);
        if (Disabled)
            yield return null;
        isDefenseBonusEnabled = false;
        EnableDefenseBonusEffect(false);
    }
    void EnableDefenseBonusEffect(bool val)
    {
        Color col = Color.white;
        if (val)
            col = new Color(0, 0.41f, 1);
        defenseBonusEffect.gameObject.GetComponent<SpriteRenderer>().color = col;
    }
}
