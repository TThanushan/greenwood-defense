using UnityEngine;

public class GhostUnit3 : GhostUnit2
{
    [Header("ConvertEnemyTag")]
    public float timeBetweenEffect = 5f;
    public int convertCount;
    public GameObject triggerEffect;
    float nextEffectTime;
    int currentConvertCount;
    protected bool nextAttackConvertEnemyTag;



    protected override void Update()
    {
        base.Update();
        if (nextEffectTime <= Time.time)
            nextAttackConvertEnemyTag = true;
    }

    public override void Attack()
    {
        if (nextAttackConvertEnemyTag)
            ConvertEnemyCamp(Target);
        else
            base.Attack();
    }

    void ConvertEnemyCamp(GameObject target)
    {
        if (target.name == "EnemyCaptain" || target.name.Contains("Ultimate"))
            return;
        target.transform.position = transform.position;
        CreateConvertEffect(triggerEffect, target.transform);
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

    void CreateConvertEffect(GameObject effect, Transform targetConverted)
    {
        if (!effect)
            return;
        GameObject newEffect = PoolObject.instance.GetPoolObject(effect);
        newEffect.transform.position = targetConverted.position;
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
