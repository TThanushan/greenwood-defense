using UnityEngine;

public class RhinocerosUnit3 : RhinocerosUnit2
{
    [Header("AoeCharge")]
    public int count;
    protected event System.Action OnChargeEnd;
    protected event System.Action OnChargeStart;

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
        if (currentCount > 0)
            ChangeMoveSpeedIfUnchanged(moveSpeedBonus);


    }
    protected override void OnEnable()
    {
        base.OnEnable();
        //OnDeath += DisableHitEffectBar;
        //HitEffectBar.transform.parent.transform.parent.gameObject.SetActive(true);
        //UpdateEffectBarLength();
        HitEffectBar.transform.localScale = new Vector3(1f, HitEffectBar.transform.localScale.y, HitEffectBar.transform.localScale.z);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        currentCount = count;
    }


    private void UpdateEffectBarLength()
    {
        float x = 1f - ((float)currentCount / count);
        if (currentCount == count && ChargeReady())
            x = 1;
        HitEffectBar.transform.localScale = new Vector3(x, HitEffectBar.transform.localScale.y, HitEffectBar.transform.localScale.z);
    }

    public override void Attack()
    {
        if (Target.name == "EnemyCaptain")
        {
            base.Attack();
            return;
        }
        if (ChargeReady() && currentCount == count)
        {
            currentCount = 0;
        }

        if (currentCount < count)
        {
            nextChargeTime = 0f;
            nextAttackTime = 0f;
            currentCount++;
            if (currentCount == 1)
                OnChargeStart?.Invoke();

            if (currentCount == count)
                OnChargeEnd?.Invoke();
        }
        base.Attack();


    }


}
