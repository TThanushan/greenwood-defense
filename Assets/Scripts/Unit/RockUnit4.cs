using System.Collections;
using UnityEngine;

public class RockUnit4 : RockUnit3
{
    [Header("invulnerability")]
    public float invulnerabilityBonusDuration;
    public float invulnerabilityBonusRespawnTime;
    protected bool isInvulnerabilityBonusEnabled;
    //bool isDefenseBonusEnabledOnce;
    float invulnerabilityBonusCooldown;
    GameObject invulnerabilityBonusEffect;

    protected override void Awake()
    {
        base.Awake();
        invulnerabilityBonusEffect = transform.Find("SpriteBody/invulnerabilityEffect").gameObject;

    }


    void ResetInvulnerabilityCooldown()
    {
        invulnerabilityBonusCooldown = Time.time + invulnerabilityBonusDuration + invulnerabilityBonusRespawnTime;

    }
    protected override void Update()
    {
        base.Update();

        if (invulnerabilityBonusCooldown <= Time.time)
            ResetDefenseBonus();
    }

    public override void GetDamage(float damage, Transform caller = null)
    {
        if (isInvulnerabilityBonusEnabled)
            return;
        base.GetDamage(damage, caller);
    }
    protected override void ResetDefenseBonus()
    {
        base.ResetDefenseBonus();
        isInvulnerabilityBonusEnabled = true;
        EnableDefenseBonusEffect(true);
        StartCoroutine(DisableInvulnerabilityBonus());
    }
    IEnumerator DisableInvulnerabilityBonus()
    {
        ResetInvulnerabilityCooldown();
        yield return new WaitForSeconds(invulnerabilityBonusDuration);
        if (Disabled)
            yield return null;
        isInvulnerabilityBonusEnabled = false;

        EnableDefenseBonusEffect(false);
    }
    void EnableDefenseBonusEffect(bool val)
    {
        invulnerabilityBonusEffect.SetActive(val);
    }



}
