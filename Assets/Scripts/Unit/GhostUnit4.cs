using UnityEngine;

public class GhostUnit4 : GhostUnit3
{
    [Header("LifeStealExplosion")]
    public float timeBetweenEffect4 = 5f;
    [Range(0f, 100f)]
    public float effectDamagePercentage;
    [Range(0f, 100f)]
    public float lifeStealPercentage;
    public float explosionRange = 1f;
    public GameObject triggerEffect4;
    public string effectSFX = "Classic";
    float nextEffectTime;

    protected override void Update()
    {
        base.Update();
        if (nextEffectTime <= Time.time && EnoughRangeToAttackTarget() && !nextAttackConvertEnemyTag)
            DoEffect();
    }

    void DoEffect()
    {
        GameObject[] enemies = GetEnemies();
        float totalDamage = 0f;
        poolObject.audioManager.Play(effectSFX);
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= explosionRange)
            {

                // Take damage based on max health percentage.
                float damageTaken = enemy.GetComponent<Unit>().maxHealth * (effectDamagePercentage / 100f);
                totalDamage += damageTaken;
                enemy.GetComponent<Unit>().GetDamage(damageTaken, transform);
            }
        }
        CreateEffect(triggerEffect4);
        // Heal a percentage of total damage done.
        currentHealth += totalDamage * (lifeStealPercentage / 100);
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        nextEffectTime = Time.time + timeBetweenEffect4;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
