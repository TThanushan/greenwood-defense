using System.Collections;
using UnityEngine;

public class GhostUnit2 : GhostUnit1
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
        StartCoroutine(DisableInvulnerabilityBonusIE());
    }
    IEnumerator DisableInvulnerabilityBonusIE()
    {
        ResetInvulnerabilityCooldown();
        yield return new WaitForSeconds(invulnerabilityBonusDuration);
        if (Disabled)
            yield return null;
        DisableInvulnerabilityBonus();
    }

    void DisableInvulnerabilityBonus()
    {
        isInvulnerabilityBonusEnabled = false;

        EnableInvulnerabilityBonusEffect(false);
    }
    void EnableInvulnerabilityBonusEffect(bool val)
    {
        invulnerabilityBonusEffect.SetActive(val);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        DisableInvulnerabilityBonus();
    }
}
