using UnityEngine;

public class FireskullUnit2 : UnitShooter
{
    [Header("SmallFireBall")]
    public float timeBetweenEffect = 5f;
    public int smallFireBallCount = 5;
    public float smallFireBallDamage = 5;
    public GameObject smallFireBall;
    public GameObject triggerEffect;
    float nextEffectTime;

    protected override void Update()
    {
        if (Disabled)
            return;
        base.Update();
        if (nextEffectTime <= Time.time && EnoughRangeToAttackTarget())
            DoEffect();
    }
    void DoEffect()
    {
        CreateEffect();
        SpawnSmallFireBall();

        nextEffectTime = Time.time + timeBetweenEffect;
    }

    void SpawnSmallFireBall()
    {
        if (!smallFireBall)
            return;
        for (int i = 0; i < smallFireBallCount; i++)
        {
            GameObject newSmallFireBall = PoolObject.instance.GetPoolObject(smallFireBall);
            newSmallFireBall.GetComponent<BulletScript>().SetTargetTag(targetTag);
            newSmallFireBall.GetComponent<BulletScript>().WaitBeforeMove(0.25f);
            newSmallFireBall.GetComponent<BulletScript>().attackDamage = smallFireBallDamage;

            newSmallFireBall.transform.position = GetRandomPosition(transform.position, -0.5f, 0.5f, -0.5f, 0.5f);

        }
    }

    Vector2 GetRandomPosition(Vector2 pos, float xRangeA, float xRangeB, float yRangeA, float yRangeB)
    {
        pos.x += Random.Range(xRangeA, xRangeB);
        pos.y += Random.Range(yRangeA, yRangeB);
        return pos;
    }

    void CreateEffect()
    {
        if (triggerEffect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(triggerEffect);
            newEffect.transform.position = transform.position;
        }
    }
}
