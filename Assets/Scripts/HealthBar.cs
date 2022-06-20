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
        shield = amount;
    }

    public void SetShieldRelatedToCurrentHealthPercentage(float amount)
    {
        if (amount > shield)
            shield = currentHealth - currentHealth * (1 - amount / 100);
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
        float width = 100f;
        if (shield < currentHealth)
            width = shield / currentHealth * 100f;
        return width;
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

    public virtual void GetDamage(float damage, Transform caller = null)
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
        poolObject.audioManager.PlayHitSound();

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
