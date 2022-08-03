using UnityEngine;

public class PigUnit1 : BunnyUnit2
{
    [Header("MoneyGenerated")]
    public float moneyGenerated;
    public override void Attack()
    {
        base.Attack();
        GivePlayerMoney();
    }

    void GivePlayerMoney()
    {
        poolObject.stageManager.GivePlayerMoney(moneyGenerated);
    }
}