using UnityEngine;

public class RhinocerosUnit3 : RhinocerosUnit2
{
    [Header("AoeCharge")]
    public int count;
    protected event System.Action OnChargeEnd;

    int currentCount;
    GameObject HitEffectBar;

    protected override void Awake()
    {
        base.Awake();
        if (!HitEffectBar)
            HitEffectBar = transform.Find("EffectBar/Canvas/Bar").gameObject;
    }

    protected override void Update()
    {
        base.Update();
        UpdateEffectBarLength();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        OnDeath += DisableHitEffectBar;
        HitEffectBar.transform.parent.transform.parent.gameObject.SetActive(true);

    }
    void DisableHitEffectBar()
    {
        OnDeath -= DisableHitEffectBar;
        HitEffectBar.transform.parent.transform.parent.gameObject.SetActive(false);
    }

    private void UpdateEffectBarLength()
    {
        HitEffectBar.transform.localScale = new Vector3(1 - (float)currentCount / count, HitEffectBar.transform.localScale.y, HitEffectBar.transform.localScale.z);
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
