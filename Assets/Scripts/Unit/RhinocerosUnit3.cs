using UnityEngine;

public class RhinocerosUnit3 : RhinocerosUnit2
{
    [Header("AoeCharge")]
    //public float effectRange;
    public int count;
    protected event System.Action OnChargeEnd;

    int currentCount;

    protected override void Awake()
    {
        base.Awake();
        OnChargeEnd += testM;
    }

    void testM()
    {
        print("testM");
    }
    public override void Attack()
    {
        if (ChargeReady() && currentCount == count)
        {
            currentCount = 0;
            //OnChargeEnd.Invoke();
        }

        if (currentCount < count)
        {
            nextChargeTime = 0f;
            nextAttackTime = 0f;
            currentCount++;
            if (currentCount == count)
                OnChargeEnd.Invoke();
        }

        base.Attack();

        //DamageEnemiesAroundTarget();

    }

    //protected virtual void DamageEnemiesAroundTarget()
    //{
    //    GameObject[] enemies = GetEnemies();
    //    foreach (GameObject enemy in enemies)
    //    {
    //        float distance = Vector2.Distance(Target.transform.position, enemy.transform.position);
    //        if (distance <= effectRange)
    //        {
    //            enemy.GetComponent<Unit>().GetDamage(attackDamage, transform, "Classic");
    //        }
    //    }
    //}

}
