using UnityEngine;

public class NinjaFrog : Unit
{
    public float timeBetweenEffect = 5f;
    public GameObject dummyLog;
    public float teleportDistance = 1f;
    public GameObject triggerEffect;
    float nextEffectTime;

    protected override void Update()
    {
        base.Update();
        if (nextEffectTime <= Time.time && EnoughRangeToAttackTarget())
            DoEffect();
    }

    void DoEffect()
    {
        if (Target.name == "PlayerCaptain")
            return;
        CreateEffect();
        // Spawn log.
        SpawnDummyLog();

        // Teleport behind.
        TeleportBehind();

        nextEffectTime = Time.time + timeBetweenEffect;
    }

    void TeleportBehind()
    {
        Vector2 pos = transform.position;
        transform.position = new Vector2(pos.x - teleportDistance * wayX, pos.y);

    }

    void SpawnDummyLog()
    {
        if (dummyLog)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(dummyLog);
            newEffect.transform.position = new Vector2(transform.position.x + Random.Range(-0.04f, 0.04f), transform.position.y);
        }
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
