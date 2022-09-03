using UnityEngine;

public class PigUnit4 : PigUnit3
{
    [Header("MaxManaIncrease")]
    public int manaMaxIncrease;

    bool isManaIncreased;
    protected override void Start()
    {
        base.Start();
        OnDeath += DecreaseManaMax;
        //IncreaseManaMax();

    }

    //protected override void OnEnable()
    //{
    //    base.OnEnable();
    //    if (manaBar)
    //        IncreaseManaMax();
    //}
    protected override void Update()
    {
        if (Disabled)
            return;
        base.Update();
        if (!isManaIncreased)
            IncreaseManaMax();
    }
    void DecreaseManaMax()
    {
        manaBar.maxMana -= manaMaxIncrease;
        if (manaBar.currentMana > manaBar.maxMana)
            manaBar.currentMana = manaBar.maxMana;
        isManaIncreased = false;
        manaBar.UpdateManaMaxText();
    }

    void IncreaseManaMax()
    {

        manaBar.maxMana += manaMaxIncrease;
        isManaIncreased = true;
        manaBar.UpdateManaMaxText();
    }
}
