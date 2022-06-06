
using UnityEngine;

public class ChickenUnit3 : HitBasedUnit
{
    public float dodgeChance = 25f;
    public GameObject dodgeEffect;
    bool IsDodgingAttack()
    {
        int rand = Random.Range(0, 100);
        return rand <= dodgeChance;
    }

    public override void GetDamage(float damage, Transform caller = null)
    {
        if (!IsDodgingAttack())
            base.GetDamage(damage);
        else
            CreateDodgeEffect();
    }

    void CreateDodgeEffect()
    {
        if (dodgeEffect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(dodgeEffect);
            newEffect.transform.position = transform.position;
        }
    }

}