using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float maxHealth;
    //public Gradient healthColorGradient;

    public float currentHealth;
    private GameObject healthBar;

    private GameObject shieldBar;
    private float shield;

    public event System.Action OnDeath;
    public event System.Action OnHit;

    [HideInInspector]
    public float damageTakenIncreasePercentage = 0f;
    public float initialDamageTakenIncreasePercentage;
    protected PoolObject poolObject;
    protected virtual void Awake()
    {
        if (!healthBar)
            healthBar = transform.Find("HealthBar/Canvas/Bar").gameObject;
        if (!shieldBar)
            shieldBar = transform.Find("HealthBar/Canvas/ShieldBar").gameObject;

        currentHealth = maxHealth;
    }

    private void Start()
    {
        poolObject = PoolObject.instance;
        initialDamageTakenIncreasePercentage = damageTakenIncreasePercentage;
    }
    protected virtual void Update()
    {
        UpdateHealthBarLength();
        UpdateShieldBarWidth();
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
        return caller && (caller.GetComponent<PoisonFrog>() || caller.GetComponent<MushroomUnit2>());
    }

    //Caller can be null because the unit is calling getdamage on himself.
    public virtual void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        if (currentHealth <= 0)
            return;
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
        if (HitSoundName == "Classic")
            poolObject.audioManager.PlayHitSound();

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
