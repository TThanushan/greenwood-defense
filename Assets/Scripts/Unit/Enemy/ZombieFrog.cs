using UnityEngine;

public class ZombieFrog : Unit
{
    [Range(0, 100)]
    public float attackDamageBonus;
    public float zombieAttackSpeed;
    public float zombieMoveSpeed;
    public GameObject zombieActivationEffect;
    GameObject zombieEffect;

    float attackDamageSave;
    float attackSpeedSave;
    float moveSpeedSave;

    protected override void Start()
    {
        base.Start();
        zombieEffect = transform.Find("SpriteBody/Sprite/ZombieEffect").gameObject;
    }

    void StartZombieEffect()
    {
        CreateEffect();
        currentHealth = maxHealth;
        attackDamageSave = attackDamage;
        attackDamage *= 1 + attackDamageBonus / 100;

        attackSpeedSave = attackSpeed;
        attackSpeed = zombieAttackSpeed;
        moveSpeedSave = moveSpeed;
        moveSpeed = zombieMoveSpeed;
        zombieEffect.SetActive(true);
        nextAttackTime = 0f;
    }

    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        if (damage >= currentHealth && !zombieEffect.activeSelf && !Disabled)
            StartZombieEffect();
        base.GetDamage(damage, caller, HitSoundName);
    }

    public override void Disable()
    {
        base.Disable();
        attackDamage = attackDamageSave;
        attackSpeed = attackSpeedSave;
        moveSpeed = moveSpeedSave;
        zombieEffect.SetActive(false);
    }

    void CreateEffect()
    {
        if (!zombieActivationEffect)
            return;
        GameObject newEgg = PoolObject.instance.GetPoolObject(zombieActivationEffect);
        newEgg.transform.position = transform.position;
    }


}