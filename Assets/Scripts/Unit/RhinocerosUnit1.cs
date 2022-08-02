using UnityEngine;

public class RhinocerosUnit1 : Unit
{
    [Header("Charge")]
    public float damageBonus = 5;
    public float timeBetweenCharge = 5;

    protected float nextChargeTime;
    GameObject chargeEffect;

    protected override void Start()
    {
        base.Start();
        chargeEffect = transform.Find("SpriteBody/ChargeEffect").gameObject;
    }
    protected override void Update()
    {
        base.Update();
        chargeEffect.SetActive(ChargeReady());

    }

    protected bool ChargeReady()
    {
        return Time.time > nextChargeTime;
    }

    public override void Attack()
    {
        if (ChargeReady())
        {
            attackDamage += damageBonus;
        }

        base.Attack();

        if (ChargeReady())
        {
            attackDamage -= damageBonus;
            nextChargeTime = Time.time + timeBetweenCharge;
        }
    }
}
