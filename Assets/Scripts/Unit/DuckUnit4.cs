using UnityEngine;

public class DuckUnit4 : DuckUnit3
{

    GameObject healingEffect;
    float nextHealingTime;
    public float healingSpeed;
    public float healingPercentage;


    protected override void Start()
    {
        base.Start();
        healingEffect = transform.Find("SpriteBody/HealingEffect").gameObject;
    }
    protected override void Update()
    {
        base.Update();
        if (bigShieldcurrent > 0 && nextHealingTime <= Time.time)
        {
            Heal();
            healingEffect.SetActive(true);
        }
        else if (bigShieldcurrent <= 0)
        {
            healingEffect.SetActive(false);
        }
    }

    void Heal()
    {
        nextHealingTime = healingSpeed + Time.time;
        currentHealth *= 1 + healingPercentage / 100;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}