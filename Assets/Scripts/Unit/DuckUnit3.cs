using System.Collections;
using UnityEngine;

public class DuckUnit3 : HitBasedUnit
{
    [Header("Big Shield")]
    public float bigShieldMax;
    protected float bigShieldcurrent;

    public float bigShieldRespawnTime;
    float bigShieldCooldown;

    public float defenseBonusDamageReduction;
    public float defenseBonusDuration;
    public GameObject shieldEffect;

    GameObject defenseBonusEffect;
    protected bool isDefenseBonusEnabled;
    bool isDefenseBonusEnabledOnce;

    protected override void Awake()
    {
        base.Awake();
        defenseBonusEffect = transform.Find("SpriteBody/Sprite").gameObject;
        shieldEffect = transform.Find("SpriteBody/Shield").gameObject;
    }

    protected override void Update()
    {
        base.Update();
        if (bigShieldCooldown <= Time.time)
            ResetBigShield();

        if (bigShieldcurrent > 0)
            bigShieldCooldown = Time.time + bigShieldRespawnTime;

    }

    void ResetBigShield()
    {
        bigShieldcurrent = bigShieldMax;
        shieldEffect.SetActive(true);
        isDefenseBonusEnabledOnce = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetBigShield();
    }

    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        if (bigShieldcurrent > 0)
        {
            bigShieldcurrent--;
            audioManager.Play(Constants.SHIELD_HIT_SFX);
            if (bigShieldcurrent <= 0)
            {
                bigShieldcurrent = 0;
            }
            return;
        }
        if (bigShieldcurrent <= 0)
        {
            if (isDefenseBonusEnabledOnce)
            {
                shieldEffect.SetActive(false);
                isDefenseBonusEnabled = true;
                isDefenseBonusEnabledOnce = false;
                EnableDefenseBonusEffect(true);
                StartCoroutine(DisableDefenseBonus());
            }
        }

        if (isDefenseBonusEnabled)
            damage *= 1 - defenseBonusDamageReduction / 100;
        base.GetDamage(damage, caller, HitSoundName);
    }

    IEnumerator DisableDefenseBonus()
    {
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