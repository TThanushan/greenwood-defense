using UnityEngine;

public class UnitHealer : Unit
{
    public float healingAmount;
    public float timeBetweenHeal;
    public float healRange;
    float nextHealTime;

    protected override void Update()
    {
        base.Update();
        if (nextHealTime <= Time.time)
            HealAllies();
    }

    void HealAllies()
    {

        GameObject[] allies = GetAllies();
        if (allies == null || allies.Length == 0)
            return;

        foreach (GameObject ally in allies)
        {
            float distance = Vector2.Distance(transform.position, ally.transform.position);
            if (distance <= healRange)
            {
                DoEffect(ally);

            }
        }

        nextHealTime = Time.time + timeBetweenHeal;
    }

    void DoEffect(GameObject ally)
    {
        HealthBar healthBarScript = ally.GetComponent<HealthBar>();
        healthBarScript.Heal(healingAmount);
    }
}
