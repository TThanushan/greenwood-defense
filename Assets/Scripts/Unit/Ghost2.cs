using System.Collections;
using UnityEngine;

public class Ghost2 : Ghost1
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
        //ResetInvulnerabilityCooldown();
        invulnerabilityBonusEffect = transform.Find("SpriteBody/invulnerabilityEffect").gameObject;

    }
    void ResetInvulnerabilityCooldown()
    {
        invulnerabilityBonusCooldown = Time.time + invulnerabilityBonusDuration + invulnerabilityBonusRespawnTime;

    }
    protected override void Update()
    {
        base.Update();

        if (invulnerabilityBonusCooldown <= Time.time && ProjectileAffectMe())
            ResetInvulnerabilityBonus();
    }

    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        if (isInvulnerabilityBonusEnabled)
            return;
        base.GetDamage(damage, caller, HitSoundName);
    }
    protected virtual void ResetInvulnerabilityBonus()
    {
        isInvulnerabilityBonusEnabled = true;
        EnableInvulnerabilityBonusEffect(true);
        StartCoroutine(DisableInvulnerabilityBonus());
    }
    IEnumerator DisableInvulnerabilityBonus()
    {
        ResetInvulnerabilityCooldown();
        yield return new WaitForSeconds(invulnerabilityBonusDuration);
        if (Disabled)
            yield return null;
        isInvulnerabilityBonusEnabled = false;

        EnableInvulnerabilityBonusEffect(false);
    }
    void EnableInvulnerabilityBonusEffect(bool val)
    {
        invulnerabilityBonusEffect.SetActive(val);
    }
}
