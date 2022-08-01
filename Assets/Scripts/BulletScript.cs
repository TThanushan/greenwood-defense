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
    public SpriteRenderer spriteRenderer;

    public bool moveTowardTarget;
    string targetTag = "Ally";

    public static System.Action<float> damageEvent;
    Color startColor;

    float timeBeforeMove;

    void FixedUpdate()
    {
        if (timeBeforeMove > Time.time)
            return;
        target = GetClosestEnemy();
        AttackTarget();
        if (moveTowardTarget)
            MoveTowardTarget();
        else
            Move();

    }

    public void WaitBeforeMove(float duration)
    {
        timeBeforeMove = duration + Time.time;
    }

    private void OnEnable()
    {
        if (GetSpriteRenderer())
            GetSpriteRenderer().color = startColor;
    }

    private void Awake()
    {
        if (GetSpriteRenderer())
            startColor = GetSpriteRenderer().color;
    }

    void MoveTowardTarget()
    {
        if (!target) return;
        Vector2 dir = target.transform.position - transform.position;
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
    }

    private void Update()
    {
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
        if (IsTargetInRange() && target.GetComponent<Unit>().ProjectileAffectMe())
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
        return dir.magnitude <= (distanceThisFrame * attackRange);
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
        target.GetComponent<HealthBar>().GetDamage(attackDamage, transform, "Classic");
    }

    protected void DamageTarget(GameObject target)
    {
        OnDamageDealt(attackDamage);
        target.GetComponent<HealthBar>().GetDamage(attackDamage, transform, "Classic");
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

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
