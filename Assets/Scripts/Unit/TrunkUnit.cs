using System.Collections;
using UnityEngine;

public class TrunkUnit : UnitShooter
{
    public int bulletShotEachTime = 3;
    public float timeBetweenShoot = 0.25f;
    float currentAmmo;
    Transform ammoBar;
    Transform reloadAmmoBar;

    protected override void Awake()
    {
        base.Awake();
        ammoBar = transform.Find("AmmoBar/Canvas/Bar");
        reloadAmmoBar = transform.Find("AmmoBar/Canvas/ReloadBar");

        currentAmmo = bulletShotEachTime;

    }
    protected override void Update()
    {
        base.Update();
        UpdateAmmoBarLength();
        UpdateReloadAmmoBarLength();

    }
    protected override void Shoot(GameObject target)
    {
        StartCoroutine(ShootInRow(target));
    }

    IEnumerator ShootInRow(GameObject target)
    {
        currentAmmo = bulletShotEachTime;

        for (int i = 0; i < bulletShotEachTime; i++)
        {
            currentAmmo--;
            if (!InRangeWithTarget())
                break;
            InstantiateBullet(target);
            PlayHitSfx();

            yield return new WaitForSeconds(timeBetweenShoot);
            nextAttackTime = attackSpeed + Time.time;
        }
        //TODO:Reduce reload time base on remaining ammo.
        currentAmmo = 0f;

    }



    private void UpdateAmmoBarLength()
    {
        ammoBar.localScale = new Vector3(GetAmmoBarNewBarLength(), ammoBar.localScale.y, ammoBar.localScale.z);
    }

    private float GetAmmoBarNewBarLength()
    {
        float barLength = currentAmmo / bulletShotEachTime;
        if (barLength > 1f)
            barLength = 1f;
        return barLength;
    }

    private void UpdateReloadAmmoBarLength()
    {
        float barLength = 0f;
        if (currentAmmo <= 0)
            barLength = GetReloadAmmoBarNewBarLength();
        if (barLength > 1f)
            barLength = 1f;
        reloadAmmoBar.localScale = new Vector3(barLength, reloadAmmoBar.localScale.y, reloadAmmoBar.localScale.z);
    }

    private float GetReloadAmmoBarNewBarLength()
    {
        if (nextAttackTime <= 0)
            return 0f;
        float max = attackSpeed;
        float current = attackSpeed - (nextAttackTime - Time.time);
        float barLenght = current / max;
        return barLenght;
    }
}
