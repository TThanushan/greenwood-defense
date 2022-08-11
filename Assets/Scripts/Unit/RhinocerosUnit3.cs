using UnityEngine;

public class RhinocerosUnit3 : RhinocerosUnit2
{
    [Header("AoeCharge")]
    public int count;
    protected event System.Action OnChargeEnd;

    int currentCount;

    protected override void Awake()
    {
        base.Awake();
    }


    public override void Attack()
    {
        if (ChargeReady() && currentCount == count)
        {
            currentCount = 0;
        }

        if (currentCount < count)
        {
            nextChargeTime = 0f;
            nextAttackTime = 0f;
            currentCount++;
            if (currentCount == count)
                OnChargeEnd?.Invoke();
        }

        base.Attack();


    }


}
