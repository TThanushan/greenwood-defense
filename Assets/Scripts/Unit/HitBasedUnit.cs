using System.Collections;
using UnityEngine;

public class HitBasedUnit : Unit
{

    public enum Effect { DoubleHit, BlockNextAttack };

    [Header("Hit Based")]
    public Effect effect = Effect.DoubleHit;
    public float hitCount;
    public float hitCountNeeded;
    public float timeBetweenHitEffect;
    private GameObject HitEffectBar;


    // BlockNextAttack
    bool nextAttackBlocked;
    //public GameObject effectParticleEffect;
    public GameObject blockEffectParticleEffect;

    public override void Awake()
    {
        base.Awake();
        if (!HitEffectBar)
            HitEffectBar = transform.Find("EffectBar/Canvas/Bar").gameObject;
        hitCount = 0;
    }

    protected override void Update()
    {
        base.Update();
        UpdateHitBarLength();
    }

    private void UpdateHitBarLength()
    {
        HitEffectBar.transform.localScale = new Vector3(GetNewBarLength(), HitEffectBar.transform.localScale.y, HitEffectBar.transform.localScale.z);
        print(GetNewBarLength());
    }
    private float GetNewBarLength()
    {
        float barLenght = hitCount / hitCountNeeded;
        return barLenght;
    }

    public override void Attack()
    {
        base.Attack();
        if (effect == Effect.DoubleHit)
            IncreaseHitCount();
    }

    protected override void AttackTarget()
    {
        if (IsHitCountNeededReached())
        {
            DoEffect();
            hitCount = 0f;
        }
        else
            base.AttackTarget();
    }

    void DoEffect()
    {
        if (effect == Effect.DoubleHit)
            StartCoroutine(DoubleHitEffect(timeBetweenHitEffect));
        else if (effect == Effect.BlockNextAttack)
            BlockNextAttack();
    }

    IEnumerator DoubleHitEffect(float time)
    {
        InvokeOnAttack();
        Attack();
        yield return new WaitForSeconds(time);
        InvokeOnAttack();
        nextAttackTime = GetRandomizedNextAttackTime();
        Attack();
        hitCount = 0f;

    }

    void BlockNextAttack()
    {
        nextAttackBlocked = true;
        blockEffectParticleEffect.SetActive(true);

    }

    public override void GetDamage(float damage)
    {
        if (nextAttackBlocked)
        {
            nextAttackBlocked = false;
            blockEffectParticleEffect.SetActive(false);
            return;
        }
        if (effect == Effect.BlockNextAttack)
            IncreaseHitCount();
        base.GetDamage(damage);
    }

    //protected void CreateParticleEffect()
    //{
    //    if (effectParticleEffect)
    //    {
    //        GameObject newEffect = PoolObject.instance.GetPoolObject(effectParticleEffect);
    //        newEffect.transform.position = transform.position;
    //    }
    //}

    bool IsHitCountNeededReached()
    {
        return hitCount == hitCountNeeded;
    }

    void IncreaseHitCount()
    {
        hitCount++;
        if (hitCount > hitCountNeeded)
            hitCount = hitCountNeeded;
    }

}
