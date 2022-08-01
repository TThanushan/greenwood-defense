using UnityEngine;

public class Fireskull3 : Fireskull2
{
    [Header("Meteor")]
    public float timeBetweenEffect3 = 5f;
    public GameObject meteor;
    public GameObject triggerEffect3;
    public float meteorDamage;
    public Transform meteorSpawnPos;
    float nextEffectTime3;


    protected override void Update()
    {
        if (Disabled)
            return;
        base.Update();
        if (nextEffectTime3 <= Time.time && EnoughRangeToAttackTarget())
            DoEffect();
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
