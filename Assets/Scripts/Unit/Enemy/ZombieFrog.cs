using UnityEngine;

public class ZombieFrog : Unit
{
    [Range(0, 100)]
    public float attackDamageBonus;
    public float zombieAttackSpeed;

    GameObject zombieEffect;

    float attackDamageSave;
    float attackSpeedSave;

    protected override void Start()
    {
        base.Start();
        OnDeath += StartZombieEffect;
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

    protected override void Update()
    {
        base.Update();
    }
}