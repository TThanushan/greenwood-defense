using System.Collections;
using UnityEngine;

public class TrunkUnit : UnitShooter
{
    public int bulletShotEachTime = 3;
    public float timeBetweenShoot = 0.25f;
    float currentAmmo;
    Transform ammoBar;
    Transform reloadAmmoBar;
    public int AMMO_RELOAD_AMOUNT = 3;

    bool shooting;
    protected override void Awake()
    {
        base.Awake();
        ammoBar = transform.Find("AmmoBar/Canvas/Bar");
        reloadAmmoBar = transform.Find("AmmoBar/Canvas/ReloadBar");

        currentAmmo = bulletShotEachTime;
        InvokeRepeating("ReloadCurrentAmmo", 0f, 0.2f);
    }
    protected override void Update()
    {
        base.Update();
        UpdateAmmoBarLength();
        UpdateReloadAmmoBarLength();

        if (InRangeWithTarget() && currentAmmo >= 5 && !shooting)
            StartCoroutine(ShootInRow(Target));
        else if (!InRangeWithTarget())
            shooting = false;
    }

    void ReloadCurrentAmmo()
    {
        // Reload if not target or if in range but ammo under 5
        if (!InRangeWithTarget() || (InRangeWithTarget() && currentAmmo < 5))
        {
            currentAmmo += AMMO_RELOAD_AMOUNT;
            if (currentAmmo > bulletShotEachTime)
                currentAmmo = bulletShotEachTime;
        }
    }

    protected override void Shoot(GameObject target)
    {
    }

    IEnumerator ShootInRow(GameObject target)
    {
        shooting = true;
        for (int i = 0; i < bulletShotEachTime; i++)
        {
            currentAmmo--;
            if (!InRangeWithTarget() || currentAmmo <= 0)
                break;
            InstantiateBullet(target);
            PlayHitSfx();
            yield return new WaitForSeconds(timeBetweenShoot);
        }

        if (currentAmmo <= 0)
            shooting = false;

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
        barLength = GetReloadAmmoBarNewBarLength();
        if (barLength > 1f)
            barLength = 1f;
        reloadAmmoBar.localScale = new Vector3(barLength, reloadAmmoBar.localScale.y, reloadAmmoBar.localScale.z);
    }

    private float GetReloadAmmoBarNewBarLength()
    {
        if (InRangeWithTarget())
            return 0f;
        float currentAmmoSave = currentAmmo;
        currentAmmo += AMMO_RELOAD_AMOUNT * 2;
        float barLenght = GetAmmoBarNewBarLength();
        currentAmmo = currentAmmoSave;
        return barLenght;
    }
}
