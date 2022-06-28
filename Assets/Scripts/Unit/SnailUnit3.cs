using UnityEngine;

public class SnailUnit3 : SnailUnit2
{
    [Header("Snail2")]
    [Range(0, 100)]
    public float healingPercentage;

    protected override void DoEffect(GameObject ally)
    {
        base.DoEffect(ally);
        HealAlly(ally, healingPercentage);
    }

    protected void HealAlly(GameObject ally, float _healPercentage)
    {
        HealthBar healthBarScript = ally.GetComponent<HealthBar>();
        healthBarScript.HealMaxHealthPercentage(_healPercentage);
    }

}

