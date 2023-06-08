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

    GameObject manaBarFill;
    TextMeshProUGUI currentManaText;


    Transform manaBarFillAbovePlayer;
    TextMeshProUGUI currentManaTextAbovePlayer;
    private void Awake()
    {
        if (!instance)
            instance = this;
        if (!manaBarFill)
            manaBarFill = transform.Find(Constants.MANA_BAR_FILL).gameObject;
        //manaBar = transform.Find("ManaBar/Bar").gameObject;
        if (!currentManaText)
            currentManaText = transform.Find(Constants.MAN_BAR_CURRENT_TEXT).GetComponent<TextMeshProUGUI>();
        //currentManaText = transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        UpdateManaBarLength();
        UpdateCurrentManaText();
        RegenerateMana();
        UpdateManaBarAbovePlayer();
    }
    private void Start()
    {
        //regenerationSpeed += 0.1f * StageInfosManager.instance.GetCurrentStageNumber();
        LoadStatsFromPrefs();
        InitCurrentManaUsingStartManaBonus();

        InitManaBarAbovePlayer();

    }
    void InitManaBarAbovePlayer()
    {
        var playerCaptain = PoolObject.instance.playerCaptain;
        manaBarFillAbovePlayer = playerCaptain.transform.Find("ManaBarCanvas/ManaBody/" + Constants.MANA_BAR_FILL);
        currentManaTextAbovePlayer = playerCaptain.transform.Find("ManaBarCanvas/ManaBody/CurrentManaText").GetComponent<TextMeshProUGUI>();
        transform.Find("MaxManaText").GetComponent<TextMeshProUGUI>().text = "/" + maxMana.ToString();
    }

    void UpdateManaBarAbovePlayer()
    {
        currentManaTextAbovePlayer.text = currentManaText.text;
        manaBarFillAbovePlayer.transform.localScale = manaBarFill.transform.localScale;
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


    void InitCurrentManaUsingStartManaBonus()
    {
        foreach (string name in SaveManager.instance.unlockedHeroUpgrades)
        {
            if (name.Contains("StartMana"))
                currentMana = (maxMana * (1 + (GetUpgradeNameNumbersOnly(name) / 100))) - maxMana;
        }
    }
    private void UpdateCurrentManaText()
    {
        currentManaText.text = Mathf.FloorToInt(currentMana).ToString();
    }

    private void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            if (currentMana + (regenerationSpeed * Time.deltaTime) > maxMana)
                currentMana = maxMana;
            else
                currentMana += regenerationSpeed * Time.deltaTime;
        }
        else if (currentMana > maxMana)
            currentMana = maxMana;
    }

    public void GiveMana(float mana)
    {
        if (currentMana + mana > maxMana)
            currentMana = maxMana;
        else
            currentMana += mana;
    }

    public void UseMana(float amount)
    {
        currentMana -= amount;
    }

    private void UpdateManaBarLength()
    {
        manaBarFill.transform.localScale = new Vector3(GetNewBarLength(), manaBarFill.transform.localScale.y, manaBarFill.transform.localScale.z);
    }

    private float GetNewBarLength()
    {
        float barLenght = currentMana / maxMana;
        return barLenght;
    }
}
