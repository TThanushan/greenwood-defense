using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float attackRange = 2f;

    public float moveSpeed = 1;

    [HideInInspector]
    public float attackDamage = 1;
    [HideInInspector]
    public int wayX = 1;
    //[HideInInspector]
    public GameObject target;

    public GameObject effect;
    string targetTag = "Ally";

    public static System.Action<float> damageEvent;


    void FixedUpdate()
    {
        AttackTarget();
        Move();

    }

    private void Update()
    {
        target = GetClosestEnemy();
    }

    public void SetTargetTag(string tag)
    {
        targetTag = tag;
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

    protected GameObject[] GetEnemies()
    {
        if (targetTag == "Enemy")
            return PoolObject.instance.Enemies;
        else
            return PoolObject.instance.Allies;
    }

    protected void DestroyEffect()
    {
        if (effect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(effect);
            newEffect.transform.position = (transform.position + target.transform.position * 1.25f) / 2;
            RotateObjAwayFrom(newEffect, target);
        }
    }

    protected bool IsTargetInRange()
    {
        Vector2 dir = target.transform.position - transform.position;
        float distanceThisFrame = moveSpeed * Time.deltaTime;
        return dir.magnitude <= (distanceThisFrame * 3);
    }

    protected bool IsTargetInRange(GameObject target)
    {
        Vector2 dir = target.transform.position - transform.position;
        float distanceThisFrame = moveSpeed * Time.deltaTime;
        return dir.magnitude <= (distanceThisFrame * attackRange);
    }

    protected void DamageTarget()
    {
        OnDamageDealt(attackDamage);
        target.GetComponent<HealthBar>().GetDamage(attackDamage);
        print("DAMAGE:" + attackDamage.ToString());
    }

    protected void DamageTarget(GameObject target)
    {
        OnDamageDealt(attackDamage);
        target.GetComponent<HealthBar>().GetDamage(attackDamage);
    }

    public virtual void OnDamageDealt(float damage)
    {
        damageEvent?.Invoke(damage);
    }

    protected void CreateTargetDeathEffect(GameObject deathEffect)
    {
        GameObject effect = PoolObject.instance.GetPoolObject(deathEffect);
        RotateObjAgainst(effect, target);
        effect.transform.position = target.transform.position;
        float randomValue = 0.125f;
        effect.transform.Translate(Random.Range(-randomValue, randomValue), Random.Range(-randomValue, randomValue), 0);
    }

    private void RotateObjAgainst(GameObject obj, GameObject _target)
    {
        if (target.CompareTag("Ally"))
            obj.transform.Rotate(180, 0, 0);
        //Vector3 dir = _target.transform.position - transform.position;
        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //obj.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
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
