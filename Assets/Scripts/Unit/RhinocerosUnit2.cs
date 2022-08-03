using UnityEngine;

public class RhinocerosUnit2 : RhinocerosUnit1
{
    [Header("KnockBack")]
    public float knockBackPower = 0.5f;


    public override void Attack()
    {
        if (ChargeReady() && Target.name != "EnemyCaptain")
            Target.transform.Translate(knockBackPower * wayX * Vector3.right);
        base.Attack();

    }

}
