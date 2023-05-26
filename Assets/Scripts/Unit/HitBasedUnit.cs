using System.Collections;
using UnityEngine;

public class HitBasedUnit : Unit
{

    public enum Effect { DoubleHit, TripleHit, BlockNextAttack };

    [Header("Hit Based")]
    public Effect effect = Effect.DoubleHit;
    private float hitCount;
    public float hitCountNeeded;
    public float timeBetweenHitEffect;
    private GameObject HitEffectBar;


    // BlockNextAttack
    bool isNextAttackBlocked;
    //public GameObject effectParticleEffect;
    public GameObject blockEffect;

    protected override void Awake()
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


    void DisableHitEffectBar()
    {
        OnDeath -= DisableHitEffectBar;
        transform.Find("EffectBar").gameObject.SetActive(false);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        transform.Find("EffectBar").gameObject.SetActive(true);
        OnDeath += DisableHitEffectBar;
        ResetStats();
    }

    private void ResetStats()
    {
        hitCount = 0f;
    }

    private void UpdateHitBarLength()
    {
        HitEffectBar.transform.localScale = new Vector3(GetNewBarLength(), HitEffectBar.transform.localScale.y, HitEffectBar.transform.localScale.z);
    }
    private float GetNewBarLength()
    {
        float barLength = hitCount / hitCountNeeded;
        return barLength;
    }

    public override void Attack()
    {
        if (!EnoughRangeToAttackTarget())
            return;
        base.Attack();
        if (effect == Effect.DoubleHit || effect == Effect.TripleHit)
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
        else if (effect == Effect.TripleHit)
            StartCoroutine(TripleHitEffect(timeBetweenHitEffect));
        else if (effect == Effect.BlockNextAttack)
            BlockNextAttack();
    }

    IEnumerator DoubleHitEffect(float time)
    {
        InvokeOnAttack();
        Attack();
        yield return new WaitForSeconds(time);
        if (!IsTargetEnabled(Target) || !EnoughRangeToAttackTarget())
            yield return null;
        InvokeOnAttack();
        nextAttackTime = GetRandomizedNextAttackTime();
        Attack();
        hitCount = 0f;

    }

    IEnumerator TripleHitEffect(float time)
    {
        if (!IsTargetEnabled(Target) || !EnoughRangeToAttackTarget())
            yield return null;
        InvokeOnAttack();
        Attack();
        hitCount = 0f;
        yield return new WaitForSeconds(time);
        if (!IsTargetEnabled(Target) || !EnoughRangeToAttackTarget())
            yield return null;
        InvokeOnAttack();
        Attack();
        hitCount = 0f;
        yield return new WaitForSeconds(time);
        if (!IsTargetEnabled(Target) || !EnoughRangeToAttackTarget())
            yield return null;
        InvokeOnAttack();
        nextAttackTime = GetRandomizedNextAttackTime();
        Attack();
        hitCount = 0f;

    }


    void BlockNextAttack()
    {
        isNextAttackBlocked = true;
        CreateBlockEffect();
    }


    void CreateBlockEffect()
    {
        if (blockEffect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(blockEffect);
            newEffect.transform.position = transform.position;
        }
    }



    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        if (isNextAttackBlocked)
        {
            isNextAttackBlocked = false;
            return;
        }
        if (effect == Effect.BlockNextAttack && !IsCallerPoisoning(caller) && caller && !caller.GetComponent<Trap>())
            IncreaseHitCount();
        base.GetDamage(damage, caller, HitSoundName);
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