using UnityEngine;

public class RockUnit3 : RockUnit2
{
    [Header("ReflectDamage")]
    public float damageReflectedPercentage;
    GameObject[] bullets;
    GameObject reflectDamageEffect;


    protected override void Start()
    {
        base.Start();
        reflectDamageEffect = transform.Find("SpriteBody/ReflectDamageEffect").gameObject;
        //InvokeRepeating("ReflectBullets", 0f, 0.0001f);
    }
    protected override void Update()
    {
        base.Update();
        ReflectBullets();


    }

    protected override void ResetDefenseBonus()
    {
        base.ResetDefenseBonus();

        EnableReflectDamageEffect(true);
    }
    protected override void DisableDefenseBonusEffect()
    {
        base.DisableDefenseBonusEffect();
        EnableReflectDamageEffect(false);
    }
    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        base.GetDamage(damage, caller, HitSoundName);

        ReflectDamage(damage, caller);
    }

    void ReflectBullets()
    {
        if (!isDefenseBonusEnabled)
            return;
        bullets = GameObject.FindGameObjectsWithTag("Bullet");

        float triggerRange = 0.25f;
        foreach (GameObject bullet in bullets)
        {
            if (!bullet.activeSelf || !isDefenseBonusEnabled)
                return;
            float distance = Vector2.Distance(transform.position, bullet.transform.position);
            if (distance <= triggerRange && bullet && IsBulletAimingAtMe(bullet))
            {
                BulletScript bulletScript = bullet.GetComponent<BulletScript>();
                bulletScript.wayX *= -1;
                //bulletScript.moveSpeed *= 1.25f;
                bulletScript.SetTargetTag(targetTag);
                bulletScript.GetSpriteRenderer().color = Color.white;
            }
        }
    }

    bool IsBulletAimingAtMe(GameObject bullet)
    {
        if (!bullet.activeSelf)
            return false;
        //print("1:" + bullet.GetComponent<BulletScript>().target.GetInstanceID());
        //print("2" + gameObject.GetInstanceID());
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
                caller.GetComponent<HealthBar>().GetDamage(damage, caller, "Classic");
            }
        }
    }

    void EnableReflectDamageEffect(bool val)
    {
        if (!reflectDamageEffect)
            return;
        reflectDamageEffect.SetActive(val);
    }
}
