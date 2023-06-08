using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCaptainUnit : Unit
{
    public enum Weapon { Sword, Crossbow };

    public GameObject phoenixEffect;
    public Weapon weapon = Weapon.Sword;

    [Header("Sword")]
    public float swordRange = 1;
    public float swordDamage = 1;
    public float swordAttackSpeed = 1;
    [Range(0, 1)]
    public float swordCriticalChance = 1;
    public GameObject criticalAttackEffect;
    public GameObject criticalText;

    [Header("SwordAbilitySlash")]
    public float swordAbilitySlashRange;
    public float swordAbilitySlashDamage;
    public string swordAbilitySlashSoundName;
    public float swordAbilitySlashCooldown;
    float nextSwordAbilitySlash;
    public GameObject swordAbilityEffect;
    public Transform swordAbilityEffectPosition;

    [Header("CrossbowAbilityPiercingArrow")]
    //public float crossbowAbilityPiercingArrowRange;
    public float crossbowAbilityPiercingArrowDamage;
    public string crossbowAbilityPiercingArrowSoundName;
    public float crossbowAbilityPiercingArrowCooldown;
    public GameObject piercingArrow;
    float nextCrossbowAbilityPiercingArrow;

    [Header("Crossbow")]
    public GameObject bullet;
    public Transform bulletPosition;
    public float crossbowRange = 1;
    public float crossbowDamage = 1;
    public float crossbowAttackSpeed = 1;
    public bool isCrossbowUnlocked;

    public GameObject walkingDustEffect;
    public Transform walkingDustPosition;

    GameObject swordWeaponAbilityCooldownEffectBar;
    GameObject swordWeaponAbilityCooldownGlow;
    GameObject crossbowWeaponAbilityCooldownEffectBar;
    GameObject crossbowWeaponAbilityCooldownGlow;

    PlayerWalkingSfx playerWalkingSfx;
    Transform sprite;
    Animator animator;
    bool isPhoenix;
    CameraController shakeCamera;

    protected override void Start()
    {

        base.Start();
        Init();
        foreach (string name in SaveManager.instance.unlockedHeroUpgrades)
        {
            if (name == "Phoenix1")
            {
                isPhoenix = true;
                transform.Find("SpriteBody/PhoenixEnabled").gameObject.SetActive(true);
            }
        }
        Invoke(nameof(SetStats), 0.1f);
        InvokeRepeating(nameof(PassiveHealing), 0f, 1f);

    }

    protected override void Update()
    {
        base.Update();
        PlayerControl();
        SwitchAutomaticallyWeapon();
        UpdateWeaponSprite();
        UpdateWeaponAbilityCooldownBar();
    }
    void Init()
    {
        OnAttack += PlayAttackAnimation;
        sprite = transform.GetComponent<Unit>().GetSpriteTransform().parent;
        animator = GetComponent<Animator>();

        swordWeaponAbilityCooldownEffectBar = transform.Find("SwordEffectBar/Canvas/Bar").gameObject;
        swordWeaponAbilityCooldownGlow = transform.Find("SwordEffectBar/Canvas/Glow").gameObject;

        crossbowWeaponAbilityCooldownGlow = transform.Find("CrossbowEffectBar/Canvas/Glow").gameObject;
        crossbowWeaponAbilityCooldownEffectBar = transform.Find("CrossbowEffectBar/Canvas/Bar").gameObject;

        nextSwordAbilitySlash = Time.time + swordAbilitySlashCooldown;
        nextCrossbowAbilityPiercingArrow = Time.time + crossbowAbilityPiercingArrowCooldown;
        playerWalkingSfx = GetComponent<PlayerWalkingSfx>();
        shakeCamera = CameraController.instance;

        OnDeath += PlayDeathAnimation;
    }

    void PlayDeathAnimation()
    {
        //GetComponent<Animator>().Play("Death");
        GetSpriteBody().gameObject.SetActive(false);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GetSpriteBody().gameObject.SetActive(true);
    }

    void PassiveHealing()
    {
        if (currentHealth > 0)
            HealMaxHealthPercentage(2);
    }


    void Test()
    {
        currentHealth = 10;
        maxHealth = 10;
    }

    void SetStats()
    {
        //SetWeapon(Weapon.Sword);
        //SetWeapon(Weapon.Crossbow);
        attackSpeed = swordAttackSpeed;
        attackDamage = swordDamage;
        attackRange = swordRange;
        InitWeaponStats();
        SwitchAutomaticallyWeapon();
        crossbowWeaponAbilityCooldownEffectBar.transform.parent.parent.gameObject.SetActive(isCrossbowUnlocked);
        Test();
    }

    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {

        if (isPhoenix && damage >= currentHealth)
        {
            TriggerPhoenix();
            return;
        }

        base.GetDamage(damage, caller, HitSoundName);

    }

    void TriggerPhoenix()
    {
        isPhoenix = false;
        currentHealth = maxHealth;
        SaveManager.instance.unlockedHeroUpgrades.Remove("Phoenix1");
        SaveManager.instance.unlockedHeroUpgrades.Add("Phoenix0");
        poolObject.GetPoolObject(phoenixEffect).transform.position = transform.position;
        transform.Find("SpriteBody/PhoenixEnabled").gameObject.SetActive(false);
    }



    void UpdateWeaponAbilityCooldownBar()
    {
        UpdateEffectBarLength(swordWeaponAbilityCooldownEffectBar, swordWeaponAbilityCooldownGlow, nextSwordAbilitySlash, swordAbilitySlashCooldown);
        UpdateEffectBarLength(crossbowWeaponAbilityCooldownEffectBar, crossbowWeaponAbilityCooldownGlow, nextCrossbowAbilityPiercingArrow, crossbowAbilityPiercingArrowCooldown);
    }

    private void UpdateEffectBarLength(GameObject bar, GameObject glow, float nextCooldown, float cooldown)
    {
        //bar.transform.localScale = new Vector3(GetNewBarLength(nextCooldown, cooldown), bar.transform.localScale.y, bar.transform.localScale.z);
        var fillAmount = GetNewBarLength(nextCooldown, cooldown);
        bar.GetComponent<Image>().fillAmount = fillAmount;
        if (fillAmount >= 1)
            glow.SetActive(true);
        else
            glow.SetActive(false);
    }

    private float GetNewBarLength(float nextCooldown, float cooldown)
    {
        float barLength = 1 - (Mathf.Abs(nextCooldown - Time.time) / cooldown);
        if (nextCooldown < Time.time)
            barLength = 1;
        else if (barLength < 0) barLength = 0;
        return barLength;
    }
    private bool canCreateDust = true;

    void PlayerControl()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
            UseSwordAbilitySlash();

        if (Input.GetKeyDown(KeyCode.Mouse1))
            UseCrossbowAbilityPiercingArrow();
        if (Input.GetKeyDown(KeyCode.A))
        {
            wayX = -1;
            transform.Find("SpriteBody/Sprite/UnitSprite").rotation = new Quaternion(0, 180f, 0, 0);
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            wayX = 1;
            transform.Find("SpriteBody/Sprite/UnitSprite").rotation = new Quaternion(0, 0, 0, 0);
        }


        if (Input.GetKey(KeyCode.A))
        {

            animator.SetBool("IsRunning", true);
            MoveFoward();

            playerWalkingSfx.PlayWalkingSound();
            //if (canCreateDust)
            //    StartCoroutine(CreateDustEffectWithDelay(0.20f)); // Adjust the delay to control the frequency
        }
        else if (Input.GetKey(KeyCode.D) && !IsTooCloseToTargetToMove())
        {
            animator.SetBool("IsRunning", true);
            MoveFoward();
            playerWalkingSfx.PlayWalkingSound();


            //if (canCreateDust)
            //    StartCoroutine(CreateDustEffectWithDelay(0.20f)); // Adjust the delay to control the frequency
        }
        else
            animator.SetBool("IsRunning", false);
    }


    void CreateWalkingDustEffect()
    {
        GameObject dust = poolObject.GetPoolObject(walkingDustEffect);
        dust.transform.position = walkingDustPosition.position;
        Vector3 dustRotation = dust.transform.localScale;
        dust.transform.localScale = new Vector3(dustRotation.x * wayX, dustRotation.y, dustRotation.z);
    }

    IEnumerator CreateDustEffectWithDelay(float delay)
    {
        canCreateDust = false;
        yield return new WaitForSeconds(delay);
        CreateWalkingDustEffect();
        canCreateDust = true;
    }


    bool IsTooCloseToTargetToMove()
    {
        if (!Target)
            return false;
        return Vector2.Distance(transform.position, Target.transform.position) < 0.25f;

    }

    void UseSwordAbilitySlash()
    {
        if (!EnoughRangeToAttackTarget(swordRange) || nextSwordAbilitySlash > Time.time)
            return;

        DamageEnemiesAroundTarget();
        audioManager.Play(swordAbilitySlashSoundName);
        //PlayAttackAnimation();
        GetComponent<Animator>().Play("SwordAbilitySlash");
        nextSwordAbilitySlash = Time.time + swordAbilitySlashCooldown;

        poolObject.GetPoolObject(swordAbilityEffect).transform.position = swordAbilityEffectPosition.position;

    }

    void UseCrossbowAbilityPiercingArrow()
    {
        if (weapon != Weapon.Crossbow || nextCrossbowAbilityPiercingArrow > Time.time)
            return;
        //DamageEnemiesAroundTarget();
        ShootPiercingArrow(Target);
        audioManager.Play(crossbowAbilityPiercingArrowSoundName);
        //PlayAttackAnimation();
        //GetComponent<Animator>().Play("CrossbowAbilityPiercingArrow");
        nextCrossbowAbilityPiercingArrow = Time.time + crossbowAbilityPiercingArrowCooldown;
    }

    void DamageEnemiesAroundTarget()
    {
        GameObject[] enemies = GetEnemies();
        //poolObject.audioManager.Play(damageSFXName);
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(Target.transform.position, enemy.transform.position);
            if (distance <= swordAbilitySlashRange)
                enemy.GetComponent<Unit>().GetDamage(swordAbilitySlashDamage, transform);
        }
    }

    bool IsAttackCritical(float criticalChance)
    {
        float chance = Random.Range(0f, 1f);
        return chance <= criticalChance;
    }


    float GetCriticalDamage(float damage)
    {
        return damage * 3;
    }

    public override void Attack()
    {
        if (!Target)
            return;
        if (weapon == Weapon.Sword)
        {
            float damageDealt = attackDamage;
            bool isCritical = IsAttackCritical(swordCriticalChance);
            if (isCritical)
            {
                damageDealt = GetCriticalDamage(damageDealt);
                //poolObject.GetPoolObject(criticalAttackEffect).transform.position = GetRandomPosition(Target.transform.position, yRangeA: 0f, yRangeB: -0.25f);
                poolObject.GetPoolObject(criticalAttackEffect).transform.position = transform.position;

            }
            PlayAttackSfx(isCritical);

            Target.GetComponent<Unit>().GetDamage(damageDealt, transform);
        }
        else if (weapon == Weapon.Crossbow)
            Shoot(Target);
    }



    void PlayAttackSfx(bool isCritical)
    {
        if (isCritical)
        {
            string sfxName = "SwordCriticalAttack" + Random.Range(1, 3).ToString();
            audioManager.Play(sfxName, volume: 0.75f);
            //poolObject.DisplayText("CRIT", 1).transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 0.30f, Target.transform.position.z);
            poolObject.GetPoolObject(criticalText).transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 0.30f, Target.transform.position.z);
            //shakeCamera.ShakeWithDelay(0.15f, 0.01f, 0.25f);

        }
        else
        {

            string name = "EarthPunch" + Random.Range(3, 6).ToString();
            audioManager.Play(name);
            name = "SwordAttack" + Random.Range(1, 6).ToString();
            audioManager.Play(name);
        }
    }

    void SwitchAutomaticallyWeapon()
    {
        if (EnoughRangeToAttackTarget(swordRange) && weapon != Weapon.Sword)
            SetWeapon(Weapon.Sword);
        else if (isCrossbowUnlocked && weapon != Weapon.Crossbow && !EnoughRangeToAttackTarget(swordRange))
            SetWeapon(Weapon.Crossbow);
    }

    void UpdateWeaponSprite()
    {
        string spriteLocation = "SpriteBody/Sprite/";
        string swordInBackLocation = spriteLocation + "SwordInBack";
        string crossbowInBackLocation = spriteLocation + "CrossbowInBack";
        if (wayX == -1)
        {
            //transform.Find(spriteLocation + Weapon.Crossbow.ToString()).gameObject.SetActive(false);
            transform.Find(spriteLocation + Weapon.Crossbow.ToString()).eulerAngles = new Vector3(0, 180f, 0);
            transform.Find(spriteLocation + Weapon.Crossbow.ToString()).localPosition = new Vector3(-0.104000002f, -0.0890000015f, 0);
            transform.Find(spriteLocation + Weapon.Sword.ToString()).gameObject.SetActive(false);
            transform.Find(swordInBackLocation).eulerAngles = new Vector3(0f, 180f, -176f);
            transform.Find(swordInBackLocation).gameObject.SetActive(true);

            return;
        }
        transform.Find(spriteLocation + Weapon.Crossbow.ToString()).eulerAngles = Vector3.zero;
        transform.Find(spriteLocation + Weapon.Crossbow.ToString()).localPosition = new Vector3(0.107000001f, -0.0879999995f, 0);


        if (weapon == Weapon.Sword)
        {
            if (EnoughRangeToAttackTarget())
            {
                transform.Find(spriteLocation + Weapon.Sword.ToString()).gameObject.SetActive(true);
                transform.Find(spriteLocation + Weapon.Crossbow.ToString()).gameObject.SetActive(false);
                transform.Find(swordInBackLocation).gameObject.SetActive(false);
            }
            else
            {
                transform.Find(swordInBackLocation).eulerAngles = new Vector3(0, 0f, -176f);
                transform.Find(swordInBackLocation).gameObject.SetActive(true);
                transform.Find(spriteLocation + Weapon.Sword.ToString()).gameObject.SetActive(false);
                transform.Find(spriteLocation + Weapon.Crossbow.ToString()).gameObject.SetActive(false);
            }

        }
        else if (weapon == Weapon.Crossbow)
        {

            transform.Find(spriteLocation + Weapon.Sword.ToString()).gameObject.SetActive(false);
            transform.Find(spriteLocation + Weapon.Crossbow.ToString()).gameObject.SetActive(true);
        }


    }

    public override bool EnoughRangeToAttackTarget()
    {
        if (weapon == Weapon.Sword)
            return EnoughRangeToAttackTarget(swordRange);
        else if (weapon == Weapon.Crossbow)
            return EnoughRangeToAttackTarget(crossbowRange);
        return false;
    }

    void PlayAttackAnimation()
    {
        string name = weapon.ToString() + "Attack";
        GetComponent<Animator>().Play(name);
    }

    void Shoot(GameObject target)
    {
        PlayHitSfx(1);
        InstantiateBullet(target);
    }

    void ShootPiercingArrow(GameObject target)
    {
        GameObject newBullet = poolObject.GetPoolObject(piercingArrow);
        BulletScript newBulletScript = newBullet.GetComponent<BulletScript>();

        newBullet.transform.position = bulletPosition.transform.position;
        newBulletScript.target = target;
        newBulletScript.SetTargetTag(targetTag);
        newBulletScript.attackDamage = crossbowAbilityPiercingArrowDamage;
    }

    protected GameObject InstantiateBullet(GameObject target, GameObject bullet = null)
    {
        if (bullet == null)
            bullet = this.bullet;
        if (!bullet || !target)
            return null;
        GameObject newBullet = poolObject.GetPoolObject(bullet);
        newBullet.transform.position = bulletPosition.transform.position;

        BulletScript newBulletScript = newBullet.GetComponent<BulletScript>();
        newBulletScript.target = target;
        newBulletScript.SetTargetTag(targetTag);
        newBulletScript.attackDamage = attackDamage;


        return newBullet;
    }

    void MoveFoward()
    {
        transform.Translate(new Vector2(moveSpeed * wayX * Time.deltaTime, 0));
    }
    protected override void MoveTowardTarget()
    {
        //Disabling
    }
    protected override void UpdateTag()
    {
        //Disabling
    }

    protected override void MoveToward()
    {
        //Disabling
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
        //Transform spriteTransform = GetSpriteTransform();

        //string[] WeaponNames = System.Enum.GetNames(typeof(Weapon));

        //foreach (string weaponName in WeaponNames)
        //{
        //    spriteTransform.Find(weaponName).gameObject.SetActive(false);
        //}
        //spriteTransform.Find(weapon.ToString()).gameObject.SetActive(true);

        InitWeaponStats();
        UpdateWeaponSprite();
    }

    void InitWeaponStats()
    {
        if (weapon == Weapon.Sword)
        {
            attackSpeed = swordAttackSpeed;
            attackDamage = swordDamage;
            attackRange = swordRange;
        }
        else if (weapon == Weapon.Crossbow)
        {
            attackSpeed = crossbowAttackSpeed;
            attackDamage = crossbowDamage;
            attackRange = crossbowRange;
        }
    }
}
