using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradesShopHeroSave : MonoBehaviour
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
        //UpdateShopUI();
    }
    void InitUpgradesCards()
    {
        Transform unitsButtonPanel = transform.Find("Buttons/UnitsButtonPanel");
        foreach (Transform upgradeCardButton in unitsButtonPanel)
        {
            string unitName = upgradeCardButton.name.Replace("UpgradeCard", "");
            AddTriggers(upgradeCardButton);
            SetUpgradeButtonTexts(upgradeCardButton, unitName);
            //if (!saveManager.unlockedUnits.Contains(unitName))
        }
    }

    void SetUpgradeButtonTexts(Transform upgradeCardButton, string upgradeName)
    {

    }


    void AddTriggers(Transform upgradeCardButton)
    {
        AddEventTriggerOnButton(upgradeCardButton.Find("Button").gameObject, SelectCard);
        AddEventTriggerOnButton(upgradeCardButton.Find("Button").gameObject, SetSelectedCardButtonCursor);
        AddEventTriggerOnButton(upgradeCardButton.Find("Button").gameObject, PlayButtonClick);
    }
    void SetSelectedCardButtonCursor()
    {
        if (selectedCard != "")
            selectCursor.transform.position = GetSelectedCard().position;
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
    public void PlayButtonClick()
    {
        AudioManager.instance.PlaySfx(Constants.BUTTON_CLICK_SFX_NAME);
    }
    public void SelectCard()
    {

        selectedCard = EventSystem.current.currentSelectedGameObject.transform.parent.name;
        SetSelectedCardInfos();
    }

    void SetSelectedCardInfos()
    {
        if (selectedCard == "")
            return;
        Transform selectedCardInfos = transform.Find("SelectedCardInfos");
        string upgradeName = GetUpgradeNameFromSelectCard();
        if (IsUpgradeMax(upgradeName))
        {
            GetSelectedCard().Find("UpgradePrice/PriceText").GetComponent<TMPro.TextMeshProUGUI>().text = "Max";
            return;
        }

        //SaveManager.Unit nextUnit = GetUpgradeUnit(upgradeName);
        string name = upgradeName;
        string effectText = saveManager.GetUnit(upgradeName).effectDescription;
        if (IsAnyUpgradeUnlocked(upgradeName))
        {
            SaveManager.HeroUpgrade upgradeUnit = GetUpgradeHero(upgradeName);
            name = upgradeUnit.name;
            effectText = upgradeUnit.description;
        }

        selectedCardInfos.Find("UnitName").GetComponent<TextMeshProUGUI>().text = name + " : ";
        selectedCardInfos.Find("Description").GetComponent<TextMeshProUGUI>().text = effectText;
    }
    string GetUpgradeNameFromSelectCard()
    {
        return selectedCard.Replace("UpgradeCard", "");
    }

    bool IsUpgradeMax(string upgradeName)
    {
        return float.Parse(upgradeName) == GetMaxUpgradeLevel(upgradeName);
    }

    SaveManager.HeroUpgrade GetUpgradeHero(string upgradeName)
    {
        string name = GetUpgradeNameWithoutNumbers(upgradeName);
        float level = float.Parse(upgradeName.Replace(name, ""));

        float levelCurrent;
        string nameCurrent;
        foreach (SaveManager.HeroUpgrade heroUpgrade in saveManager.heroUpgrades)
        {
            nameCurrent = GetUpgradeNameWithoutNumbers(heroUpgrade.name);
            if (name != nameCurrent)
                break;

            levelCurrent = float.Parse(heroUpgrade.name.Replace(name, ""));
            if (levelCurrent > level)
                return heroUpgrade;
        }
        return null;
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
                break;

            levelCurrent = float.Parse(heroUpgrade.name.Replace(name, ""));
            if (levelCurrent > levelMax)
                levelMax = levelCurrent;
        }
        return levelMax;
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
    Transform GetSelectedCard()
    {
        if (selectedCard == "")
            return null;
        return transform.Find("Buttons/UnitsButtonPanel/" + selectedCard);
    }

    string GetUpgradeNameWithoutNumbers(string unitName)
    {
        return Regex.Replace(unitName, @"[\d-]", string.Empty);
    }

}