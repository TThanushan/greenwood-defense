using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradesShopUIManager : MonoBehaviour
{
    private SaveManager saveManager;
    public string selectedCard;
    public GameObject selectCursor;
    private void Start()
    {
        if (!saveManager)
            saveManager = SaveManager.instance;
        UpdateShopUI();
        InitUpgradesCards();
    }

    void UpdateShopUI(string oldCardName = "")
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

    void UpdateButtonName(string oldCardName)
    {
        if (selectedCard == "")
            return;
        Transform button = transform.Find("Buttons/UnitsButtonPanel/" + oldCardName);
        button.name = selectedCard;
    }

    void UpdateUpgradesCardsForLockedUnits()
    {
        Transform unitsButtonPanel = transform.Find("Buttons/UnitsButtonPanel");
        foreach (Transform upgradeCardButton in unitsButtonPanel)
        {
            string unitName = upgradeCardButton.name.Replace("UpgradeCard", "");
            SetUpgradeButtonTexts(upgradeCardButton, unitName);
            AddTriggers(upgradeCardButton);
        }
    }
    void UpdateUpgradesCards()
    {
        UpdateUpgradesCardsForLockedUnits();

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
        string effectText = GetUnit(unitName).effectDescription;
        if (IsUnitUnlocked(unitName))
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
    void InitUpgradesCards()
    {
        Transform unitsButtonPanel = transform.Find("Buttons/UnitsButtonPanel");
        foreach (Transform upgradeCardButton in unitsButtonPanel)
        {
            string unitName = upgradeCardButton.name.Replace("UpgradeCard", "");
            if (!saveManager.unlockedUnits.Contains(unitName))
                SetActiveUpgradeCardButtonLock(upgradeCardButton);
            SetUpgradeButtonTexts(upgradeCardButton, unitName);
            AddTriggers(upgradeCardButton);
        }
    }





    Transform GetSelectedCard()
    {
        if (selectedCard == "")
            return null;
        return transform.Find("Buttons/UnitsButtonPanel/" + selectedCard);
    }



    bool IsUnitUnlocked(string unitName)
    {
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
            level++;
            string nextUnitName = GetUnitNameWithoutNumbers(unitName) + level.ToString();
            unit = GetUnit(nextUnitName);
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
        price = int.Parse(GetUnitPrice(upgradeUnitName));
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
        print("bip");
    }

    void DisableUpgradeCard()
    {
        Button button = GetSelectedCard().Find("Button").GetComponent<Button>();
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
        if (IsUnitLevelMax())
            return;
        float price;
        string unitName, upgradeUnitName;
        GetUnlockInfos(out unitName, out upgradeUnitName, out price);


        if (CanUpgrade(upgradeUnitName, price))
            return;
        foreach (var unit in saveManager.unlockedUnits)
        {
            print(unit);

        }
        print(unitName);
        if (!IsUnitUnlocked(unitName))
        {
            Unlock(unitName, price);
            upgradeUnitName = unitName;
        }
        else
            Upgrade(upgradeUnitName, unitName, price);

        UpdateUIAfterUnlock(upgradeUnitName);
        if (IsUnitLevelMax())
        {
            DisableUpgradeCard();
            HideSelectedCardButtonCursor();
            EmptySelectedCardInfos();
        }
        SetActiveUpgradeCardButtonLock(GetSelectedCard(), false);

        saveManager.SaveUnlockedUnits();
    }

    bool CanUpgrade(string upgradeUnitName, float price)
    {
        return !DoesUnitExist(upgradeUnitName) || UnitAlreadyUnlocked(upgradeUnitName) || PlayerStatsScript.instance.money < price;
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
        // Set unit sprite.
        upgradeCardButton.Find("UnitSprite").GetComponent<Image>().sprite = (Sprite)Resources.Load(Constants.UNITS_SPRITE_RESOURCES_PATH + '/' + nameWithoutNumbers);
        // Set title.
        upgradeCardButton.Find("Title").GetComponent<TextMeshProUGUI>().text = nameWithoutNumbers;


        //Set Level.
        string lvl = GetUnitLevel(unitName);
        if (!IsUnitUnlocked(unitName))
            lvl = "1";
        upgradeCardButton.Find("LevelText").GetComponent<TextMeshProUGUI>().text = "Lvl " + lvl + "/4";
        // Set upgrade price;
        if (!IsUnitLevelMax(unitName))
            upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TMPro.TextMeshProUGUI>().text = GetUnitPrice(unitName) + '$';

    }



    SaveManager.Unit GetUnit(string unitName)
    {
        foreach (var unit in saveManager.units)
        {
            if (unit.name == unitName)
                return unit;
        }

        return null;
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
