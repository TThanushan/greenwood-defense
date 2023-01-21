using UnityEngine;
using UnityEngine.UI;

public class UltimateHealthBar : MonoBehaviour
{
    public static UltimateHealthBar instance;

    GameObject healthBar;
    GameObject damageTakenBar;
    Unit ultimate;
    Image image;
    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
        if (!healthBar)
            healthBar = transform.Find("HealthBar/Canvas/Bar").gameObject;
        damageTakenBar = transform.Find("HealthBar/Canvas/DamageTaken").gameObject;
        image = transform.Find("HealthBar/Canvas/UltimateImage").GetComponent<Image>();
    }
    private void Update()
    {
        UpdateUltimateHealthBar();
    }
    void UpdateUltimateHealthBar()
    {
        if (ultimate && !ultimate.Disabled)
        {
            healthBar.transform.parent.gameObject.SetActive(true);
            UpdateHealthBarLength();
            UpdateDamageTakenBar();
            if (image.sprite == null)
                image.sprite = ultimate.GetUnitSpriteRenderer().sprite;

        }
        else
            healthBar.transform.parent.gameObject.SetActive(false);
    }

    public void SetUltimateReference(Unit _ultimate)
    {
        ultimate = _ultimate;
    }

    private void UpdateHealthBarLength()
    {
        healthBar.transform.localScale = new Vector3(GetNewBarLength(), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    private float GetNewBarLength()
    {
        float barLenght = ultimate.currentHealth / ultimate.maxHealth;
        return barLenght;
    }
    void UpdateDamageTakenBar()
    {
        float decreaseSpeed = 0.01f;
        if (damageTakenBar.transform.localScale.x > healthBar.transform.localScale.x)
            damageTakenBar.transform.localScale = new Vector3(damageTakenBar.transform.localScale.x - decreaseSpeed, damageTakenBar.transform.localScale.y, damageTakenBar.transform.localScale.z);

        if (damageTakenBar.transform.localScale.x < healthBar.transform.localScale.x)
            damageTakenBar.transform.localScale = healthBar.transform.localScale;
    }
}
