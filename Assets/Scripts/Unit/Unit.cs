﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : HealthBar
{
    [Header("Attack")]

    public float attackDamage = 1f;
    public float attackSpeed = 1f;
    public float attackRange = 0.24f;

    [Header("Others")]

    public float moveSpeed = 0.15f;

    [Header("Death Reward")]
    [Tooltip("Only Enemy give money.")]
    public float moneyReward = 1f;
    [Tooltip("Only Enemy give mana.")]
    public float manaReward = 1f;


    [Header("SFXs")]
    public string deathSfxName;
    public string[] hitSFX;


    public event System.Action OnAttack;
    public bool isNotAUnit;
    protected int wayX = 1;

    protected string targetTag = "Enemy";
    protected float nextAttackTime = 0f;

    protected float initialAttackDamage;
    protected float initialAttackSpeed;
    protected float initialMoveSpeed;

    protected AudioManager audioManager;
    GameObject target;
    bool disabled;
    Color originalColor;
    string initialTag;

    [HideInInspector]
    public bool paralysed;
    public bool Disabled { get => disabled; set => disabled = value; }
    public GameObject Target { get => target; set => target = value; }

    List<IEnumerator> coroutines;
    Quaternion originalSpriteRotation;

    protected override void Awake()
    {

        if (GetSpriteTransform())
            originalSpriteRotation = GetSpriteTransform().localRotation;
        base.Awake();
        InvokeRepeating("UpdateTag", 0f, 0.1f);
        initialTag = tag;
        OnHit += CreateOnHitEffect;
    }

    protected virtual void UpdateTag()
    {
        if (isNotAUnit)
            return;
        if (transform.CompareTag("Enemy"))
        {
            targetTag = "Ally";
            wayX = -1;
        }
        else
        {
            targetTag = "Enemy";
            wayX = 1;

        }
    }

    void CreateOnHitEffect()
    {
        poolObject.GetOnHitEffect().position = GetRandomPosition(transform.position, yRangeA: 0f, yRangeB: -0.25f);
    }

    public void SetInitialAttackDamage(float value)
    {
        initialAttackDamage = value;
    }
    protected override void Update()
    {
        if (disabled) return;
        base.Update();

        if (paralysed)
            return;
        Target = GetClosestEnemyInFrontOfMe();
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
    protected virtual void Start()
    {
        poolObject = PoolObject.instance;
        audioManager = AudioManager.instance;
        RandomizeAttackRange();
        coroutines = new List<IEnumerator>();
        //FlipUnitSpriteOnWayX();
        if (GetUnitSpriteRenderer())
            originalColor = GetUnitSpriteRenderer().color;
        initialAttackDamage = attackDamage;
        initialAttackSpeed = attackSpeed;
        initialMoveSpeed = moveSpeed;
    }


    public virtual bool ProjectileAffectMe()
    {
        return true;
    }

    protected override void OnDisable()
    {
        ResetSpriteRotation();
        base.OnDisable();
        Unsubscribe();
        tag = initialTag;
    }

    public float GetInitialAttackSpeed()
    {
        return initialAttackSpeed;
    }

    //public void SetTargetTagFully(string _tag)
    //{
    //    targetTag = _tag;

    //}


    public Transform GetSpriteTransform()
    {
        return transform.Find("SpriteBody/Sprite").transform;
    }
    public Transform GetSpriteBody()
    {
        return transform.Find("SpriteBody").transform;
    }
    public void RotateSprite()
    {
        Transform spriteTransform = GetSpriteTransform();
        if (spriteTransform.rotation.y != 180f)
            spriteTransform.rotation = Quaternion.Euler(spriteTransform.rotation.x, 180f, spriteTransform.rotation.z);
        else
            spriteTransform.rotation = Quaternion.Euler(spriteTransform.rotation.x, 0f, spriteTransform.rotation.z);
    }
    public void InvokeResetSpriteColor(float time)
    {
        Invoke("ResetSpriteColor", time);
    }

    void Subscribe()
    {
        transform.GetComponent<HealthBar>().OnDeath += Disable;
        transform.GetComponent<HealthBar>().OnDeath += GiveMoneyReward;
        transform.GetComponent<HealthBar>().OnDeath += GiveManaReward;
        transform.GetComponent<HealthBar>().OnDeath += OnDeathEffect;

    }

    void Unsubscribe()
    {
        transform.GetComponent<HealthBar>().OnDeath -= Disable;
        transform.GetComponent<HealthBar>().OnDeath -= GiveMoneyReward;
        transform.GetComponent<HealthBar>().OnDeath -= GiveManaReward;
        transform.GetComponent<HealthBar>().OnDeath -= OnDeathEffect;

    }

    void OnDeathEffect()
    {
        poolObject.GetPoolObject(poolObject.frogOnDeathEffect).transform.position = transform.position;
    }

    public void BuffAttackDamage(float _attackDamage, float duration)
    {
        attackDamage += _attackDamage;
        InvokeResetAttackDamage(duration);
    }

    void ResetAttackDamage()
    {
        attackDamage = initialAttackDamage;
    }

    void ResetAttackSpeed()
    {
        attackSpeed = initialAttackSpeed;
    }

    protected void ChangeMoveSpeedIfUnchanged(float newMoveSpeed)
    {
        if (moveSpeed == initialMoveSpeed)
            moveSpeed = newMoveSpeed;
    }

    void ResetMoveSpeed()
    {
        moveSpeed = initialMoveSpeed;
    }
    public void InvokeResetAttackDamage(float time)
    {
        Invoke("ResetAttackDamage", time);
    }
    public void InvokeResetAttackSpeed(float time)
    {
        Invoke("ResetAttackSpeed", time);
    }
    public void InvokeResetMoveSpeed(float time)
    {
        Invoke("ResetMoveSpeed", time);
    }

    void GiveMoneyReward()
    {
        if (gameObject.CompareTag("Enemy"))
            poolObject.stageManager.GivePlayerMoney(moneyReward);
    }

    void GiveManaReward()
    {
        if (gameObject.CompareTag("Enemy"))
            poolObject.manaBar.GiveMana(manaReward);
    }



    public void SetTargetTag(string tag)
    {
        targetTag = tag;
        if (isNotAUnit)
            return;
        if (targetTag == "Enemy")
            transform.tag = "Ally";
        else
            transform.tag = "Enemy";
    }
    protected bool IsTargetEnabled(GameObject target)
    {
        //return !target || !target.GetComponent<Unit>().Disabled || target.activeSelf;
        return !target || !target.GetComponent<Unit>().Disabled;
    }

    public SpriteRenderer GetUnitSpriteRenderer()
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
        else if (tag.Equals("Enemy"))
            poolObject.audioManager.Play("FrogDeath", true);
    }

    void ResetSpriteRotation()
    {
        Transform spriteTransform = GetSpriteTransform();
        if (spriteTransform)
            spriteTransform.transform.localRotation = originalSpriteRotation;
    }
    public void FlipUnitSpriteOnWayX()
    {
        SpriteRenderer spriteRenderer = GetUnitSpriteRenderer();
        if (spriteRenderer)
        {
            // TODO
            if (targetTag == "Enemy" && CompareTag("Enemy"))
                spriteRenderer.flipX = wayX == 1;
            else
                spriteRenderer.flipX = wayX == -1;
        }
    }

    public void AddCoroutinesToList(IEnumerator enumerator)
    {
        coroutines.Add(enumerator);
    }
    protected void CreateEffect(GameObject effect)
    {
        if (!effect)
            return;
        GameObject newEffect = PoolObject.instance.GetPoolObject(effect);
        newEffect.transform.position = transform.position;
    }
    public void ParalyseEffect(bool isParalyse)
    {
        paralysed = isParalyse;
        transform.GetComponent<Animator>().enabled = !isParalyse;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetSpriteRotation();

        RandomizeAttackRange();
        Disabled = false;
        Subscribe();
    }



    IEnumerator DisableIE()
    {
        disabled = true;
        PlayDeathSfx();
        ResetSpriteColor();
        if (GetComponent<Animator>())
            GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);


    }
    public virtual void Disable()
    {
        if (GetComponent<Animator>())
            StartCoroutine(DisableIE());
        else
        {
            disabled = true;
            PlayDeathSfx();
            ResetSpriteColor();
            gameObject.SetActive(false);
        }
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

    protected virtual void MoveToward()
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
        //PlayHitSfx();
        Target.GetComponent<Unit>().GetDamage(attackDamage, transform);
    }

    protected void PlayHitSfx(int index = -1)
    {
        string name = "";
        if (index != -1)
            name = hitSFX[index];
        else if (hitSFX.Length > 0)
            name = hitSFX[Random.Range(0, hitSFX.Length)];

        audioManager.Play(name, true);
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
            InvokeOnAttack();
            nextAttackTime = GetRandomizedNextAttackTime();
            Attack();
        }
    }

    protected void InvokeOnAttack()
    {
        OnAttack?.Invoke();
    }

    void RandomizeAttackRange()
    {
        if (targetTag == "Enemy")
            attackRange += Random.Range(0f, 0.05f);
    }

    protected virtual float GetRandomizedNextAttackTime()
    {
        //return Time.time + attackSpeed - Random.Range(0f, attackSpeed / 2f);
        return Time.time + attackSpeed;
    }

    protected bool IsTargetDisabled()
    {
        return Target.GetComponent<Unit>().Disabled;
    }

    public virtual bool EnoughRangeToAttackTarget()
    {
        if (!Target)
            return false;
        return Vector2.Distance(transform.position, Target.transform.position) <= attackRange;
    }

    protected bool EnoughRangeToAttackTarget(float range)
    {
        if (!Target)
            return false;
        return Vector2.Distance(transform.position, Target.transform.position) <= range;

    }

    protected bool NextAttackReady()
    {
        return nextAttackTime <= Time.time;
    }

    public GameObject GetClosestEnemy()
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

    virtual protected GameObject GetClosestEnemyInFrontOfMe()
    {
        GameObject[] enemies;
        if ((enemies = GetEnemies()) == null) return null;

        GameObject closestEnemy = null;
        float lowestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            //if (!IsInRange(enemy) || enemy.GetComponent<Unit>().Disabled)
            if (enemy.GetComponent<Unit>().Disabled || !IsEnemyBehindMeOnXAxis(enemy.transform))
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



    protected virtual bool IsEnemyBehindMeOnXAxis(Transform target)
    {
        if (wayX == -1)
            return transform.position.x > target.transform.position.x;
        return transform.position.x < target.transform.position.x;
    }


    protected GameObject[] GetEnemies()
    {
        if (targetTag == "Enemy")
            return PoolObject.instance.Enemies;
        else
            return PoolObject.instance.Allies;
    }

    protected GameObject[] GetAllies()
    {
        if (targetTag == "Ally")
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
