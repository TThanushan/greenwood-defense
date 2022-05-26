using System.Collections;
using UnityEngine;

public class Unit : HealthBar
{
    [Header("Attack")]

    public float attackDamage = 1f;
    public float attackSpeed = 1f;
    public float attackRange = 0.24f;

    [Header("Others")]

    public float moveSpeed = 0.5f;
    public float reward = 5f;

    public event System.Action onAttack;

    protected int wayX = 1;
    protected string targetTag = "Enemy";
    protected float nextAttackTime = 0f;
    protected PoolObject poolObject;
    private GameObject target;
    private bool disabled;

    public bool Disabled { get => disabled; set => disabled = value; }
    public GameObject Target { get => target; set => target = value; }

    public override void Awake()
    {
        base.Awake();
        if (transform.CompareTag("Enemy"))
        {
            targetTag = "Ally";
            wayX = -1;
        }

    }
    private void Start()
    {
        poolObject = PoolObject.instance;
        transform.GetComponent<HealthBar>().OnDeath += Disable;
        RandomizeAttackRange();
    }

    protected override void Update()
    {
        if (disabled) return;
        base.Update();

        //if (!Target)
        Target = GetClosestEnemy();
        if (Target)
        {
            if (EnoughRangeToAttackTarget())
                AttackTarget();
            else
                MoveTowardTarget();
        }
        else
            MoveToward();
    }

    private void OnEnable()
    {
        RandomizeAttackRange();
    }

    public virtual void Disable()
    {
        StartCoroutine(DisableIE());
    }

    IEnumerator DisableIE()
    {
        disabled = true;
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    private void MoveToward()
    {
        transform.Translate(new Vector2(moveSpeed * wayX * Time.deltaTime, 0));
    }

    protected virtual void MoveTowardTarget()
    {
        if (!Target) return;
        Vector2 dir = Target.transform.position - transform.position;
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
    }
    public virtual void Attack()
    {
        Target.GetComponent<Unit>().GetDamage(attackDamage);
    }

    protected virtual void AttackTarget()
    {
        if (IsTargetDisabled())
        {
            Target = null;
            return;
        }
        if (NextAttackReady())
        {
            onAttack?.Invoke();
            nextAttackTime = GetRandomizedNextAttackTime();
            Attack();
        }
    }

    protected void InvokeOnAttack()
    {
        onAttack?.Invoke();
    }

    private void RandomizeAttackRange()
    {
        attackRange += Random.Range(0f, attackRange / 8f);
    }

    protected float GetRandomizedNextAttackTime()
    {
        //return Time.time + attackSpeed - Random.Range(0f, attackSpeed / 2f);
        return Time.time + attackSpeed;
    }

    private bool IsTargetDisabled()
    {
        return Target.GetComponent<Unit>().Disabled;
    }

    public virtual bool EnoughRangeToAttackTarget()
    {
        return Vector2.Distance(transform.position, Target.transform.position) <= attackRange;
    }

    private bool NextAttackReady()
    {
        return nextAttackTime <= Time.time;
    }

    protected GameObject GetClosestEnemy()
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

    private bool IsInRange(GameObject enemy)
    {
        float detectionRange = 0.5f;
        return Vector2.Distance(transform.position, enemy.transform.position) <= detectionRange;
    }

    protected GameObject[] GetEnemies()
    {
        if (targetTag == "Enemy")
            return PoolObject.instance.Enemies;
        else
            return PoolObject.instance.Allies;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
