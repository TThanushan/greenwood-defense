using UnityEngine;

public class UnitShooter : Unit
{
    public GameObject bullet;
    public GameObject bullet2;
    public Transform bulletPosition;
    //public float attackDamage;
    //public float bulletSpeed;
    public override void Attack()
    {
        Shoot(Target);
    }

    protected virtual void Shoot(GameObject target)
    {
        InstantiateBullet(target, bullet);
        InstantiateBullet(target, bullet2);
    }

    protected override void Update()
    {
        base.Update();
        Target = GetClosestEnemy();
    }


    void MoveTowardTargetY()
    {
        if (!Target) return;

        Vector3 actualTargetPosition = new Vector3(transform.position.x, Target.transform.position.y);
        Vector2 dir = actualTargetPosition - transform.position;
        float reduceCoef = 3f;
        float reducedMoveSpeed = moveSpeed / reduceCoef;
        transform.Translate(dir.normalized * reducedMoveSpeed * Time.deltaTime);
    }
    protected override void MoveTowardTarget()
    {
        if (!Target) return;

        // Does unit needs to move only on Y axis ?
        if (InRangeWithTarget() && !AlignedOnYWithTarget())
            MoveTowardTargetY();
        else
            base.MoveTowardTarget();
    }

    bool AlignedOnYWithTarget()
    {
        float yDist = Mathf.Abs(transform.position.y - Target.transform.position.y);
        float yRange = 0.01f;
        return yDist < yRange;

    }

    protected bool InRangeWithTarget()
    {
        if (!Target)
            return false;
        return Vector2.Distance(transform.position, Target.transform.position) <= attackRange;
    }
    public override bool EnoughRangeToAttackTarget()
    {
        return InRangeWithTarget() && AlignedOnYWithTarget();
    }
    protected GameObject InstantiateBullet(GameObject target, GameObject _bullet)
    {
        if (!_bullet || !target)
            return null;
        GameObject newBullet = poolObject.GetPoolObject(_bullet);
        newBullet.transform.position = bulletPosition.transform.position;

        BulletScript newBulletScript = newBullet.GetComponent<BulletScript>();
        newBulletScript.target = target;
        newBulletScript.SetTargetTag(targetTag);
        newBulletScript.attackDamage = attackDamage;
        newBulletScript.wayX = wayX;

        //newBulletScript.moveSpeed = bulletSpeed;
        //newBulletScript.piercingDamage = piercingDamage;

        return newBullet;
    }

    protected GameObject InstantiateBullet(GameObject target)
    {
        if (!bullet || !target)
            return null;
        GameObject newBullet = poolObject.GetPoolObject(bullet);
        newBullet.transform.position = bulletPosition.transform.position;

        BulletScript newBulletScript = newBullet.GetComponent<BulletScript>();
        newBulletScript.target = target;
        newBulletScript.SetTargetTag(targetTag);
        newBulletScript.attackDamage = attackDamage;
        newBulletScript.wayX = wayX;

        //newBulletScript.moveSpeed = bulletSpeed;
        //newBulletScript.piercingDamage = piercingDamage;

        return newBullet;
    }

}
