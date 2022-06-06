using UnityEngine;

public class RockUnit3 : RockUnit2
{
    public float damageReflectedPercentage;

    protected override void Update()
    {
        base.Update();
        if (isDefenseBonusEnabled)
            ReflectBullets();
    }


    public override void GetDamage(float damage, Transform caller = null)
    {
        base.GetDamage(damage);

        ReflectDamage(damage, caller);
    }

    void ReflectBullets()
    {
        if (!isDefenseBonusEnabled)
            return;

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        foreach (GameObject bullet in bullets)
        {
            float distance = Vector2.Distance(transform.position, bullet.transform.position);
            float triggerRange = 0.25f;
            if (distance <= triggerRange && IsBulletAimingAtMe(bullet))
            {
                BulletScript bulletScript = bullet.GetComponent<BulletScript>();
                bulletScript.wayX *= -1;
                bulletScript.moveSpeed *= 1.25f;
                bulletScript.SetTargetTag(targetTag);
            }
        }
    }

    bool IsBulletAimingAtMe(GameObject bullet)
    {
        return bullet.GetComponent<BulletScript>().target.GetInstanceID() == gameObject.GetInstanceID();
    }

    void ReflectDamage(float damage, Transform caller)
    {
        if (!isDefenseBonusEnabled)
            return;
        if (caller)
        {
            if (caller.GetComponent<HealthBar>())
            {
                damage *= 1 - damageReductionPercentage / 100;
                caller.GetComponent<HealthBar>().GetDamage(damage);
            }
        }
    }


}
