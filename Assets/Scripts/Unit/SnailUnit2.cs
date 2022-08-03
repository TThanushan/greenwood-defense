using UnityEngine;

public class SnailUnit2 : Unit
{
    public float healthShieldBonusRespawnTime;
    public float healthShieldBonusHealthAmount;
    [Range(0, 100)]
    public float healthShieldBonusPercentage;
    public float bonusRange;

    float healthBonusCooldown;

    private GameObject HitEffectBar;

    protected override void Awake()
    {
        base.Awake();
        if (!HitEffectBar)
            HitEffectBar = transform.Find("EffectBar/Canvas/Bar").gameObject;

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnDeath += DisableHitEffectBar;
        HitEffectBar.transform.parent.transform.parent.gameObject.SetActive(true);

    }
    void DisableHitEffectBar()
    {
        OnDeath -= DisableHitEffectBar;
        HitEffectBar.transform.parent.transform.parent.gameObject.SetActive(false);
    }
    private void UpdateEffectBarLength()
    {
        HitEffectBar.transform.localScale = new Vector3(GetNewBarLength(), HitEffectBar.transform.localScale.y, HitEffectBar.transform.localScale.z);
    }

    private float GetNewBarLength()
    {
        float barLength = 1 - (Mathf.Abs(healthBonusCooldown - Time.time) / healthShieldBonusRespawnTime);
        return barLength;
    }


    protected override void Update()
    {
        base.Update();

        if (healthBonusCooldown <= Time.time)
        {
            healthBonusCooldown = Time.time + healthShieldBonusRespawnTime;
            GiveHealthBonusToAllies();
        }

        UpdateEffectBarLength();
    }
    void GiveHealthBonusToAllies()
    {
        GameObject[] allies = poolObject.Allies;
        if (allies == null || allies.Length == 0)
            return;

        foreach (GameObject ally in allies)
        {
            float distance = Vector2.Distance(transform.position, ally.transform.position);
            if (distance <= bonusRange)
            {
                DoEffect(ally);

            }
        }
    }

    protected virtual void DoEffect(GameObject ally)
    {
        HealthBar healthBarScript = ally.GetComponent<HealthBar>();
        healthBarScript.SetShieldRelatedToCurrentHealthPercentage(healthShieldBonusPercentage);
        healthBarScript.SetShield(healthShieldBonusHealthAmount);
    }
}
