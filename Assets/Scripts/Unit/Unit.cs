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
    [Tooltip("Only Enemy give money.")]
    public float moneyReward = 1f;
    [Tooltip("Only Enemy give mana.")]
    public float manaReward = 1f;

    public event System.Action onAttack;
    public string deathSfxName;

    protected int wayX = 1;
    protected string targetTag = "Enemy";
    protected float nextAttackTime = 0f;

    ManaBar manaBar;

    GameObject target;
    bool disabled;
    Color originalColor;

    [HideInInspector]
    public bool paralysed;
    public bool Disabled { get => disabled; set => disabled = value; }
    public GameObject Target { get => target; set => target = value; }

    List<IEnumerator> coroutines;

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
        manaBar = ManaBar.instance;
        RandomizeAttackRange();
        coroutines = new List<IEnumerator>();
        FlipUnitSpriteOnWayX();
        originalColor = GetUnitSpriteRenderer().color;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Unsubscribe();
    }

    void Subscribe()
    {
        transform.GetComponent<HealthBar>().OnDeath += Disable;
        transform.GetComponent<HealthBar>().OnDeath += GiveMoneyReward;
        transform.GetComponent<HealthBar>().OnDeath += GiveManaReward;
    }

    void Unsubscribe()
    {
        transform.GetComponent<HealthBar>().OnDeath -= Disable;
        transform.GetComponent<HealthBar>().OnDeath -= GiveMoneyReward;
        transform.GetComponent<HealthBar>().OnDeath -= GiveManaReward;

    }

    void GiveMoneyReward()
    {
        if (gameObject.CompareTag("Enemy"))
            poolObject.stageManager.GivePlayerMoney(moneyReward);
    }

    void GiveManaReward()
    {
        if (gameObject.CompareTag("Enemy"))
            poolObject.manaBar.currentMana += manaReward;
    }



    public void SetTargetTag(string tag)
    {
        targetTag = tag;
    }
    protected bool IsTargetEnabled(GameObject target)
    {
        //return !target || !target.GetComponent<Unit>().Disabled || target.activeSelf;
        return !target || !target.GetComponent<Unit>().Disabled;
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

    SpriteRenderer GetUnitSpriteRenderer()
    {
        string[] spritePaths = { "SpriteBody/Sprite/UnitSprite", "SpriteBody/Sprite/Sprite", "SpriteBody/Sprite" };
        foreach (string path in spritePaths)
        {
            Transform t = transform.Find(path);
            if (!t)
                continue;
            SpriteRenderer spriteRenderer = t.GetComponent<SpriteRenderer>();
            if (spriteRenderer)
                return spriteRenderer;
        }
        return null;
    }

    void PlayDeathSfx()
    {
        if (deathSfxName != "")
            poolObject.audioManager.Play(deathSfxName, true);
    }
    void FlipUnitSpriteOnWayX()
    {
        SpriteRenderer spriteRenderer = GetUnitSpriteRenderer();
        if (spriteRenderer)
        {
            // TODO
            if (targetTag == "Enemy")
                spriteRenderer.flipX = (wayX == 1);
            else
                spriteRenderer.flipX = (wayX == -1);
        }
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
        Subscribe();
    }


    IEnumerator DisableIE()
    {
        disabled = true;
        PlayDeathSfx();
        ResetSpriteColor();
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);


    }
    public virtual void Disable()
    {
        StartCoroutine(DisableIE());
    }

    public void ChangeSpriteColor(Color color)
    {
        GetUnitSpriteRenderer().color = color;
    }
    public void ResetSpriteColor()
    {
        SpriteRenderer spriteRenderer = GetUnitSpriteRenderer();
        if (spriteRenderer)
            spriteRenderer.color = originalColor;
    }

    protected void MoveToward()
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

    void RandomizeAttackRange()
    {
        float randCoef = 6f;
        if (targetTag == "Ally")
            randCoef = 15f;
        attackRange += Random.Range(0f, attackRange / randCoef);
    }

    protected float GetRandomizedNextAttackTime()
    {
        //return Time.time + attackSpeed - Random.Range(0f, attackSpeed / 2f);
        return Time.time + attackSpeed;
    }

    bool IsTargetDisabled()
    {
        return Target.GetComponent<Unit>().Disabled;
    }

    public virtual bool EnoughRangeToAttackTarget()
    {
        if (!Target)
            return false;
        return Vector2.Distance(transform.position, Target.transform.position) <= attackRange;
    }

    bool NextAttackReady()
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
