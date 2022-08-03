using UnityEngine;

public class FireBreatherFrogUnit : ShotgunFrogUnit
{
    [Header("FireBreath")]
    public float fireBreathDuration;
    public float timeBetweenFireBreath;

    [Header("Meteor")]
    public float timeBetweenEffect3 = 5f;
    public GameObject meteor;
    public GameObject triggerEffect3;
    public float meteorDamage;
    public float magmaDamage;
    public Transform meteorSpawnPos;
    float nextEffectTime3;

    float fireBreathCooldown;
    bool isFireBreathing;
    protected override void Shoot(GameObject target)
    {
        if (fireBreathCooldown <= Time.time)
        {
            base.Shoot(target);

            if (!isFireBreathing)
            {
                isFireBreathing = true;
                Invoke(nameof(DisableFireBreath), fireBreathDuration);
            }
        }

    }
    protected override void Update()
    {
        if (Disabled)
            return;
        base.Update();
        if (nextEffectTime3 <= Time.time && EnoughRangeToAttackTarget())
            DoEffect();
    }
    void DisableFireBreath()
    {
        fireBreathCooldown = Time.time + timeBetweenFireBreath;
        isFireBreathing = false;
    }

    void DoEffect()
    {
        CreateEffect();
        SpawnMeteor();

        nextEffectTime3 = Time.time + timeBetweenEffect3;
    }

    void SpawnMeteor()
    {
        if (!meteor)
            return;
        GameObject newMeteor = PoolObject.instance.GetPoolObject(meteor);
        newMeteor.GetComponent<BulletScript>().SetTargetTag(targetTag);
        newMeteor.GetComponent<BulletScript>().attackDamage = meteorDamage;
        newMeteor.GetComponent<MeteorMagma>().magmaDamage = magmaDamage;

        //newMeteor.GetComponent<BulletScript>().WaitBeforeMove(0.25f);
        newMeteor.transform.position = meteorSpawnPos.position;
    }

    void CreateEffect()
    {
        if (triggerEffect3)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(triggerEffect3);
            newEffect.transform.position = transform.position;
        }
    }
}