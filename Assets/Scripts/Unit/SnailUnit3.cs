using UnityEngine;

public class SnailUnit3 : SnailUnit2
{
    [Range(0, 100)]
    public float healingPercentage;

    protected override void DoEffect(GameObject ally)
    {
        base.DoEffect(ally);
        HealAlly(ally);
    }

    void HealAlly(GameObject ally)
    {
        HealthBar healthBarScript = ally.GetComponent<HealthBar>();
        healthBarScript.HealMaxHealthPercentage(healingPercentage);
    }

}

