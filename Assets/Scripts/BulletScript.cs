using UnityEngine;

public class BulletScript : MonoBehaviour
{

    private float attackRange = 2f;

    public float moveSpeed = 1;

    public float attackDamage = 1;
    public int wayX = 1;
    public bool piercingDamage = false;
    public GameObject target;
    public GameObject effect;
    string targetTag = "Ally";
    //PlayerStatsScript playerStats;

    public static System.Action<float> damageEvent;

    void Awake()
    {
        //playerStats = PlayerStatsScript.instance;
    }
    void FixedUpdate()
    {
        //if (playerStats.IsGamePaused)
        //    return;
        AttackTarget();
        Move();

    }

    private void Update()
    {
        GetClosestEnemy();
    }

    private void Move()
    {
        transform.Translate(new Vector2(moveSpeed * wayX * Time.deltaTime, 0));
    }

    protected virtual void AttackTarget()
    {
        if (target == null && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }
        if (IsTargetInRange())
        {
            TargetDestroyEffect();
            DamageTarget();
            DestroyEffect();
            target = null;
            gameObject.SetActive(false);
        }
    }

    private GameObject GetClosestEnemy()
    {
        GameObject[] enemies;
        if ((enemies = GetEnemies()) == null) return null;

        GameObject closestEnemy = null;
        float lowestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            //if (!IsInRange(enemy) || enemy.GetComponent<Unit>().Disabled)
            if (enemy.GetComponent<Unit>().Disabled)
                continue;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    private GameObject[] GetEnemies()
    {
        if (targetTag == "Enemy")
            return PoolObject.instance.Enemies;
        else
            return PoolObject.instance.Allies;
    }
    private void TargetDestroyEffect()
    {
        HealthBar targetProgressBarScript = target.GetComponent<HealthBar>();
        //if (targetProgressBarScript.IsKilled((int)attackDamage) && targetProgressBarScript.deathEffect)
        //    CreateTargetDeathEffect(targetProgressBarScript.deathEffect);
    }

    protected void DestroyEffect()
    {
        if (effect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(effect);
            newEffect.transform.position = transform.position;
            RotateObjAwayFrom(newEffect, target);
        }
    }

    protected bool IsTargetInRange()
    {
        Vector2 dir = target.transform.position - transform.position;
        float distanceThisFrame = moveSpeed * Time.deltaTime;
        return dir.magnitude <= distanceThisFrame;
    }

    private void DamageTarget()
    {
        OnDamageDealt(attackDamage);
        //target.GetComponent<HealthBar>().GetDamage(attackDamage, piercingDamage);
        target.GetComponent<HealthBar>().GetDamage(attackDamage);
    }

    public virtual void OnDamageDealt(float damage)
    {
        damageEvent?.Invoke(damage);
    }

    protected void CreateTargetDeathEffect(GameObject deathEffect)
    {
        GameObject effect = PoolObject.instance.GetPoolObject(deathEffect);
        RotateObjToward(effect, target);
        effect.transform.position = target.transform.position;
        float randomValue = 0.125f;
        effect.transform.Translate(Random.Range(-randomValue, randomValue), Random.Range(-randomValue, randomValue), 0);
    }

    private void RotateObjToward(GameObject obj, GameObject _target)
    {
        Vector3 dir = _target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void RotateObjAwayFrom(GameObject obj, GameObject _target)
    {
        Vector3 dir = _target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
