using UnityEngine;

public class ChameleonUnit : Unit
{
    [Range(1, 4)]
    public int unitLevel = 1;
    public float cycleDuration;

    [Header("AttackSpeedReductionOnHit")]
    //const string ATTACK_SPEED_REDUCTION = "AttackSpeedReductionOnHit";
    [Range(0f, 100f)]
    public float attackSpeedReduction = 40f;
    public float effectDuration = 2f;

    [Header("RageEffect")]
    //const string RAGE_EFFECT = "RageEffect";
    public float attackSpeedBuff = 40f;
    public float attackDamageBuff = 40f;

    [Header("LifeSteal")]
    //const string LIFE_STEAL = "LifeSteal";
    public float lifeStealPercentage = 40f;
    int currentStateIndex = -1;
    float nextCycleTime;


    GameObject rageEffectSprite;
    GameObject lifeStealSprite;
    GameObject attackSpeedReductionSprite;

    private GameObject HitEffectBar;

    public override void Attack()
    {
        base.Attack();
        if (unitLevel < 4)
        {
            if (currentStateIndex == 0)
                SlowTargetAttackSpeed();
            if (currentStateIndex == 2 && unitLevel == 3)
                LifeSteal(attackDamage);
        }
        else
        {
            if (currentStateIndex == 0 || currentStateIndex == 1)
                SlowTargetAttackSpeed();
            if (currentStateIndex == 2 || currentStateIndex == 3)
                LifeSteal(attackDamage);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        if (!HitEffectBar)
        {
            if (unitLevel == 1)
                transform.Find("EffectBar").gameObject.SetActive(false);
            else
                HitEffectBar = transform.Find("EffectBar/Canvas/Bar").gameObject;
        }

        rageEffectSprite = transform.Find("SpriteBody/SwordSprite").gameObject;
        lifeStealSprite = transform.Find("SpriteBody/HeartSprite").gameObject;
        attackSpeedReductionSprite = transform.Find("SpriteBody/SlowSprite").gameObject;
    }
    protected override void Update()
    {
        base.Update();
        RunCycle();
        UpdateEffectBarLength();

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnDeath += DisableHitEffectBar;
        HitEffectBar.transform.parent.transform.parent.gameObject.SetActive(true);

    }
    private void UpdateEffectBarLength()
    {
        if (!HitEffectBar)
            return;
        HitEffectBar.transform.localScale = new Vector3(GetNewBarLength(), HitEffectBar.transform.localScale.y, HitEffectBar.transform.localScale.z);
    }
    private float GetNewBarLength()
    {
        float barLength = 1 - (Mathf.Abs(nextCycleTime - Time.time) / cycleDuration);
        return barLength;
    }
    void RunCycle()
    {
        if (nextCycleTime > Time.time)
            return;
        IterateState();

        if (unitLevel < 4)
        {
            if (currentStateIndex == 0)
            {
                DisableRageEffect();
                GetUnitSpriteRenderer().color = Color.white;
                DisableAllSprite();
                attackSpeedReductionSprite.SetActive(true);
            }
            if (currentStateIndex == 1 && unitLevel >= 2)
            {
                GetUnitSpriteRenderer().color = Color.red;
                DisableAllSprite();
                rageEffectSprite.SetActive(true);
                EnableRageEffect();
            }
            if (currentStateIndex == 2 && unitLevel >= 3)
            {
                GetUnitSpriteRenderer().color = Color.yellow;
                DisableAllSprite();
                lifeStealSprite.SetActive(true);
                DisableRageEffect();
            }
        }
        else
        {
            if (currentStateIndex == 0)//
            {
                DisableRageEffect();
                GetUnitSpriteRenderer().color = Color.white;
                DisableAllSprite();
                attackSpeedReductionSprite.SetActive(true);
            }
            if (currentStateIndex == 1)//
            {
                GetUnitSpriteRenderer().color = Color.red;
                DisableAllSprite();
                rageEffectSprite.SetActive(true);
                EnableRageEffect();
            }
            if (currentStateIndex == 2)
            {
                GetUnitSpriteRenderer().color = Color.yellow;
                DisableAllSprite();
                lifeStealSprite.SetActive(true);
                //DisableRageEffect();
            }
        }


        nextCycleTime = Time.time + cycleDuration;
    }


    void DisableHitEffectBar()
    {
        OnDeath -= DisableHitEffectBar;
        HitEffectBar.transform.parent.transform.parent.gameObject.SetActive(false);
    }
    void DisableAllSprite()
    {
        attackSpeedReductionSprite.SetActive(false);
        lifeStealSprite.SetActive(false);
        rageEffectSprite.SetActive(false);

    }

    void IterateState()
    {
        currentStateIndex += 1;
        if (currentStateIndex >= unitLevel || currentStateIndex == 3)
            currentStateIndex = 0;
    }

    void EnableRageEffect()
    {
        if (attackDamage != initialAttackDamage)
            return;
        nextAttackTime = 0f;
        attackSpeed *= 1 - attackSpeedBuff / 100;
        attackDamage *= 1 + attackDamageBuff / 100;
    }

    void DisableRageEffect()
    {
        InvokeResetAttackDamage(0.01f);
        InvokeResetAttackSpeed(0.01f);

    }

    void LifeSteal(float damageDone)
    {
        print("lifesteal");
        currentHealth += damageDone * lifeStealPercentage / 100;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    void SlowTargetAttackSpeed()
    {
        print("slow");
        Unit unit = Target.GetComponent<Unit>();
        if (unit.attackSpeed == unit.GetInitialAttackSpeed())
            unit.attackSpeed *= 1 + attackSpeedReduction / 100f;
        unit.InvokeResetAttackSpeed(effectDuration);
    }
}
