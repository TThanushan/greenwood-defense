using UnityEngine;

public class EggBomb : MonoBehaviour
{
    public float timeBeforeExplosion;
    public float explosionRange;
    public float explosionDamage;
    public GameObject explosionEffect;
    public string explosionSFX;

    float currentTimeBeforeExplosion;
    string targetTag = "Enemy";
    private void Update()
    {
        if (currentTimeBeforeExplosion <= Time.time)
            DamageEnemiesAround();
    }
    private void OnEnable()
    {
        currentTimeBeforeExplosion = Time.time + timeBeforeExplosion;

    }

    private void DamageEnemiesAround()
    {
        GameObject[] enemies = GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= explosionRange)
            {
                DamageTarget(enemy);
            }
        }
        CreateEffect();
        gameObject.SetActive(false);
    }

    protected void DamageTarget(GameObject target)
    {
        target.GetComponent<HealthBar>().GetDamage(explosionDamage, transform, explosionSFX);
    }


    void CreateEffect()
    {
        if (!explosionEffect)
            return;
        GameObject newEgg = PoolObject.instance.GetPoolObject(explosionEffect);
        newEgg.transform.position = transform.position;
    }
    public void SetTargetTag(string tag)
    {
        targetTag = tag;
    }
    protected GameObject[] GetEnemies()
    {
        return targetTag == "Enemy" ? PoolObject.instance.GetEnemiesAsArray() : PoolObject.instance.GetAlliesAsArray();
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
