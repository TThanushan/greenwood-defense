using System.Collections;
using UnityEngine;

public class TrunkUnit : UnitShooter
{
    public int bulletShotEachTime = 3;
    public float timeBetweenShoot = 0.25f;


    protected override void Shoot(GameObject target)
    {
        StartCoroutine(ShootInRow(target));
    }

    IEnumerator ShootInRow(GameObject target)
    {
        for (int i = 0; i < bulletShotEachTime; i++)
        {
            if (!InRangeWithTarget())
                break;
            InstantiateBullet(target);
            yield return new WaitForSeconds(timeBetweenShoot);
        }
    }
}
