using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    public static ManaBar instance;

    public float maxMana;
    public float currentMana;
    public float regenerationSpeed;

    private GameObject manaBar;
    private TextMeshProUGUI currentManaText;

    private void Awake()
    {
        if (!instance)
            instance = this;
        if (!manaBar)
            manaBar = transform.Find("ManaBar/Bar").gameObject;
        if (!currentManaText)
            currentManaText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }



    void LoadStatsFromPrefs()
    {
        foreach (string name in SaveManager.instance.unlockedHeroUpgrades)
        {
            const string MANA_MAX = "ManaMax";
            const string MANA_REGEN = "ManaRegen";
            if (name.Contains(MANA_MAX))
            {
                maxMana = GetUpgradeNameNumbersOnly(name);
                transform.Find("MaxManaText").GetComponent<TextMeshProUGUI>().text = "/" + maxMana.ToString();
            }
            else if (name.Contains(MANA_REGEN))
            {
                regenerationSpeed += GetUpgradeNameNumbersOnly(name);
            }
        }

    }

    public void UpdateManaMaxText()
    {
        transform.Find("MaxManaText").GetComponent<TextMeshProUGUI>().text = "/" + maxMana.ToString();
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
    private void Update()
    {
        UpdateManaBarLength();
        UpdateCurrentManaText();
        RegenerateMana();
    }
    private void Start()
    {
        //regenerationSpeed += 0.1f * StageInfosManager.instance.GetCurrentStageNumber();
        LoadStatsFromPrefs();
        InitCurrentManaUsingStartManaBonus();
    }

    void InitCurrentManaUsingStartManaBonus()
    {
        foreach (string name in SaveManager.instance.unlockedHeroUpgrades)
        {
            if (name.Contains("StartMana"))
                currentMana = maxMana * (1 + (GetUpgradeNameNumbersOnly(name) / 100)) - maxMana;
        }
    }
    private void UpdateCurrentManaText()
    {
        currentManaText.text = Mathf.FloorToInt(currentMana).ToString();
    }

    private void RegenerateMana()
    {
        if (currentMana < maxMana)
            currentMana += regenerationSpeed * Time.deltaTime;
        else if (currentMana > maxMana)
            currentMana = maxMana;
    }

    public void UseMana(float amount)
    {
        currentMana -= amount;
    }

    private void UpdateManaBarLength()
    {
        manaBar.transform.localScale = new Vector3(GetNewBarLength(), manaBar.transform.localScale.y, manaBar.transform.localScale.z);
    }

    private float GetNewBarLength()
    {
        float barLenght = currentMana / maxMana;
        return barLenght;
    }
}
