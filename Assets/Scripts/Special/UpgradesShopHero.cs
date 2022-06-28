using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradesShopHero : MonoBehaviour
{
    private SaveManager saveManager;
    public string selectedCard;
    public GameObject selectCursor;
    public TMP_ColorGradient tooExpensivePriceColor;
    public TMP_ColorGradient priceOriginalColor;


    private void Start()
    {
        if (!saveManager)
            saveManager = SaveManager.instance;
        InitUpgradesCards();
    }
    public void UpdateShopUI()
    {
        SetPlayerGoldText();
        UpdateButton();
        SetSelectedCardInfos();

    }

    void UpdateButton()
    {
        Transform unitsButtonPanel = transform.Find("Buttons/HeroButtonPanel");
        foreach (Transform upgradeCardButton in unitsButtonPanel)
        {
            string upgradeName = upgradeCardButton.name.Replace("UpgradeCard", "");
            string unlockedName = GetUnlockedUpgradeName(upgradeName);
            upgradeCardButton.Find("Title").GetComponent<TextMeshProUGUI>().text = GetUpgradeNameWithoutNumbers(upgradeName);
            upgradeCardButton.name = "UpgradeCard" + unlockedName;
            string price = "Max";
            TextMeshProUGUI priceText = upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TextMeshProUGUI>();
            if (!IsUpgradeMax(unlockedName))
            {
                SaveManager.HeroUpgrade nextHeroUpgrade = GetNextHeroUpgrade(unlockedName);
                price = nextHeroUpgrade.shopPrice.ToString() + "$";
                UpdatePriceTextColor(priceText, nextHeroUpgrade.name);
            }
            //string lvl = GetUpgradeLevel(unlockedName).ToString();
            //string maxlvl = GetMaxLevel(unlockedName).ToString();
            //upgradeCardButton.Find("LevelText").GetComponent<TextMeshProUGUI>().text = "Lvl " + lvl + "/" + maxlvl;
            UpdateUpgradeCardLevelText(upgradeCardButton, unlockedName);
            priceText.text = price;
        }
    }

    void UpdateUpgradeCardLevelText(Transform button, string upgradeName)
    {
        string lvl = GetUpgradeLevel(upgradeName).ToString();
        string maxlvl = GetMaxLevel(upgradeName).ToString();
        button.Find("LevelText").GetComponent<TextMeshProUGUI>().text = "Lvl " + lvl + "/" + maxlvl;
    }

    void UpdatePriceTextColor(TextMeshProUGUI text, string upgradeName)
    {

        if (PlayerStatsScript.instance.money >= saveManager.GetHeroUpgrade(upgradeName).shopPrice)
            text.colorGradientPreset = priceOriginalColor;
        else
            text.colorGradientPreset = tooExpensivePriceColor;
    }

    void SetPlayerGoldText()
    {
        transform.Find("Money/MoneyText").GetComponent<TextMeshProUGUI>().text = PlayerStatsScript.instance.money.ToString() + '$';
    }

    void InitUpgradesCards()
    {
        Transform unitsButtonPanel = transform.Find("Buttons/HeroButtonPanel");
        foreach (Transform upgradeCardButton in unitsButtonPanel)
        {
            string upgradeName = upgradeCardButton.name.Replace("UpgradeCard", "");
            string unlockedName = GetUnlockedUpgradeName(upgradeName);
            //string name = GetUnlockedUpgradeName(upgradeName);
            upgradeCardButton.Find("Title").GetComponent<TMPro.TextMeshProUGUI>().text = GetUpgradeNameWithoutNumbers(upgradeName);
            upgradeCardButton.name = "UpgradeCard" + unlockedName;
            TextMeshProUGUI priceText = upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TextMeshProUGUI>();

            if (!IsUpgradeMax(unlockedName))
            {
                AddTriggers(upgradeCardButton);
                SaveManager.HeroUpgrade nextHeroUpgrade = GetNextHeroUpgrade(unlockedName);
                UpdatePriceTextColor(priceText, nextHeroUpgrade.name);
                string price = nextHeroUpgrade.shopPrice.ToString() + "$";
                priceText.text = price;

            }
            else
                DisableUpgradeCard(upgradeCardButton.Find("Button").GetComponent<Button>());
            UpdateUpgradeCardLevelText(upgradeCardButton, unlockedName);

        }
    }

    string GetUnlockedUpgradeName(string upgradeName)
    {
        foreach (string unlockedUpgradeName in saveManager.unlockedHeroUpgrades)
        {
            if (GetUpgradeNameWithoutNumbers(unlockedUpgradeName) == GetUpgradeNameWithoutNumbers(upgradeName))
                return unlockedUpgradeName;
        }

        return null;
    }
    void SetSelectedCardButtonCursor()
    {
        if (selectedCard != "")
            selectCursor.transform.position = GetSelectedCard().position;
    }
    public void SelectCard()
    {

        selectedCard = EventSystem.current.currentSelectedGameObject.transform.parent.name;
        SetSelectedCardInfos();
    }

    void SetSelectedCardInfos()
    {
        Transform selectedCardInfos = transform.Find("SelectedCardInfos");
        string upgradeName = GetUpgradeNameFromSelectCard();
        if (selectedCard == "" || IsUpgradeMax(upgradeName))
        {
            selectedCardInfos.Find("UnitName").GetComponent<TextMeshProUGUI>().text = "";
            selectedCardInfos.Find("Description").GetComponent<TextMeshProUGUI>().text = "";
            return;
        }

        string name = upgradeName;
        string effectText = GetNextHeroUpgrade(upgradeName).description;
        if (IsAnyUpgradeUnlocked(upgradeName))
        {
            SaveManager.HeroUpgrade upgradeUnit = GetNextHeroUpgrade(upgradeName);
            name = upgradeUnit.name;
            effectText = upgradeUnit.description;
        }

        selectedCardInfos.Find("UnitName").GetComponent<TextMeshProUGUI>().text = name + " : ";
        selectedCardInfos.Find("Description").GetComponent<TextMeshProUGUI>().text = effectText;
    }

    bool IsAnyUpgradeUnlocked(string unitName)
    {
        foreach (string name in saveManager.unlockedUnits)
        {
            string nameNoNumber = GetUpgradeNameWithoutNumbers(name);
            if (nameNoNumber == GetUpgradeNameWithoutNumbers(unitName))
                return true;
        }
        return saveManager.unlockedUnits.Contains(unitName);
    }
    string GetUpgradeNameFromSelectCard()
    {
        return selectedCard.Replace("UpgradeCard", "");
    }

    bool IsUpgradeMax(string upgradeName)
    {
        return GetUpgradeNameNumbersOnly(upgradeName) == GetMaxUpgradeLevel(upgradeName);
    }
    float GetMaxUpgradeLevel(string upgradeName)
    {
        string name = GetUpgradeNameWithoutNumbers(upgradeName);
        float levelMax = 0f;
        string nameCurrent;
        float levelCurrent;
        foreach (SaveManager.HeroUpgrade heroUpgrade in saveManager.heroUpgrades)
        {
            nameCurrent = GetUpgradeNameWithoutNumbers(heroUpgrade.name);
            if (name != nameCurrent)
                continue;
            levelCurrent = GetUpgradeNameNumbersOnly(heroUpgrade.name);
            if (levelCurrent > levelMax)
                levelMax = levelCurrent;
        }
        return levelMax;
    }

    Transform GetSelectedCard()
    {
        if (selectedCard == "")
            return null;
        return transform.Find("Buttons/HeroButtonPanel/" + selectedCard);
    }
    void AddTriggers(Transform upgradeCardButton)
    {
        AddEventTriggerOnButton(upgradeCardButton.Find("Button").gameObject, SelectCard);
        AddEventTriggerOnButton(upgradeCardButton.Find("Button").gameObject, SetSelectedCardButtonCursor);
    }

    public void PlayButtonClick()
    {
        AudioManager.instance.PlaySfx(Constants.BUTTON_CLICK_SFX_NAME);
    }
    void AddEventTriggerOnButton(GameObject button, System.Action function)
    {
        EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
        if (eventTrigger is null)
            eventTrigger = button.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => { function(); });
        eventTrigger.triggers.Add(entry);
    }
    SaveManager.HeroUpgrade GetNextHeroUpgrade(string upgradeName)
    {
        string name = GetUpgradeNameWithoutNumbers(upgradeName);
        float level = GetUpgradeNameNumbersOnly(upgradeName);

        float levelCurrent;
        string nameCurrent;
        foreach (SaveManager.HeroUpgrade heroUpgrade in saveManager.heroUpgrades)
        {
            nameCurrent = GetUpgradeNameWithoutNumbers(heroUpgrade.name);
            if (name != nameCurrent)
                continue;

            levelCurrent = GetUpgradeNameNumbersOnly(heroUpgrade.name);
            if (levelCurrent > level)
            {
                return heroUpgrade;
            }
        }
        return null;
    }

    int GetUpgradeLevel(string upgradeName)
    {
        string name = GetUpgradeNameWithoutNumbers(upgradeName);

        int level = 0;
        string nameCurrent;
        foreach (SaveManager.HeroUpgrade heroUpgrade in saveManager.heroUpgrades)
        {
            nameCurrent = GetUpgradeNameWithoutNumbers(heroUpgrade.name);
            if (name == nameCurrent)
            {
                level++;
                if (upgradeName == heroUpgrade.name)
                    break;
            }
        }
        return level;
    }

    int GetMaxLevel(string upgradeName)
    {
        string name = GetUpgradeNameWithoutNumbers(upgradeName);

        int level = 0;
        string nameCurrent;
        foreach (SaveManager.HeroUpgrade heroUpgrade in saveManager.heroUpgrades)
        {
            nameCurrent = GetUpgradeNameWithoutNumbers(heroUpgrade.name);
            if (name == nameCurrent)
            {
                level++;
                if (name == heroUpgrade.name)
                    break;
            }
        }
        return level;
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

    public void UnselectCard()
    {
        selectedCard = "";
        selectCursor.transform.position = new Vector2(100000, 100000);
    }

    public void UnlockUpgrade()
    {
        if (selectedCard == "")
            return;
        string currentHeroUpgradeName = GetUpgradeNameFromSelectCard();
        SaveManager.HeroUpgrade nextHeroUpgrade = GetNextHeroUpgrade(currentHeroUpgradeName);

        if (!IsUpgradeMax(currentHeroUpgradeName) && CanUpgrade(nextHeroUpgrade.name))
        {
            saveManager.unlockedHeroUpgrades.Remove(currentHeroUpgradeName);
            saveManager.unlockedHeroUpgrades.Add(nextHeroUpgrade.name);
            PlayerStatsScript.instance.money -= nextHeroUpgrade.shopPrice;
            saveManager.SaveUnlockedHeroUpgrades();
            string oldCardName = selectedCard;
            selectedCard = "UpgradeCard" + nextHeroUpgrade.name;
            UpdateButtonName(oldCardName);
            UpdateShopUI();
            AudioManager.instance.PlaySfx(Constants.BUY_SFX_NAME);
        }
        if (IsUpgradeMax(GetUpgradeNameFromSelectCard()))
        {
            DisableUpgradeCard();
            UnselectCard();
        }
    }

    void UpdateButtonName(string oldCardName)
    {
        if (selectedCard == "")
            return;
        Transform button = transform.Find("Buttons/HeroButtonPanel/" + oldCardName);
        button.name = selectedCard;
    }

    void DisableUpgradeCard(Button button = null)
    {
        if (button is null)
            button = GetSelectedCard().Find("Button").GetComponent<Button>();
        button.transform.parent.Find("UpgradePrice/PriceText").GetComponent<TMPro.TextMeshProUGUI>().text = "Max";
        button.interactable = false;
        Destroy(button.GetComponent<EventTrigger>());
    }

    bool CanUpgrade(string upgradeName)
    {
        if (IsAlreadyUnlocked(upgradeName))
            return false;
        bool enoughMoney = PlayerStatsScript.instance.money >= saveManager.GetHeroUpgrade(upgradeName).shopPrice;
        return enoughMoney;

    }

    bool IsAlreadyUnlocked(string upgradeName)
    {
        return saveManager.unlockedHeroUpgrades.Contains(upgradeName);
    }

}