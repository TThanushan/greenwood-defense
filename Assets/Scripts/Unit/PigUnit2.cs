using UnityEngine;

public class PigUnit2 : PigUnit1
{
    [Header("ManaGenerated")]
    public float manaGenerated;

    protected ManaBar manaBar;

    protected override void Start()
    {
        base.Start();
        manaBar = ManaBar.instance;
    }
    public override void Attack()
    {
        base.Attack();
        GivePlayerMana();
    }

    void GivePlayerMana()
    {
        manaBar.currentMana += manaGenerated;
    }
}