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

        }

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
