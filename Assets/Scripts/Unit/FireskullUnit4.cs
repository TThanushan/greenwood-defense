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
        CreateEffect();
        Invoke("DamageEnemiesAround", 0.1f);

        nextEffectTime4 = Time.time + timeBetweenEffect3;
    }

    private void DamageEnemiesAround()
    {
        GameObject[] enemies = GetEnemies();
        print("bip2");
        foreach (GameObject enemy in enemies)
        {
            print("bip1");
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= explosionRange)
            {
                print("bip");
                print(enemy.name);
                enemy.GetComponent<HealthBar>().GetDamage(explosionDamage, transform, "Classic");
                InstantiateTrap(enemy);
            }
        }

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
