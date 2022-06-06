using UnityEngine;

public class ChickenUnit4 : ChickenUnit3
{

    [Header("Rage Effect")]
    public float healthThreshold;
    public float attackSpeedBonus;
    private float startAttackSpeed;
    private Transform rageEffectParticle;

    [Header("Damage Bonus By Hit")]
    public float damageBonus;
    public float maxDamageBonus;
    public float damageBonusDuration;
    private float currentDamageBonus;

    private float damageBonusTimeExpiration;
    private float startAttackDamage;


    protected override void Awake()
    {
        base.Awake();
        startAttackDamage = attackDamage;
        startAttackSpeed = attackSpeed;

        rageEffectParticle = transform.Find("SpriteBody/RageEffect");
    }

    protected override void Update()
    {
        base.Update();
        if (damageBonusTimeExpiration <= Time.time)
            attackDamage = startAttackDamage;
        RageEffect();
    }


    public override void Attack()
    {
        base.Attack();
        IncreaseAttackDamage();
    }

    void IncreaseAttackDamage()
    {
        if (currentDamageBonus >= damageBonus)
        {
            currentDamageBonus = maxDamageBonus;
            return;
        }

        attackDamage *= 1 + (damageBonus / 100);
        damageBonusTimeExpiration = Time.time + damageBonusDuration;
    }


    void RageEffect()
    {
        if (attackSpeed == startAttackSpeed && IsHealthThresholdReached())
        {
            attackSpeed /= 1 + (attackSpeedBonus / 100);
            rageEffectParticle.gameObject.SetActive(true);
        }
        else if (!IsHealthThresholdReached())
        {
            attackSpeed = startAttackSpeed;
            rageEffectParticle.gameObject.SetActive(false);
        }
    }

    bool IsHealthThresholdReached()
    {
        float threshold = (1 - (healthThreshold / 100)) * maxHealth;
        print(threshold);
        return currentHealth <= threshold;
    }

    void CreateDodgeEffect()
    {
        if (dodgeEffect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(dodgeEffect);
            newEffect.transform.position = transform.position;
        }
    }
}
