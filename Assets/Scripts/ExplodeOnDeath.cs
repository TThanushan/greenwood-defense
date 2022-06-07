using UnityEngine;

public class ExplodeOnDeath : MonoBehaviour
{
    public float explosionRange;
    public float explosionDamage;
    public GameObject explosionEffect;

    string targetTag = "Enemy";
    bool once;
    HealthBar healthBar;
    private void OnEnable()
    {
        once = true;
        //transform.GetComponent<HealthBar>().OnDeath += DamageEnemiesAround;
    }
    private void Start()
    {
        healthBar = transform.GetComponent<HealthBar>();
    }
    private void Update()
    {
        if (once && healthBar.currentHealth <= 0)
        {
            once = false;
            DamageEnemiesAround();
        }
    }
    private void DamageEnemiesAround()
    {
        GameObject[] enemies = GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= explosionRange)
            {
                enemy.GetComponent<HealthBar>().GetDamage(explosionDamage, transform);
            }
        }
        CreateEffect();

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
    public void SetTargetTag(string tag)
    {
        targetTag = tag;
    }
    void CreateEffect()
    {
        if (!explosionEffect)
            return;
        GameObject newEgg = PoolObject.instance.GetPoolObject(explosionEffect);
        newEgg.transform.position = transform.position;
    }
    protected GameObject[] GetEnemies()
    {
        if (targetTag == "Enemy")
            return PoolObject.instance.Enemies;
        else
            return PoolObject.instance.Allies;
    }
}
