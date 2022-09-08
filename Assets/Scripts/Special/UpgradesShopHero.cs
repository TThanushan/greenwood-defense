using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradesShopHero : MonoBehaviour
{
    private SaveManager saveManager;
    public string selectedCard;
    public GameObject heroUpgradeButtonPrefab;
    public GameObject selectCursor;
    public TMP_ColorGradient tooExpensivePriceColor;
    public TMP_ColorGradient priceOriginalColor;
    public GameObject upgradeEffect;

    List<SaveManager.HeroUpgrade> nextHeroUpgrades;

    StringBuilder _sb = new(50);
    List<float> prices;
    float oldPlayerMoney;
    private void Start()
    {
        if (!saveManager)
            saveManager = SaveManager.instance;
        oldPlayerMoney = saveManager.money;

        prices = new List<float>();
        //UpdateNextHeroUpgrades();
        GenerateUpgradeButton();
        InitNextHeroUpgrades();
        InitUpgradesCards();
    }

    public void UpdateShopUI()
    {
        if (oldPlayerMoney != saveManager.money)
        {
            oldPlayerMoney = saveManager.money;
            SetPlayerGoldText();
            UpdateButton();
        }
        SetSelectedCardInfos();

    }

    void GenerateUpgradeButton()
    {
        GameObject button;
        GameObject spriteLoaded;
        GameObject sprite;
        saveManager.unlockedHeroUpgrades.Sort();

        foreach (string upgradeName in saveManager.unlockedHeroUpgrades)
        {
            button = Instantiate(heroUpgradeButtonPrefab, transform.Find(Constants.HERO_BUTTON_PANEL_PATH));
            spriteLoaded = (GameObject)Resources.Load(Constants.HERO_UPGRADES_SPRITE_RESOURCES_PATH + '/' + GetUpgradeNameWithoutNumbers(upgradeName));
            sprite = Instantiate(spriteLoaded, button.transform.Find("Sprite").transform);
            button.name = "UpgradeCard" + upgradeName;
        }
    }
    void InitUpgradesCards()
    {
        //Transform unitsButtonPanel = transform.Find("Buttons/HeroButtonPanel");
        Transform unitsButtonPanel = transform.Find(Constants.HERO_BUTTON_PANEL_PATH);
        string upgradeName;
        string unlockedName;
        foreach (Transform upgradeCardButton in unitsButtonPanel)
        {
            upgradeName = upgradeCardButton.name.Replace("UpgradeCard", "");
            unlockedName = GetUnlockedUpgradeName(upgradeName);
            //string name = GetUnlockedUpgradeName(upgradeName);
            upgradeCardButton.Find("Title").GetComponent<TextMeshProUGUI>().text = GetUpgradeNameWithoutNumbers(upgradeName);
            upgradeCardButton.name = "UpgradeCard" + unlockedName;
            TextMeshProUGUI priceText = upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TextMeshProUGUI>();

            if (!IsUpgradeMax(unlockedName))
            {
                AddTriggers(upgradeCardButton);
                SaveManager.HeroUpgrade nextHeroUpgrade = GetNextHeroUpgrade(unlockedName);
                UpdatePriceTextColor(priceText, nextHeroUpgrade.name);
                prices.Add(nextHeroUpgrade.shopPrice);
                string price = nextHeroUpgrade.shopPrice.ToString();
                priceText.text = price;

            }
            else
                DisableUpgradeCard(upgradeCardButton.Find("Button").GetComponent<Button>());
            UpdateUpgradeCardLevelText(upgradeCardButton, unlockedName);

        }
    }


    public void UpdateCurrentButtonLevelText()
    {
        UpdateUpgradeCardLevelText(GetSelectedCard(), GetUpgradeNameFromSelectCard());
    }
    void UpdateButtonPriceTextColor()
    {
        Transform unitsButtonPanel = transform.Find(Constants.HERO_BUTTON_PANEL_PATH);
        foreach (Transform upgradeCardButton in unitsButtonPanel)
        {
            //if (!upgradeCardButton.name.Equals(GetSelectedCard().name))
            //    continue;
            string upgradeName = upgradeCardButton.name.Replace("UpgradeCard", "");
            SaveManager.HeroUpgrade nextUpgrade = GetNextHeroUpgrade(upgradeName);
            if (nextUpgrade is null || nextUpgrade.shopPrice < saveManager.money)
                continue;
            //if (!IsUpgradeMax(upgradeName))
            UpdatePriceTextColor(upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TextMeshProUGUI>(), GetNextHeroUpgrade(upgradeName).name);
        }
    }

    void UpdateCurrentButton()
    {
        Transform unitsButtonPanel = transform.Find(Constants.HERO_BUTTON_PANEL_PATH);
        foreach (Transform upgradeCardButton in unitsButtonPanel)
        {
            if (!upgradeCardButton.name.Equals(GetSelectedCard().name))
                continue;
            string upgradeName = upgradeCardButton.name.Replace("UpgradeCard", "");
            string unlockedName = GetUnlockedUpgradeName(upgradeName);
            upgradeCardButton.Find("Title").GetComponent<TextMeshProUGUI>().text = GetUpgradeNameWithoutNumbers(upgradeName);


            upgradeCardButton.name = "UpgradeCard" + unlockedName;
            string price = "Max";
            TextMeshProUGUI priceText = upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TextMeshProUGUI>();
            if (!IsUpgradeMax(unlockedName))
            {
                SaveManager.HeroUpgrade nextHeroUpgrade = GetNextHeroUpgrade(unlockedName);
                price = nextHeroUpgrade.shopPrice.ToString();
                UpdatePriceTextColor(priceText, nextHeroUpgrade.name);
            }
            UpdateUpgradeCardLevelText(upgradeCardButton, unlockedName);

            priceText.text = price;
        }

    }

    void UpdateButton()
    {
        Transform unitsButtonPanel = transform.Find(Constants.HERO_BUTTON_PANEL_PATH);
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
                price = nextHeroUpgrade.shopPrice.ToString();
                UpdatePriceTextColor(priceText, nextHeroUpgrade.name);
            }
            priceText.text = price;
        }
    }

    void InitNextHeroUpgrades()
    {
        nextHeroUpgrades = new List<SaveManager.HeroUpgrade>();
        foreach (string unlockedHeroUpgrade in saveManager.unlockedHeroUpgrades)
        {
            SaveManager.HeroUpgrade heroUpgrade = saveManager.GetHeroUpgrade(unlockedHeroUpgrade);
            SaveManager.HeroUpgrade a = GetNextHeroUpgradeSimple(heroUpgrade.name);
            if (a != null)
                nextHeroUpgrades.Add(a);
        }
    }
    void UpdateNextHeroUpgrades(string oldUpgradeName)
    {
        int i = 0;
        foreach (SaveManager.HeroUpgrade unlockedHeroUpgrade in nextHeroUpgrades)
        {
            if (unlockedHeroUpgrade.name == oldUpgradeName)
            {
                nextHeroUpgrades.RemoveAt(i);
                break;
            }
            i++;
        }
        SaveManager.HeroUpgrade newUpgrade = GetNextHeroUpgradeSimple(oldUpgradeName);
        if (newUpgrade != null)
            nextHeroUpgrades.Add(newUpgrade);
    }



    void UpdateUpgradeCardLevelText(Transform button, string upgradeName)
    {

        TextMeshProUGUI tmPro = button.Find("LevelText").GetComponent<TextMeshProUGUI>();

        int lvl = GetUpgradeLevel(upgradeName) - 1;
        string lvlText = lvl.ToString();

        int maxlvl = GetMaxLevel(upgradeName) - 1;
        tmPro.text = ConcatLevelText(lvlText, maxlvl.ToString());
    }

    string ConcatLevelText(string currentLevel, string maxLevel)
    {
        _sb.Clear();
        _sb.Append(currentLevel);
        _sb.Append('/');

        _sb.Append(maxLevel);
        return _sb.ToString();
    }

    string Concat(string[] stringArray)
    {
        _sb.Clear();

        for (int i = 0; i < stringArray.Length; i++)
        {
            _sb.Append(stringArray[i]);
        }

        return _sb.ToString();
    }

    void UpdatePriceTextColor(TextMeshProUGUI text, string upgradeName)
    {

        if (saveManager.money >= saveManager.GetHeroUpgrade(upgradeName).shopPrice)
            text.colorGradientPreset = priceOriginalColor;
        else
            text.colorGradientPreset = tooExpensivePriceColor;
    }

    void SetPlayerGoldText()
    {
        transform.Find("Money/MoneyText").GetComponent<TextMeshProUGUI>().text = saveManager.money.ToString();
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
    public void SetSelectedCardButtonCursor()
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
            selectedCardInfos.Find("ManaCost").GetComponent<TextMeshProUGUI>().text = "";

            return;
        }

        string effectText = GetNextHeroUpgrade(upgradeName).description;
        if (IsAnyUpgradeUnlocked(upgradeName))
        {
            SaveManager.HeroUpgrade upgradeUnit = GetNextHeroUpgrade(upgradeName);
            effectText = upgradeUnit.description;
        }

        selectedCardInfos.Find("UnitName").GetComponent<TextMeshProUGUI>().text = "Next level : ";
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


    //float GetMaxUpgradeLevel(string upgradeName)
    //{
    //    string name = GetUpgradeNameWithoutNumbers(upgradeName);
    //    float levelMax = 0f;
    //    string nameCurrent;
    //    float levelCurrent;
    //    foreach (SaveManager.HeroUpgrade heroUpgrade in saveManager.heroUpgrades)
    //    {
    //        nameCurrent = GetUpgradeNameWithoutNumbers(heroUpgrade.name);
    //        if (name != nameCurrent)
    //            continue;
    //        levelCurrent = GetUpgradeNameNumbersOnly(heroUpgrade.name);
    //        if (levelCurrent > levelMax)
    //            levelMax = levelCurrent;
    //    }
    //    return levelMax;
    //}
    float GetMaxUpgradeLevel(string upgradeName)
    {
        string name = GetUpgradeNameWithoutNumbers(upgradeName);
        float levelMax = 0f;
        string nameCurrent;
        for (int i = saveManager.heroUpgrades.Count - 1; i != 0; i--)
        {
            SaveManager.HeroUpgrade heroUpgrade = saveManager.heroUpgrades[i];
            nameCurrent = GetUpgradeNameWithoutNumbers(heroUpgrade.name);
            if (name == nameCurrent)
                return GetUpgradeNameNumbersOnly(heroUpgrade.name);
        }

        return levelMax;
    }

    Transform GetSelectedCard()
    {
        if (selectedCard == "")
            return null;
        return transform.Find(Constants.HERO_BUTTON_PANEL_PATH + '/' + selectedCard);
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

    SaveManager.HeroUpgrade GetNextHeroUpgradeSimple(string upgradeName)
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
    SaveManager.HeroUpgrade GetNextHeroUpgrade(string upgradeName)
    {
        upgradeName = GetUpgradeNameWithoutNumbers(upgradeName);
        foreach (SaveManager.HeroUpgrade nextHeroUpgrade in nextHeroUpgrades)
        {
            if (GetUpgradeNameWithoutNumbers(nextHeroUpgrade.name) == upgradeName)
                return nextHeroUpgrade;
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
        if (upgradeName is null)
            return null;
        string withoutNumbers = Regex.Replace(upgradeName, @"[\d-]", string.Empty);
        withoutNumbers = withoutNumbers.Replace(".", "");
        return withoutNumbers;
    }

    public void UnselectCard()
    {
        selectedCard = "";
        selectCursor.transform.position = new Vector2(100000, 100000);
    }
    void CreateUpgradeEffect()
    {
        GameObject effect = Instantiate(upgradeEffect);
        effect.transform.position = selectCursor.transform.position;
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
            saveManager.money -= nextHeroUpgrade.shopPrice;
            string oldCardName = selectedCard;
            selectedCard = "UpgradeCard" + nextHeroUpgrade.name;
            UpdateButtonName(oldCardName);
            //UpdateShopUI();
            UpdateButtonPriceTextColor();
            //UpdateNextHeroUpgrades(next);
            UpdateNextHeroUpgrades(nextHeroUpgrade.name);
            SetPlayerGoldText();
            AudioManager.instance.PlaySfx(Constants.BUY_SFX_NAME);
            UpdateCurrentButton();
            CreateUpgradeEffect();
            //UpdateCurrentButtonLevelText();
        }

        if (IsUpgradeMax(GetUpgradeNameFromSelectCard()))
        {
            AudioManager.instance.PlaySfx("UnitLevelMax");
            DisableUpgradeCard();
            UnselectCard();
        }
    }

    void UpdateButtonName(string oldCardName)
    {
        if (selectedCard == "")
            return;
        Transform button = transform.Find(Constants.HERO_BUTTON_PANEL_PATH + '/' + oldCardName);
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
        bool enoughMoney = saveManager.money >= saveManager.GetHeroUpgrade(upgradeName).shopPrice;
        return enoughMoney;

    }

    bool IsAlreadyUnlocked(string upgradeName)
    {
        return saveManager.unlockedHeroUpgrades.Contains(upgradeName);
    }

}