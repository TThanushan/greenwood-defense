using UnityEngine;

public class SnailUnit5 : SnailUnit4
{
    [Range(0, 100)]
    public float healthTriggerPercentage;
    public float timeBetweenlowLifeGlobalHeal;
    public GameObject globalHealEffect;
    public float globalHealPercentage = 100f;
    private float lowLifeGlobalHealCooldown;
    private GameObject[] allies;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        poolObject = PoolObject.instance;
    }

    protected override void Update()
    {
        // Cache allies at the beginning of the update cycle to avoid repeated calls
        allies = poolObject.GetAlliesAsArray();

        if (allies.Length > 0 && IsInRangeWithAlly() && IsAnyAllyLowLife() && lowLifeGlobalHealCooldown <= Time.time)
            StartLowLifeGlobalHeal();

        base.Update();
    }

    private bool IsAnyAllyLowLife()
    {
        foreach (GameObject ally in allies)
        {
            Unit unit = ally.GetComponent<Unit>();
            if (unit.Disabled || ally.name.Contains("ReallySmallSlime") || ally.name.Contains("WeakLittleBunny"))
                continue;

            float percentage = unit.currentHealth / unit.maxHealth * 100;
            if (percentage <= healthTriggerPercentage)
                return true;
        }
        return false;
    }

    private void StartLowLifeGlobalHeal()
    {
        foreach (GameObject ally in allies)
        {
            float distance = Vector2.Distance(transform.position, ally.transform.position);
            if (distance <= bonusRange)
            {
                HealAlly(ally, globalHealPercentage);
            }
        }
        CreateEffect();
        lowLifeGlobalHealCooldown = Time.time + timeBetweenlowLifeGlobalHeal;
    }

    private bool IsInRangeWithAlly()
    {
        foreach (GameObject ally in allies)
        {
            float distance = Vector2.Distance(transform.position, ally.transform.position);
            if (distance <= bonusRange)
            {
                return true;
            }
        }
        return false;
    }

    private void CreateEffect()
    {
        if (!globalHealEffect)
            return;

        GameObject newEffect = poolObject.GetPoolObject(globalHealEffect);
        newEffect.transform.position = transform.position;
    }
}
