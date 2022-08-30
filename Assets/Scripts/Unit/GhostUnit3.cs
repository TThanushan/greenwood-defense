using UnityEngine;

public class GhostUnit3 : GhostUnit2
{
    [Header("ConvertEnemyTag")]
    public float timeBetweenEffect = 5f;
    public int convertCount;
    public GameObject triggerEffect;
    float nextEffectTime3;
    int currentConvertCount;
    protected bool nextAttackConvertEnemyTag;



    protected override void Update()
    {
        base.Update();
        if (nextEffectTime3 <= Time.time)
            nextAttackConvertEnemyTag = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        nextEffectTime3 = 0f;
        currentConvertCount = convertCount;
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
        if (target.name == "EnemyCaptain" || target.name.Contains("Ultimate") || target.name.Contains("DummyLog"))
            return;
        target.transform.position = transform.position;
        CreateConvertEffect(triggerEffect, target.transform);
        target.GetComponent<Unit>().SetTargetTag(targetTag);
        target.GetComponent<Unit>().RotateSprite();
        target.tag = tag;
        if (target.GetComponent<ExplodeOnDeath>())
            target.GetComponent<ExplodeOnDeath>().SetTargetTag(targetTag);


        currentConvertCount--;
        if (currentConvertCount <= 0)
        {
            nextAttackConvertEnemyTag = false;
            nextEffectTime3 = Time.time + timeBetweenEffect;
            currentConvertCount = convertCount;
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
