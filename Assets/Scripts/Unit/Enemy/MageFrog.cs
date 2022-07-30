public class MageFrog : UnitShooter
{
    public float bigShieldAmount;

    protected override void Start()
    {
        base.Start();
        SetBigShieldCurrent(bigShieldAmount);
    }
}
