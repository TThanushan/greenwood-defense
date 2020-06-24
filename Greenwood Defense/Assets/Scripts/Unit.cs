using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : HealthBar
{
    public float attackDamage = 1f;
    public float attackSpeed = 1f;
    public float moveSpeed = 1;

    public float attackRange = 1f;
    public float reward = 5f;

    public event System.Action onAttack;

    private int wayX = 1;
    private string targetTag = "Enemy";
    private float nextAttackTime = 0f;
    private PoolObject poolObject;
    private GameObject target;
    private bool disabled;

    public bool Disabled { get => disabled; set => disabled = value; }
    public GameObject Target { get => target; set => target = value; }

    private void Start()
    {
        if (transform.CompareTag("Enemy"))
        {
            targetTag = "Ally";
            wayX = -1;
        }
        poolObject = PoolObject.instance;
        transform.GetComponent<HealthBar>().OnDeath += Disable;
        RandomizeAttackRange();
    }

    protected override void Update()
    {
        if (disabled) return;
        base.Update();

        if (!Target)
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

    private void MoveTowardTarget()
    {
        if (!Target) return;
        Vector2 dir = Target.transform.position - transform.position;
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
    }

    private void AttackTarget()
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
            Target.GetComponent<Unit>().GetDamage(attackDamage);
        }
    }

    private void RandomizeAttackRange()
    {
        attackRange += Random.Range(0f, attackRange / 8f);
    }

    private float GetRandomizedNextAttackTime()
    {
        return Time.time + attackSpeed - Random.Range(0f, attackSpeed / 2f);
    }

    private bool IsTargetDisabled()
    {
        return Target.GetComponent<Unit>().Disabled;
    }

    private bool EnoughRangeToAttackTarget()
    {
        return Vector2.Distance(transform.position, Target.transform.position) <= attackRange;
    }

    private bool NextAttackReady()
    {
        return nextAttackTime <= Time.time;
    }

    private GameObject GetClosestEnemy()
    {
        GameObject[] enemies;
        if ((enemies = GetEnemies()) == null) return null;

        GameObject closestEnemy = null;
        float lowestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            if (!IsInRange(enemy) || enemy.GetComponent<Unit>().Disabled)
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

    private GameObject[] GetEnemies()
    {
        if (targetTag == "Enemy")
            return poolObject.Enemies;
        else
            return poolObject.Allies;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
