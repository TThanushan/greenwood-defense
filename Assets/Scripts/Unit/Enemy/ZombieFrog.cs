using UnityEngine;

public class ZombieFrog : Unit
{
    [Range(0, 100)]
    public float attackDamageBonus;
    public float zombieAttackSpeed;
    public GameObject zombieActivationEffect;
    GameObject zombieEffect;

    float attackDamageSave;
    float attackSpeedSave;

    protected override void Start()
    {
        base.Start();
        zombieEffect = transform.Find("SpriteBody/Sprite/ZombieEffect").gameObject;
    }

    void StartZombieEffect()
    {
        currentHealth = maxHealth;
        attackDamageSave = attackDamage;
        attackDamage *= 1 + attackDamageBonus / 100;

        attackSpeedSave = attackSpeed;
        attackSpeed = zombieAttackSpeed;
        zombieEffect.SetActive(true);
        CreateEffect();
    }

    public override void GetDamage(float damage, Transform caller = null)
    {
        if (damage >= currentHealth && !zombieEffect.activeSelf)
            StartZombieEffect();
        base.GetDamage(damage, caller);
    }

    public override void Disable()
    {
        base.Disable();
        attackDamage = attackDamageSave;
        attackSpeed = attackSpeedSave;
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