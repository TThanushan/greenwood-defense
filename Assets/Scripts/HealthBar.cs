using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float maxHealth = 5f;
    //public Gradient healthColorGradient;

    public float currentHealth;
    private GameObject healthBar;
    private GameObject damageTakenBar;

    private GameObject shieldBar;
    private float shield;

    public event System.Action OnDeath;
    public event System.Action OnHit;

    public float damageTakenIncreasePercentage = 0f;
    [HideInInspector]
    public float initialDamageTakenIncreasePercentage;
    protected PoolObject poolObject;


    // Player captain only.
    private float bigShieldCurrent;
    private GameObject bigShield;
    protected virtual void Awake()
    {
        if (!healthBar)
            healthBar = transform.Find("HealthBar/Canvas/Bar").gameObject;
        damageTakenBar = transform.Find("HealthBar/Canvas/DamageTaken").gameObject;
        if (!shieldBar)
            shieldBar = transform.Find("HealthBar/Canvas/ShieldBar").gameObject;
        if (transform.Find("SpriteBody/BigShield"))
            bigShield = transform.Find("SpriteBody/BigShield").gameObject;
        currentHealth = maxHealth;
    }

    private void Start()
    {
        poolObject = PoolObject.instance;
        initialDamageTakenIncreasePercentage = damageTakenIncreasePercentage;
    }
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
    protected virtual void Update()
    {
        UpdateHealthBarLength();
        UpdateShieldBarWidth();
        if (bigShield)
            UpdateBigShieldCurrent();
        UpdateDamageTakenBar();
    }

    void UpdateDamageTakenBar()
    {
        float decreaseSpeed = 0.01f;
        if (damageTakenBar.transform.localScale.x > healthBar.transform.localScale.x)
            damageTakenBar.transform.localScale = new Vector3(damageTakenBar.transform.localScale.x - decreaseSpeed, damageTakenBar.transform.localScale.y, damageTakenBar.transform.localScale.z);

        if (damageTakenBar.transform.localScale.x < healthBar.transform.localScale.x)
            damageTakenBar.transform.localScale = healthBar.transform.localScale;
    }
    public void SetBigShieldCurrent(float val)
    {
        bigShieldCurrent = val;
        if (bigShield)
            bigShield.SetActive(true);
    }
    public void UpdateBigShieldCurrent()
    {
        if (bigShieldCurrent <= 0)
        {
            bigShieldCurrent = 0;
            bigShield.SetActive(false);
        }
    }

    public void HealMaxHealthPercentage(float amount)
    {
        currentHealth *= 1 + amount / 100;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }


    public void SetShield(float amount)
    {
        if (amount > shield)
            shield = amount;
    }

    public void SetShieldRelatedToCurrentHealthPercentage(float amount)
    {
        amount = currentHealth - currentHealth * (1 - amount / 100);
        if (amount > shield)
            shield = amount;
    }

    void UpdateShieldBarWidth()
    {
        if (shield <= 0)
        {
            shieldBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 100f);
            return;
        }


        shieldBar.GetComponent<RectTransform>().sizeDelta = new Vector2(GetShieldWidth(), 100f);
        shieldBar.transform.localScale = new Vector3(1, shieldBar.transform.localScale.y, shieldBar.transform.localScale.z);
    }

    float GetShieldWidth()
    {
        // Set width using shield percentage on health (? on 100).
        return shield / maxHealth * 100f;
    }

    void DisableHealthBar()
    {
        healthBar.SetActive(false);

    }

    protected virtual void OnEnable()
    {
        healthBar.SetActive(true);
        OnDeath += DisableHealthBar;
        healthBar.transform.localScale = new Vector3(0, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        currentHealth = maxHealth;
    }
    protected virtual void OnDisable()
    {
        OnDeath -= DisableHealthBar;
    }
    protected bool IsCallerPoisoning(Transform caller)
    {
        return caller && (caller.GetComponent<PoisonFrogUnit>() || caller.GetComponent<MushroomUnit2>());
    }

    //Caller can be null because the unit is calling getdamage on himself.
    public virtual void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        if (currentHealth <= 0)
            return;
        if (bigShieldCurrent > 0)
        {
            bigShieldCurrent--;
            damage = 0;
        }
        if (shield > 0)
        {
            shield -= damage;
            if (shield < damage)
            {
                damage -= shield;
                if (shield < 0)
                    shield = 0;
            }
            else
                damage = 0f;
        }

        currentHealth -= damage * GetDamageTakenIncreasePercentage();
        OnHit?.Invoke();
        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
            currentHealth = 0f;
        }
        poolObject.audioManager.PlayHitSound();
        if (damage < 1)
            return;
        GameObject obj = poolObject.DisplayDamageText(damage);
        if (obj)
            obj.transform.position = GetRandomPosition(transform.position, -0.04f, 0.04f, -0.04f, 0.04f);
    }

    Vector2 GetRandomPosition(Vector2 pos, float xRangeA, float xRangeB, float yRangeA, float yRangeB)
    {
        pos.x += Random.Range(xRangeA, xRangeB);
        pos.y += Random.Range(yRangeA, yRangeB);
        return pos;
    }
    void ResetDamageTakenIncreasePercentage()
    {
        damageTakenIncreasePercentage = initialDamageTakenIncreasePercentage;
    }

    public void InvokeResetDamageTakenIncreasePercentage(float time)
    {
        Invoke("ResetDamageTakenIncreasePercentage", time);
    }
    float GetDamageTakenIncreasePercentage()
    {
        return 1f + damageTakenIncreasePercentage / 100f;
    }

    private void UpdateHealthBarLength()
    {
        healthBar.transform.localScale = new Vector3(GetNewBarLength(), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    private float GetNewBarLength()
    {
        float barLenght = currentHealth / maxHealth;
        return barLenght;
    }
}
