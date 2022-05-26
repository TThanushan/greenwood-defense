using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float maxHealth;
    //public Gradient healthColorGradient;

    private float currentHealth;
    private GameObject healthBar;

    public event System.Action OnDeath;
    public event System.Action OnHit;

    public virtual void Awake()
    {
        if (!healthBar)
            healthBar = transform.Find("HealthBar/Canvas/Bar").gameObject;
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
        UpdateHealthBarLength();
    }

    private void OnEnable()
    {
        healthBar.transform.localScale = new Vector3(0, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        currentHealth = maxHealth;
    }

    public virtual void GetDamage(float damage)
    {
        currentHealth -= damage;
        OnHit?.Invoke();
        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
            currentHealth = 0f;
        }
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
