using UnityEngine;

public class ChickenUnit4 : ChickenUnit3
{

    [Header("Rage Effect")]
    public float healthThreshold;
    public float attackSpeedBonus;
    private float startAttackSpeed;
    private Transform rageEffectParticle;

    [Header("Damage Bonus By Hit")]
    public float damageBonusPercentage;
    public float maxDamageBonusPercentage;
    public float damageBonusDuration;
    private float currentDamageBonus;

    private float damageBonusTimeExpiration;
    private float startAttackDamage;
    Transform AttackBonusBar;

    protected override void Awake()
    {
        base.Awake();
        startAttackDamage = attackDamage;
        startAttackSpeed = attackSpeed;

        rageEffectParticle = transform.Find("SpriteBody/RageEffect");
        AttackBonusBar = transform.Find("AttackBonusEffectBar/Canvas/Bar");
    }

    protected override void Update()
    {
        base.Update();
        if (damageBonusTimeExpiration <= Time.time)
            attackDamage = startAttackDamage;
        RageEffect();
        UpdateAttackBonusBarLength();
    }


    public override void Attack()
    {
        base.Attack();
        IncreaseAttackDamage();
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        OnDeath += DisableAttackBonusBar;
        AttackBonusBar.parent.gameObject.SetActive(true);
    }

    void DisableAttackBonusBar()
    {
        OnDeath -= DisableAttackBonusBar;
        AttackBonusBar.parent.gameObject.SetActive(false);
    }
    float GetMaxBonusPercentage()
    {
        return initialAttackDamage * (1 + maxDamageBonusPercentage / 100);
    }
    float GetDamageBonusPercentage()
    {
        return initialAttackDamage * (1 - (1 - (damageBonusPercentage / 100)));

    }
    void IncreaseAttackDamage()
    {
        damageBonusTimeExpiration = Time.time + damageBonusDuration;
        if (attackDamage >= GetMaxBonusPercentage())
        {
            attackDamage = GetMaxBonusPercentage();
            return;
        }
        if (currentDamageBonus == 0)
            currentDamageBonus += GetDamageBonusPercentage();
        attackDamage += currentDamageBonus;
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
        float threshold = (healthThreshold / 100) * maxHealth;
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

    private void UpdateAttackBonusBarLength()
    {
        AttackBonusBar.localScale = new Vector3(GetAttackBonusNewBarLength(), AttackBonusBar.localScale.y, AttackBonusBar.localScale.z);
    }

    private float GetAttackBonusNewBarLength()
    {
        float max = GetMaxBonusPercentage() - initialAttackDamage;
        float current = attackDamage - initialAttackDamage;
        float barLenght = current / max;
        return barLenght;
    }
}
