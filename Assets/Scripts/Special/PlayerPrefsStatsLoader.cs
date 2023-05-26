using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerPrefsStatsLoader : MonoBehaviour
{
    Unit playerUnit;


    void Start()
    {
        playerUnit = GetComponent<Unit>();
        LoadStatsFromPrefs();
    }

    void LoadStatsFromPrefs()
    {
        foreach (string name in SaveManager.instance.unlockedHeroUpgrades)
        {
            const string MAX_HEALTH = "MaxHealth";
            const string DAMAGE_REDUCTION = "DamageReduction";
            const string SHIELD = "Shield";
            const string SWORD_DAMAGE = "SwordDamage";
            const string CROSSBOW_DAMAGE = "CrossbowDamage";
            const string CROSSBOW_WEAPON = "CrossbowWeapon1";
            //const string CROSSBOW_ABILITY_PIERCING_ARROW = "CrossbowAbilityPiercingArrow";
            //const string SWORD_ABILITY_SLASH = "SwordAbilitySlash";

            if (name.Contains(MAX_HEALTH) && !name.Contains("Unit"))
            {
                float maxHealth = GetUpgradeNameNumbersOnly(name);
                playerUnit.maxHealth = maxHealth;
                playerUnit.currentHealth = maxHealth;
            }
            else if (name.Contains(DAMAGE_REDUCTION))
            {
                float damageReduction = -GetUpgradeNameNumbersOnly(name);
                playerUnit.damageTakenIncreasePercentage = damageReduction;
                playerUnit.initialDamageTakenIncreasePercentage = damageReduction;
            }
            else if (name.Contains(SHIELD))
            {
                float shield = GetUpgradeNameNumbersOnly(name);
                playerUnit.SetBigShieldCurrent(shield);
            }
            else if (name.Contains(SWORD_DAMAGE))
            {
                float damage = GetUpgradeNameNumbersOnly(name);
                if (damage > 0) playerUnit.gameObject.GetComponent<PlayerCaptainUnit>().swordDamage = damage;
            }
            else if (name.Contains(CROSSBOW_DAMAGE))
            {
                float damage = GetUpgradeNameNumbersOnly(name);
                if (damage > 0) playerUnit.gameObject.GetComponent<PlayerCaptainUnit>().crossbowDamage = damage;
            }
            else if (name.Contains(CROSSBOW_WEAPON))
            {
                playerUnit.gameObject.GetComponent<PlayerCaptainUnit>().isCrossbowUnlocked = true;
            }
        }
        //else if (name.Contains(CROSSBOW_ABILITY_PIERCING_ARROW))
        //{

        //}
        //else if (name.Contains(SWORD_ABILITY_SLASH))
        //{

    }

    float GetUpgradeNameNumbersOnly(string upgradeName)
    {
        string withoutNumbers = GetUpgradeNameWithoutNumbers(upgradeName);
        withoutNumbers = upgradeName.Replace(withoutNumbers, "");
        return float.Parse(withoutNumbers, CultureInfo.InvariantCulture.NumberFormat);
    }
    string GetUpgradeNameWithoutNumbers(string upgradeName)
    {
        string withoutNumbers = Regex.Replace(upgradeName, @"[\d-]", string.Empty);
        withoutNumbers = withoutNumbers.Replace(".", "");
        return withoutNumbers;
    }
}
