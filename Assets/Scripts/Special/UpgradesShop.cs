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
    public TMP_ColorGradient tooExpensivePriceColor;
    public TMP_ColorGradient priceOriginalColor;
    private void Start()
    {
        if (!saveManager)
            saveManager = SaveManager.instance;
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


    void SetPlayerGoldText()
    {
        transform.Find("Money/MoneyText").GetComponent<TextMeshProUGUI>().text = PlayerStatsScript.instance.money.ToString() + '$';
    }

    void UpdateUpgradesCards()
    {
        Transform unitsButtonPanel = transform.Find("Buttons/UnitsButtonPanel");
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
        if (selectedCard == "")
            return;
        Transform selectedCardInfos = transform.Find("SelectedCardInfos");
        string unitName = GetUnitNameFromSelectCard();
        if (IsUnitLevelMax(unitName))
        {
            GetSelectedCard().Find("UpgradePrice/PriceText").GetComponent<TMPro.TextMeshProUGUI>().text = "Max";
            return;
        }

        //SaveManager.Unit nextUnit = GetUpgradeUnit(unitName);
        string name = unitName;
        string effectText = saveManager.GetUnit(unitName).effectDescription;
        if (IsUnitOfAnyLevelUnlocked(unitName))
        {
            SaveManager.Unit upgradeUnit = GetUpgradeUnit(unitName);
            name = upgradeUnit.name;
            effectText = upgradeUnit.effectDescription;
        }

        selectedCardInfos.Find("UnitName").GetComponent<TextMeshProUGUI>().text = name + " : ";
        selectedCardInfos.Find("Description").GetComponent<TextMeshProUGUI>().text = effectText;
    }
    void SetSelectedCardButtonCursor()
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
        if (IsUnitLevelMax())
        {
            selectedCard = "";
            return;
        }
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
        Transform button = transform.Find("Buttons/UnitsButtonPanel/" + oldCardName);
        button.name = selectedCard;
    }


    void InitUpgradesCards()
    {
        Transform unitsButtonPanel = transform.Find("Buttons/UnitsButtonPanel");
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

    void SetUpgradeButtonName(string unitName, string oldName)
    {
        string path = "Buttons/UnitsButtonPanel/";
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
        return transform.Find("Buttons/UnitsButtonPanel/" + selectedCard);
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
        PlayerStatsScript.instance.money -= price;
        saveManager.unlockedUnits.Add(upgradeUnitName);
        saveManager.unlockedUnits.Remove(unitName);
    }

    void Unlock(string unitName, float price)
    {
        PlayerStatsScript.instance.money -= price;
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
        if (IsUnitLevelMax())
        {
            DisableUpgradeCard();
            HideSelectedCardButtonCursor();
            EmptySelectedCardInfos();
        }
        SetActiveUpgradeCardButtonLock(GetSelectedCard(), false);
        AudioManager.instance.PlaySfx(Constants.BUY_SFX_NAME);
        saveManager.SaveUnlockedUnits();
        saveManager.SavePrefs();
    }

    bool CanUpgrade(string unitName)
    {
        return DoesUnitExist(unitName) && !UnitAlreadyUnlocked(unitName) && PlayerStatsScript.instance.money >= GetUnit(unitName).shopPrice;
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
        upgradeCardButton.Find("UnitSprite").GetComponent<Image>().sprite = (Sprite)Resources.Load(Constants.UNITS_SPRITE_RESOURCES_PATH + '/' + nameWithoutNumbers);
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
            DisableUpgradeCard(upgradeCardButton.Find("Button").GetComponent<Button>());
        }
        upgradeCardButton.Find("LevelText").GetComponent<TextMeshProUGUI>().text = "Lvl " + lvl + "/4";
        TMPro.TextMeshProUGUI priceText = upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TMPro.TextMeshProUGUI>();
        if (!IsUnitLevelMax(unitName))
        {
            priceText.text = price + '$';
            //UpdatePriceTextColor(priceText, upgradeUnit.name, upgradeUnit.shopPrice);
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
