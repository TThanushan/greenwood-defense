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
    public int ammoMinimumToShoot = 5;
    public float ammoReloadTime = 0.2f;
    bool shooting;

    float ammoSave;

    protected override void Awake()
    {
        base.Awake();
        ammoBar = transform.Find("AmmoBar/Canvas/Bar");
        reloadAmmoBar = transform.Find("AmmoBar/Canvas/ReloadBar");

        currentAmmo = bulletShotEachTime;
        InvokeRepeating(nameof(ReloadCurrentAmmo), 0f, ammoReloadTime);
        InvokeRepeating(nameof(Unblocker), 0f, 1f);
    }

    void Unblocker()
    {
        if (currentAmmo == bulletShotEachTime)
            return;
        if (currentAmmo == ammoSave)
            shooting = false;
        else if (shooting)
            ammoSave = currentAmmo;
    }
    protected override void Update()
    {
        base.Update();
        if (Disabled)
            return;
        UpdateAmmoBarLength();
        UpdateReloadAmmoBarLength();

        if (InRangeWithTarget() && currentAmmo >= ammoMinimumToShoot && !shooting)
            StartCoroutine(ShootInRow(Target));
        else if (!InRangeWithTarget())
            shooting = false;
        //Unblocker();
    }

    void ReloadCurrentAmmo()
    {
        // Reload if not target or if in range but ammo under 5
        if (!InRangeWithTarget() || (InRangeWithTarget() && currentAmmo < ammoMinimumToShoot))
        {
            currentAmmo += AMMO_RELOAD_AMOUNT;
            if (currentAmmo > bulletShotEachTime)
                currentAmmo = bulletShotEachTime;
        }
    }

    protected override void Shoot(GameObject target)
    {
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        shooting = false;
        currentAmmo = bulletShotEachTime;
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
