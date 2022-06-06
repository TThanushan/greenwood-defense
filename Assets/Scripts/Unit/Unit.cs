using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : HealthBar
{
    [Header("Attack")]

    public float attackDamage = 1f;
    public float attackSpeed = 1f;
    public float attackRange = 0.24f;

    [Header("Others")]

    public float moveSpeed = 0.5f;
    //public float reward = 5f;

    public event System.Action onAttack;

    protected int wayX = 1;
    protected string targetTag = "Enemy";
    protected float nextAttackTime = 0f;
    protected PoolObject poolObject;
    private GameObject target;
    private bool disabled;

    [HideInInspector]
    public bool paralysed;
    public bool Disabled { get => disabled; set => disabled = value; }
    public GameObject Target { get => target; set => target = value; }

    private List<IEnumerator> coroutines;

    protected override void Awake()
    {
        base.Awake();
        if (transform.CompareTag("Enemy"))
        {
            targetTag = "Ally";
            wayX = -1;
        }

    }
    protected virtual void Start()
    {
        poolObject = PoolObject.instance;
        transform.GetComponent<HealthBar>().OnDeath += Disable;
        RandomizeAttackRange();
        coroutines = new List<IEnumerator>();
    }

    private void Reset()
    {

    }
    protected bool IsTargetEnabled(GameObject target)
    {
        return !target || !target.GetComponent<Unit>().Disabled || target.activeSelf;
    }
    protected override void Update()
    {
        if (disabled) return;
        base.Update();

        if (paralysed)
            return;
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

    public void AddCoroutinesToList(IEnumerator enumerator)
    {
        coroutines.Add(enumerator);
    }

    public void ParalyseEffect(bool isParalyse)
    {
        paralysed = isParalyse;
        transform.GetComponent<Animator>().enabled = !isParalyse;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        RandomizeAttackRange();
        Disabled = false;
    }

    private void OnDisable()
    {
        //StopAllCoroutines();
    }
    IEnumerator DisableIE()
    {
        disabled = true;
        ResetSpriteColor();
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);

    }
    public virtual void Disable()
    {
        StartCoroutine(DisableIE());
    }


    void ResetSpriteColor()
    {
        string[] spritePaths = { "SpriteBody/Sprite/UnitSprite", "SpriteBody/Sprite/Sprite" };
        foreach (string path in spritePaths)
        {
            Transform spriteT = transform.Find(path);
            if (!spriteT)
                continue;
            spriteT.GetComponent<SpriteRenderer>().color = Color.white;
        }
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
        if (!Target)
            return;
        Target.GetComponent<Unit>().GetDamage(attackDamage, transform);
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
