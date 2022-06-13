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

    private void Start()
    {
        if (!saveManager)
            saveManager = SaveManager.instance;
        UpdateUpgradesCards();
        SetPlayerGoldText();
    }

    public void SelectCard()
    {
        print("select card");
        print(EventSystem.current.currentSelectedGameObject.name);
        selectedCard = EventSystem.current.currentSelectedGameObject.name;
    }

    void SetPlayerGoldText()
    {
        transform.Find("Money/MoneyText").GetComponent<TextMeshProUGUI>().text = PlayerStatsScript.instance.money.ToString() + '$';
    }
    public void UnlockUnit(string unitName)
    {
        if (!DoesUnitExist(unitName) || UnitAlreadyUnlocked(unitName))
            return;

        int price = int.Parse(GetUnitPrice(unitName));
        if (PlayerStatsScript.instance.money >= price)
        {
            PlayerStatsScript.instance.money -= price;
            SaveManager.instance.unlockedUnits.Add(unitName);
        }
    }

    bool UnitAlreadyUnlocked(string unitName)
    {
        return saveManager.unlockedUnits.Contains(unitName);
    }
    bool DoesUnitExist(string name)
    {
        return SaveManager.instance.units.Where(p => p.name == name) != null;
    }


    void UpdateUpgradesCards()
    {
        Transform buttonsPanel = transform.Find("Buttons/UnitsButtonPanel");
        foreach (string unitName in saveManager.unlockedUnits)
        {
            foreach (Transform button in buttonsPanel)
            {
                string nameWithoutNumbers = Regex.Replace(unitName, @"[\d-]", string.Empty);

                if (button.name.Contains(nameWithoutNumbers))
                {
                    // Set unit sprite.
                    button.Find("UnitSprite").GetComponent<Image>().sprite = (Sprite)Resources.Load(Constants.UNITS_SPRITE_RESOURCES_PATH + '/' + nameWithoutNumbers);

                    // Set title.
                    button.Find("Title").GetComponent<TextMeshProUGUI>().text = nameWithoutNumbers;

                    //Disable lock image.
                    button.Find("Lock").gameObject.SetActive(false);

                    //Set Level.
                    button.Find("LevelText").GetComponent<TextMeshProUGUI>().text = "Lvl " + GetUnitLevel(unitName) + "/4";

                    // Set upgrade price;
                    button.Find("UpgradePrice/PriceText").GetComponent<TMPro.TextMeshProUGUI>().text = GetUnitPrice(unitName) + '$';

                }
            }
        }
    }

    string GetUnitPrice(string unitName)
    {
        foreach (SaveManager.Unit unit in saveManager.units)
        {
            if (unit.name == unitName)
                return unit.upgradePrice.ToString();
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
