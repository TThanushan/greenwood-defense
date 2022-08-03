using UnityEngine;

public class GhostUnit3 : GhostUnit2
{
    [Header("ConvertEnemyTag")]
    public float timeBetweenEffect = 5f;
    public GameObject triggerEffect;
    float nextEffectTime;
    bool nextAttackConvertEnemyTag;

    protected override void Update()
    {
        base.Update();
        if (nextEffectTime <= Time.time)
            nextAttackConvertEnemyTag = true;
    }

    public override void Attack()
    {
        base.Attack();
        if (nextAttackConvertEnemyTag)
            ConvertEnemyCamp(Target);
    }

    void ConvertEnemyCamp(GameObject target)
    {
        if (target.name == "EnemyCaptain")
            return;
        CreateEffect(triggerEffect);
        target.GetComponent<Unit>().SetTargetTag(targetTag);
        target.GetComponent<Unit>().RotateSprite();
        target.tag = tag;

        nextAttackConvertEnemyTag = false;
        nextEffectTime = Time.time + timeBetweenEffect;
    }
    //protected virtual void CreateEffect(GameObject effect)
    //{
    //    if (effect)
    //    {
    //        GameObject newEffect = PoolObject.instance.GetPoolObject(effect);
    //        newEffect.transform.position = transform.position;
    //    }
    //}
}
