using UnityEngine;

public class FireskullUnit4 : FireskullUnit3
{
    [Header("Explosion")]
    public float explosionDamage;
    public float explosionRange;
    public float timeBetweenEffect4 = 5f;
    public GameObject magma;
    public GameObject triggerEffect4;
    float nextEffectTime4;


    protected override void Update()
    {
        if (Disabled)
            return;
        base.Update();
        if (nextEffectTime4 <= Time.time && EnoughRangeToAttackTarget())
            DoEffect();
    }
    void DoEffect()
    {
        Invoke(nameof(DamageEnemiesAround), 2f);

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        nextEffectTime4 = 0f;
    }
    private void DamageEnemiesAround()
    {
        if (!IsTargetEnabled(Target) || !EnoughRangeToAttackTarget() || nextEffectTime4 > Time.time)
        {
            //nextEffectTime4 = Time.time + 2f;
            return;
        }

        CreateEffect();

        GameObject[] enemies = GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= explosionRange)
            {
                enemy.GetComponent<HealthBar>().GetDamage(explosionDamage, transform);
                InstantiateTrap(enemy);
            }
        }
        nextEffectTime4 = Time.time + timeBetweenEffect4;

    }
    protected GameObject InstantiateTrap(GameObject target)
    {
        if (!magma || !target)
            return null;
        GameObject newTrap = poolObject.GetPoolObject(magma);
        newTrap.transform.position = target.transform.position;
        newTrap.GetComponent<Trap>().SetTargetTag(targetTag);
        //newTrap.GetComponent<Unit>().SetTargetTag(targetTag);

        return newTrap;
    }
    void CreateEffect()
    {
        if (triggerEffect4)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(triggerEffect4);
            newEffect.transform.position = transform.position;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
