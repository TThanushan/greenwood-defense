using UnityEngine;

public class GhostUnit3 : GhostUnit2
{
    [Header("ConvertEnemyTag")]
    public float timeBetweenEffect = 5f;
    public int convertCount;
    public GameObject triggerEffect;
    float nextEffectTime;
    int currentConvertCount;
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

        currentConvertCount++;
        if (currentConvertCount == convertCount)
        {
            nextAttackConvertEnemyTag = false;
            nextEffectTime = Time.time + timeBetweenEffect;
            currentConvertCount = 0;
        }
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
