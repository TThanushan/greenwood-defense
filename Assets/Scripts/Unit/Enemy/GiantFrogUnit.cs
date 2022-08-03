using UnityEngine;

public class GiantFrogUnit : UnitAoeAttack
{
    [Header("PassiveHealRegen")]
    public float regenAmount = 10;
    public float timeBetweenRegen = 1;

    [Header("DefenseBonus")]
    [Range(0, 100)]
    public float currentHealthPercentageThreshold;
    public float damageReductionPercentage;
    protected bool isDefenseBonusEnabled;
    public Color defenseColor;

    GameObject defenseBonusEffect;

    protected override void Awake()
    {
        base.Awake();
        OnDeath += DisableDefenseBonus;
        defenseBonusEffect = GetUnitSpriteRenderer().gameObject;
        InvokeRepeating(nameof(RegenHealth), 0f, timeBetweenRegen);
    }

    protected override void Update()
    {
        base.Update();

        if (!isDefenseBonusEnabled && currentHealth / maxHealth <= currentHealthPercentageThreshold / 100)
        {
            ResetDefenseBonus();
        }
    }

    void RegenHealth()
    {
        currentHealth += regenAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    void ResetDefenseBonus()
    {
        isDefenseBonusEnabled = true;
        EnableDefenseBonusEffect(true);
        //StartCoroutine(DisableDefenseBonus());
    }

    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        if (isDefenseBonusEnabled)
            damage *= 1 - damageReductionPercentage / 100;
        base.GetDamage(damage, caller, HitSoundName);
    }

    protected virtual void DisableDefenseBonus()
    {
        isDefenseBonusEnabled = false;
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
