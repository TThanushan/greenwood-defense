using UnityEngine;

public class TeleportFrogUnit : Unit
{
    public float timeBetweenEffect = 5f;
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

        if (Target.name == "PlayerCaptain" || Target.name == "EnemyCaptain")
            return;
        CreateEffect();

        // Teleport behind.
        TeleportBehind();

        nextEffectTime = Time.time + timeBetweenEffect;
    }

    void TeleportBehind()
    {
        Vector2 pos = transform.position;
        transform.position = new Vector2(pos.x + teleportDistance * wayX, pos.y);

        if (transform.position.x < poolObject.playerCaptain.transform.position.x)
            transform.position = new Vector2(poolObject.playerCaptain.transform.position.x + 0.15f, transform.position.y);
        if (transform.position.x > poolObject.enemyCaptain.transform.position.x)
            transform.position = new Vector2(poolObject.enemyCaptain.transform.position.x - 0.15f, transform.position.y);
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
