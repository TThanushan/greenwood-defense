using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradesShop : MonoBehaviour
{
    private SaveManager saveManager;
    public string selectedCard;
    public GameObject selectCursor;
    public GameObject upgradeButtonPrefab;
    public TMP_ColorGradient tooExpensivePriceColor;
    public TMP_ColorGradient priceOriginalColor;
    public GameObject upgradeEffect;
    public Color32 starsMaxColor;
    Transform buttonParentTransform;
    private void Start()
    {
        if (!saveManager)
            saveManager = SaveManager.instance;
        buttonParentTransform = transform.Find(Constants.UNITS_BUTTON_PANEL_PATH);
        GenerateUpgradeButton();
        InitUpgradesCards();
        UpdateShopUI();
    }

    public void UpdateShopUI(string oldCardName = "")
    {
        SetPlayerGoldText();
        UpdateButtonName(oldCardName);
        UpdateUpgradesCards();
        SetSelectedCardInfos();
        SetSelectedCardButtonCursor();
    }

    void GenerateUpgradeButton()
    {
        foreach (SaveManager.Unit unit in saveManager.units)
        {
            if (!unit.name.Contains('1'))
                continue;

            GameObject button = Instantiate(upgradeButtonPrefab, buttonParentTransform);
            button.name = "UpgradeCard" + unit.name;
        }
    }
    void SetPlayerGoldText()
    {
        transform.Find("Money/MoneyText").GetComponent<TextMeshProUGUI>().text = ((int)saveManager.money).ToString() + '$';
    }

    void UpdateUpgradesCards()
    {
        Transform unitsButtonPanel = transform.Find(Constants.UNITS_BUTTON_PANEL_PATH);
        foreach (Transform upgradeCardButton in unitsButtonPanel)
        {
            string unitName = upgradeCardButton.name.Replace("UpgradeCard", "");
            SetUpgradeButtonTexts(upgradeCardButton, unitName);
            SaveManager.Unit upgradeUnit = GetUpgradeUnit(unitName);
            if (upgradeUnit != null)
                UpdatePriceTextColor(upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TextMeshProUGUI>(), upgradeUnit.name);
        }
    }

    void SetSelectedCardInfos()
    {
        //if (selectedCard == "")
        //    return;
        Transform selectedCardInfos = transform.Find("SelectedCardInfos");
        string unitName = GetUnitNameFromSelectCard();
        if (selectedCard == "")
        {
            selectedCardInfos.Find("UnitName").GetComponent<TextMeshProUGUI>().text = "";
            selectedCardInfos.Find("Description").GetComponent<TextMeshProUGUI>().text = "";
            selectedCardInfos.Find("ManaCost").GetComponent<TextMeshProUGUI>().text = "";

            return;
        }
        else if (IsUnitLevelMax(unitName))
        {
            GetSelectedCard().Find("UpgradePrice/PriceText").GetComponent<TMPro.TextMeshProUGUI>().text = "Max";
            return;
        }



        //SaveManager.Unit nextUnit = GetUpgradeUnit(unitName);
        //string name = unitName;
        string effectText = saveManager.GetUnit(unitName).effectDescription;
        if (IsUnitOfAnyLevelUnlocked(unitName))
        {
            SaveManager.Unit upgradeUnit = GetUpgradeUnit(unitName);
            //name = upgradeUnit.name;
            effectText = upgradeUnit.effectDescription;
        }

        //selectedCardInfos.Find("UnitName").GetComponent<TextMeshProUGUI>().text = name + " : ";
        selectedCardInfos.Find("UnitName").GetComponent<TextMeshProUGUI>().text = "Next level : ";
        selectedCardInfos.Find("Description").GetComponent<TextMeshProUGUI>().text = effectText;
        string manaCostText = string.Concat("Mana : ", GetUnit(unitName).cost);
        if (GetUnit(unitName).cost != GetUpgradeUnit(unitName).cost)
            manaCostText += " -> " + GetUpgradeUnit(unitName).cost.ToString();

        selectedCardInfos.Find("ManaCost").GetComponent<TextMeshProUGUI>().text = manaCostText;

    }
    public void SetSelectedCardButtonCursor()
    {
        if (selectedCard != "")
            selectCursor.transform.position = GetSelectedCard().position;
    }
    void HideSelectedCardButtonCursor()
    {
        selectCursor.transform.position = new Vector2(100000, 100000);
    }
    public void UnselectCard()
    {
        selectedCard = "";
        selectCursor.transform.position = new Vector2(100000, 100000);
    }
    public void SelectCard()
    {
        selectedCard = EventSystem.current.currentSelectedGameObject.transform.parent.name;
        SetSelectedCardInfos();
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

    void UpdateButtonName(string oldCardName)
    {
        if (selectedCard == "")
            return;
        Transform button = transform.Find(Constants.UNITS_BUTTON_PANEL_PATH + '/' + oldCardName);
        button.name = selectedCard;
    }


    void InitUpgradesCards()
    {
        //Transform unitsButtonPanel = transform.Find("Buttons/UnitsButtonPanel");
        Transform unitsButtonPanel = transform.Find(Constants.UNITS_BUTTON_PANEL_PATH);
        foreach (Transform upgradeCardButton in unitsButtonPanel)
        {
            string unitName = upgradeCardButton.name.Replace("UpgradeCard", "");
            AddTriggers(upgradeCardButton);
            if (IsUnitOfAnyLevelUnlocked(unitName))
            {
                SetUpgradeButtonName(GetUnlockedUnit(unitName), upgradeCardButton.name);
            }
            if (!IsUnitOfAnyLevelUnlocked(unitName))
                SetActiveUpgradeCardButtonLock(upgradeCardButton);
            SetUpgradeButtonTexts(upgradeCardButton, unitName);
            //if (!saveManager.unlockedUnits.Contains(unitName))
        }
    }

    void CreateUpgradeEffect()
    {
        GameObject effect = Instantiate(upgradeEffect);
        effect.transform.position = selectCursor.transform.position;

    }
    void SetUpgradeButtonName(string unitName, string oldName)
    {
        //string path = "Buttons/UnitsButtonPanel/";
        string path = Constants.UNITS_BUTTON_PANEL_PATH + '/';
        transform.Find(path + oldName).name = "UpgradeCard" + unitName;
    }

    string GetUnlockedUnit(string unitName)
    {
        foreach (string unitNameUnlocked in saveManager.unlockedUnits)
        {
            if (GetUnitNameWithoutNumbers(unitName) == GetUnitNameWithoutNumbers(unitNameUnlocked))
                return unitNameUnlocked;
        }
        return unitName;
    }




    Transform GetSelectedCard()
    {
        if (selectedCard == "")
            return null;
        return transform.Find(Constants.UNITS_BUTTON_PANEL_PATH + '/' + selectedCard);
    }



    bool IsUnitOfAnyLevelUnlocked(string unitName)
    {
        foreach (string name in saveManager.unlockedUnits)
        {
            string nameNoNumber = GetUnitNameWithoutNumbers(name);
            if (nameNoNumber == GetUnitNameWithoutNumbers(unitName))
                return true;
        }
        return saveManager.unlockedUnits.Contains(unitName);
    }



    void EmptySelectedCardInfos()
    {
        Transform selectedCardInfos = transform.Find("SelectedCardInfos");
        selectedCardInfos.Find("UnitName").GetComponent<TextMeshProUGUI>().text = "";
        selectedCardInfos.Find("Description").GetComponent<TextMeshProUGUI>().text = "";
        selectedCardInfos.Find("ManaCost").GetComponent<TextMeshProUGUI>().text = "";
    }


    string GetUnitNameFromSelectCard()
    {
        return selectedCard.Replace("UpgradeCard", "");
    }

    SaveManager.Unit GetUpgradeUnit(string unitName)
    {
        if (unitName == "")
            return null;
        SaveManager.Unit unit = null;
        int level = int.Parse(GetUnitLevel(unitName));
        if (level < 4)
        {
            if (IsUnitOfAnyLevelUnlocked(unitName))
                level++;
            string name = GetUnitNameWithoutNumbers(unitName) + level.ToString();
            unit = saveManager.GetUnit(name);
        }
        return unit;
    }



    bool IsUnitLevelMax(string unitName)
    {
        return GetUpgradeUnit(unitName) is null;
    }

    bool IsUnitLevelMax()
    {
        if (selectedCard == "")
            return false;
        return GetUpgradeUnit(GetUnitNameFromSelectCard()) is null;
    }

    void GetUnlockInfos(out string unitName, out string upgradeUnitName, out float price)
    {
        unitName = GetUnitNameFromSelectCard();
        upgradeUnitName = GetUpgradeUnit(unitName).name;
        price = int.Parse(GetUnitPrice(unitName));
    }

    void Upgrade(string upgradeUnitName, string unitName, float price)
    {
        saveManager.money -= price;
        saveManager.unlockedUnits.Add(upgradeUnitName);
        saveManager.unlockedUnits.Remove(unitName);
    }

    void Unlock(string unitName, float price)
    {
        saveManager.money -= price;
        saveManager.unlockedUnits.Add(unitName);
    }

    void DisableUpgradeCard(Button button = null)
    {
        if (button is null)
            button = GetSelectedCard().Find("Button").GetComponent<Button>();
        button.interactable = false;
        Destroy(button.GetComponent<EventTrigger>());
    }



    void UpdateUIAfterUnlock(string upgradeUnitName)
    {
        // Update selected card.
        string oldCardName = selectedCard;
        selectedCard = "UpgradeCard" + upgradeUnitName;
        UpdateShopUI(oldCardName);
    }



    public void UnlockUnit()
    {
        if (selectedCard == "")
            return;

        if (IsUnitLevelMax())
            return;
        GetUnlockInfos(out string unitName, out string upgradeUnitName, out float price);


        if (!CanUpgrade(upgradeUnitName))
            return;

        if (!IsUnitOfAnyLevelUnlocked(unitName))
        {
            Unlock(unitName, price);
            upgradeUnitName = unitName;
        }
        else
        {
            price = GetUpgradeUnit(unitName).shopPrice;
            Upgrade(upgradeUnitName, unitName, price);
        }

        UpdateUIAfterUnlock(upgradeUnitName);
        CreateUpgradeEffect();
        if (IsUnitLevelMax())
        {
            DisableUpgradeCard();
            HideSelectedCardButtonCursor();
            EmptySelectedCardInfos();
            AudioManager.instance.PlaySfx("UnitLevelMax");
        }
        SetActiveUpgradeCardButtonLock(GetSelectedCard(), false);
        AudioManager.instance.PlaySfx(Constants.BUY_SFX_NAME);
        //saveManager.SaveUnlockedUnits();
        saveManager.SavePrefIfAutoSave();

    }

    bool CanUpgrade(string unitName)
    {
        return DoesUnitExist(unitName) && !UnitAlreadyUnlocked(unitName) && saveManager.money >= GetUnit(unitName).shopPrice;
    }



    bool UnitAlreadyUnlocked(string unitName)
    {
        return saveManager.unlockedUnits.Contains(unitName);
    }
    bool DoesUnitExist(string name)
    {
        return saveManager.units.Where(p => p.name == name) != null;
    }

    string GetUnitNameWithoutNumbers(string unitName)
    {
        return Regex.Replace(unitName, @"[\d-]", string.Empty);
    }


    void SetActiveUpgradeCardButtonLock(Transform button, bool value = true)
    {
        button.Find("Lock").gameObject.SetActive(value);
    }



    void SetUpgradeButtonTexts(Transform upgradeCardButton, string unitName)
    {
        string nameWithoutNumbers = GetUnitNameWithoutNumbers(unitName);
        upgradeCardButton.Find("UnitSprite").GetComponent<Image>().sprite = (Sprite)Resources.Load(Constants.UNITS_SPRITE_RESOURCES_PATH + '/' + unitName);
        upgradeCardButton.Find("Title").GetComponent<TextMeshProUGUI>().text = nameWithoutNumbers;
        string lvl = GetUnitLevel(unitName);

        string price = GetUnitPrice(unitName);
        SaveManager.Unit upgradeUnit = GetUpgradeUnit(unitName);
        if (upgradeUnit != null)
            price = GetUnitPrice(upgradeUnit.name);

        if (!IsUnitOfAnyLevelUnlocked(unitName))
        {
            lvl = "1";
            price = GetUnitPrice(unitName);
        }
        else if (IsUnitLevelMax(unitName))
        {
            upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TMPro.TextMeshProUGUI>().text = "Max";
            UpdateCardStars(upgradeCardButton, int.Parse(lvl));
            DisableUpgradeCard(upgradeCardButton.Find("Button").GetComponent<Button>());
        }
        //upgradeCardButton.Find("LevelText").GetComponent<TextMeshProUGUI>().text = "Lvl " + lvl + "/4";
        UpdateCardStars(upgradeCardButton, int.Parse(lvl));

        TMPro.TextMeshProUGUI priceText = upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TMPro.TextMeshProUGUI>();
        if (!IsUnitLevelMax(unitName))
        {
            priceText.text = price + '$';
            //UpdatePriceTextColor(priceText, upgradeUnit.name, upgradeUnit.shopPrice);
        }

    }
    void UpdateCardStars(Transform upgradeCardButton, int level)
    {
        Transform stars = upgradeCardButton.Find("Stars");
        int levelMax = 4;
        for (int i = 1; i < level + 1; i++)
        {
            string starName = "Star" + i.ToString();
            GameObject starUnlocked = stars.Find(starName + "/StarUnlocked").gameObject;
            starUnlocked.SetActive(true);
            if (level == levelMax)
                starUnlocked.GetComponent<Image>().color = starsMaxColor;
        }
    }

    void UpdatePriceTextColor(TMPro.TextMeshProUGUI text, string unitName)
    {
        if (CanUpgrade(unitName))
            text.colorGradientPreset = priceOriginalColor;
        else
            text.colorGradientPreset = tooExpensivePriceColor;
    }



    string GetUnitPrice(string unitName)
    {
        foreach (SaveManager.Unit unit in saveManager.units)
        {
            if (unit.name == unitName)
                return unit.shopPrice.ToString();
        }
        return "0";
    }

    SaveManager.Unit GetUnit(string unitName)
    {

        foreach (SaveManager.Unit unit in saveManager.units)
        {
            if (unit.name == unitName)
                return unit;
        }
        return null;
    }

    string GetUnitLevel(string unitName)
    {
        if (unitName.Contains("2"))
            return "2";
        if (unitName.Contains("3"))
            return "3";
        if (unitName.Contains("4"))
            return "4";
        return "1";
    }
}
