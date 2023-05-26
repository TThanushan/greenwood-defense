using UnityEngine;

public class NinjaFrogUnit : Unit
{
    [Header("Teleport")]
    public float timeBetweenEffect = 5f;
    public GameObject dummyLog;
    public float teleportDistance = 1f;
    public GameObject triggerEffect;

    [Header("CloneWhenLowLife")]
    public GameObject clonePrefab;
    [Range(0, 1)]
    public float cloneTriggerThreshold;
    public int cloneCount;

    bool isCloneEffectTriggered;

    float nextEffectTime;

    protected override void Update()
    {
        base.Update();
        if (nextEffectTime <= Time.time && EnoughRangeToAttackTarget())
            DoEffect();

        if (currentHealth / maxHealth <= cloneTriggerThreshold && !isCloneEffectTriggered)
        {
            InstantiateClones();
            isCloneEffectTriggered = true;
            DoEffect();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isCloneEffectTriggered = false;
    }

    void InstantiateClones()
    {
        CreateEffect();

        for (int i = 0; i < cloneCount; i++)
        {
            GameObject newClone = poolObject.GetPoolObject(clonePrefab);
            newClone.transform.position = GetRandomPosition(transform.position, -0.16f, 0.16f, -0.16f, 0.16f);
            Unit unit = newClone.GetComponent<Unit>();
            unit.attackDamage = attackDamage;
            unit.attackSpeed = attackSpeed;
            unit.SetTargetTag(targetTag);
            //unit.FlipUnitSpriteOnWayX();
            if (targetTag == "Enemy")
                unit.RotateSprite();


        }
    }
    Vector2 GetRandomPosition(Vector2 pos, float xRangeA, float xRangeB, float yRangeA, float yRangeB)
    {
        pos.x += Random.Range(xRangeA, xRangeB);
        pos.y += Random.Range(yRangeA, yRangeB);
        return pos;
    }
    void DoEffect()
    {
        if (!Target || Target.name == "PlayerCaptain")
            return;
        CreateEffect();
        //// Spawn log.
        //SpawnDummyLog();

        // Teleport behind.
        TeleportBehind();

        nextEffectTime = Time.time + timeBetweenEffect;
    }

    void TeleportBehind()
    {
        Vector2 pos = transform.position;
        transform.position = new Vector2(pos.x - (teleportDistance * wayX), pos.y);

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
