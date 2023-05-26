using UnityEngine;

public class BlueBirdUnit3save : BlueBirdUnit2
{

    public float travelTime;
    public float timeBetweenTravel;

    float currentTimeBetweenTravel;
    bool isTraveling;
    int originalWayX;
    float originalMoveSpeed;

    protected override void Awake()
    {
        base.Awake();
        originalWayX = wayX;
        originalMoveSpeed = moveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        if (!isTraveling && currentTimeBetweenTravel <= Time.time && EnoughRangeToAttackTarget())
            DropEggsOnDistance();
        if (isTraveling)
            MoveToward();

    }

    protected override void DoEffect()
    {
        if (isTraveling)
            DropEgg();
    }

    void DropEggsOnDistance()
    {
        currentTimeBetweenTravel = (travelTime * 2) + timeBetweenTravel;
        isTraveling = true;
        Invoke("ReverseWayX", travelTime);

        Invoke("StopTravel", travelTime * 2);
    }
    protected override void AttackTarget()
    {
        if (isTraveling)
            return;

        base.AttackTarget();
    }
    void StopTravel()
    {
        isTraveling = false;
        wayX = originalWayX;
        moveSpeed = originalMoveSpeed;
    }
    void ReverseWayX()
    {
        wayX *= -1;
    }


    protected override void MoveTowardTarget()
    {
        if (isTraveling)
            return;
        base.MoveTowardTarget();
    }


}
