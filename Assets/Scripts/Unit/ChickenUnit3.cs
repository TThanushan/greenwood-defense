
using UnityEngine;

public class ChickenUnit3 : HitBasedUnit
{
    public float dodgeChance = 25f;
    public GameObject dodgeEffect;
    bool IsDodgingAttack(Transform enemyCaller = null)
    {
        if (enemyCaller && enemyCaller.GetComponent<PoisonFrog>())
            return false;
        int rand = Random.Range(0, 100);
        return rand <= dodgeChance;
    }

    public override void GetDamage(float damage, Transform caller = null)
    {
        if (!IsDodgingAttack(caller))
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