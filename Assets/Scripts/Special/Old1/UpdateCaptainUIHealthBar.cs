using UnityEngine;

public class UpdateCaptainUIHealthBar : MonoBehaviour
{
    GameObject playerCaptainHealthBar;
    GameObject enemyCaptainHealthBar;

    Unit playerCaptainUnit;
    Unit enemyCaptainUnit;

    //PoolObject poolObject;

    private void Start()
    {
        //poolObject = PoolObject.instance;
        playerCaptainUnit = PoolObject.instance.playerCaptain;
        enemyCaptainUnit = PoolObject.instance.enemyCaptain;
        InitBar();
    }

    void InitBar()
    {
        string groupPath = "TopGroup/HealthPointBody";
        string barPath = "HealthBar/Canvas/Bar";
        playerCaptainHealthBar = transform.Find(groupPath + "/Player/" + barPath).gameObject;
        enemyCaptainHealthBar = transform.Find(groupPath + "/Enemy/" + barPath).gameObject;
    }

    private void Update()
    {
        UpdateHealthBarLength(playerCaptainHealthBar, playerCaptainUnit.currentHealth, playerCaptainUnit.maxHealth);
        UpdateHealthBarLength(enemyCaptainHealthBar, enemyCaptainUnit.currentHealth, enemyCaptainUnit.maxHealth);
    }

    private void UpdateHealthBarLength(GameObject healthBar, float currentHealth, float maxHealth)
    {
        healthBar.transform.localScale = new Vector3(GetNewBarLength(currentHealth, maxHealth),
            healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    private float GetNewBarLength(float currentHealth, float maxHealth)
    {
        float barLenght = currentHealth / maxHealth;
        return barLenght;
    }
}
