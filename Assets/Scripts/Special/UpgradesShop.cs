using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UpgradesShop : MonoBehaviour
{
    private SaveManager saveManager;
    public string selectedCard;
    public GameObject selectCursor;
    public GameObject upgradeButtonPrefab;
    public GameObject chosenUnitPrefab;

    public TMP_ColorGradient tooExpensivePriceColor;
    public TMP_ColorGradient priceOriginalColor;
    public GameObject upgradeEffect;
    public Color32 starsMaxColor;
    Transform buttonsParentTransform;
    private void Start()
    {
        if (!saveManager)
            saveManager = SaveManager.instance;
        buttonsParentTransform = transform.Find(Constants.UNITS_BUTTON_PANEL_PATH);
        GenerateUpgradeButton();
        InitUpgradesCards();
        InitChosenUnitsButtons();
        UpdateShopUI();
        InvokeRepeating(nameof(SetSelectedCardButtonCursor), 0, 0.1f);
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

            if (saveManager.IsModeNormalChosen() && !unit.name.Contains('1'))
                continue;
            GameObject button = Instantiate(upgradeButtonPrefab, buttonsParentTransform);
            button.name = "UpgradeCard" + unit.name;
        }
    }
    void SetPlayerGoldText()
    {
        transform.Find("Money/MoneyText").GetComponent<TextMeshProUGUI>().text = ((int)saveManager.money).ToString();
    }

    void UpdateUpgradesCards()
    {

        foreach (Transform upgradeCardButton in buttonsParentTransform)
        {
            string unitName = upgradeCardButton.name.Replace("UpgradeCard", "");
            SetUpgradeButtonTexts(upgradeCardButton, unitName);
            SaveManager.Unit upgradeUnit = GetUpgradeUnit(unitName);
            if (upgradeUnit != null)
                UpdatePriceTextColor(upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TextMeshProUGUI>(), upgradeUnit.name);
            InitLevelLockMessageText(upgradeCardButton, unitName);

        }
    }


    Transform GetUpgradeButton(string unitName)
    {
        unitName = GetUnitNameWithoutNumbers(unitName);

        foreach (Transform upgradeCardButton in buttonsParentTransform)
        {
            if (upgradeCardButton.name.Contains(unitName))
                return upgradeCardButton;
        }
        return null;
    }

    string GetUnitNameFromButton(string buttonName)
    {
        return buttonName.Replace("UpgradeCard", "");
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
        else if (IsUnitLevelMax(unitName) && !unitName.Contains("Frog"))
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
        if (!buttonsParentTransform.gameObject.activeInHierarchy)
            return;
        GameObject heroCursor = selectCursor.transform.Find("HerosUpgrades").gameObject;
        if (heroCursor.activeSelf)
        {
            heroCursor.SetActive(false);
            selectCursor.transform.Find("UnitsUpgrades").gameObject.SetActive(true);
        }

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



    public void PlayButtonClick()
    {
        AudioManager.instance.PlaySfx(Constants.BUTTON_CLICK_SFX_NAME);
    }

    void AddEventTriggerOnButton(GameObject button, System.Action function)
    {
        EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
        if (eventTrigger is null)
            eventTrigger = button.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new()
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => { function(); });
        eventTrigger.triggers.Add(entry);
    }

    void AddEventTriggerOnChooseButton(GameObject button, string unitName)
    {
        EventTrigger eventTrigger = button.transform.Find("RemoveButton").GetComponent<EventTrigger>();
        if (eventTrigger is null)
            eventTrigger = button.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new()
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => { RemoveChosenUnit(unitName); });

        eventTrigger.triggers.Add(entry);

        eventTrigger = button.transform.Find("ChooseButton").GetComponent<EventTrigger>();
        if (eventTrigger is null)
            eventTrigger = button.AddComponent<EventTrigger>();
        entry = new()
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => { ChooseUnit(unitName); });
        eventTrigger.triggers.Add(entry);
    }

    void AddEventTriggerOnChosenUnitButton(GameObject button)
    {
        EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
        if (eventTrigger is null)
            eventTrigger = button.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new()
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => { RemoveChosenUnitUsingButtonName(button); });
        eventTrigger.triggers.Add(entry);
    }

    void UpdateButtonName(string oldCardName)
    {
        if (selectedCard == "")
            return;
        Transform button = buttonsParentTransform.Find(oldCardName);
        button.name = selectedCard;
    }


    void InitUpgradesCards()
    {
        //Transform unitsButtonPanel = transform.Find("Buttons/UnitsButtonPanel");
        foreach (Transform upgradeCardButton in buttonsParentTransform)
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
            UpdateStarsLock(upgradeCardButton, unitName);
            //if (!saveManager.unlockedUnits.Contains(unitName))
        }
    }



    int GetLockAmount(Transform upgradeCardButton, int level)
    {
        int levelMax = 4;
        int lockCount = 0;
        for (int i = levelMax; i > 0 + 1; i--)
        {
            if (IsUnitLevelUnlocked(i))
                break;
            lockCount++;
        }
        return lockCount;
    }
    void InitLevelLockMessageText(Transform upgradeCardButton, string unitName)
    {
        int maxLevelUnlocked = saveManager.maxLevelUnlocked;
        bool doesMessageNeedToBeShowned = IsUnitOfAnyLevelUnlocked(unitName) && DoesUnitExist(unitName) && GetMaxUnitLevelUnlocked() == GetUnitLevelNumber(unitName);
        if (maxLevelUnlocked >= Constants.UNIT_LEVEL_4_LOCK_UNTIL_STAGE || !doesMessageNeedToBeShowned)
            return;

        string text = "Unlock <color=\"red\"> stage " + GetStageToUnlockNextUnitLevel().ToString();
        upgradeCardButton.Find("LockMessage/Message").GetComponent<TextMeshProUGUI>().text = text;
        upgradeCardButton.Find("LockMessage").gameObject.SetActive(true);
    }

    int GetStageToUnlockNextUnitLevel()
    {
        int maxLevelUnlocked = saveManager.maxLevelUnlocked;
        int stageNumber = 0;
        if (maxLevelUnlocked < Constants.UNIT_LEVEL_2_LOCK_UNTIL_STAGE)
            stageNumber = Constants.UNIT_LEVEL_2_LOCK_UNTIL_STAGE;
        else if (maxLevelUnlocked < Constants.UNIT_LEVEL_3_LOCK_UNTIL_STAGE)
            stageNumber = Constants.UNIT_LEVEL_3_LOCK_UNTIL_STAGE;
        else if (maxLevelUnlocked < Constants.UNIT_LEVEL_4_LOCK_UNTIL_STAGE)
            stageNumber = Constants.UNIT_LEVEL_4_LOCK_UNTIL_STAGE;
        return stageNumber;
    }

    int GetMaxUnitLevelUnlocked()
    {
        int maxLevelUnlocked = saveManager.maxLevelUnlocked;
        if (maxLevelUnlocked < Constants.UNIT_LEVEL_2_LOCK_UNTIL_STAGE)
            return 1;
        else if (maxLevelUnlocked < Constants.UNIT_LEVEL_3_LOCK_UNTIL_STAGE)
            return 2;
        else if (maxLevelUnlocked < Constants.UNIT_LEVEL_4_LOCK_UNTIL_STAGE)
            return 3;
        return 4;
    }

    //void EnableLevelLockMessage(Transform upgradeButton, string unitName)
    //{
    //    if (!IsUnitLevelMax(unitName) && !IsUnitUnlocked(unitName))
    //        upgradeButton.Find("LockMessage").gameObject.SetActive(true);

    //}
    void AddTriggers(Transform upgradeCardButton)
    {
        AddEventTriggerOnButton(upgradeCardButton.Find("Button").gameObject, SelectCard);
        AddEventTriggerOnButton(upgradeCardButton.Find("Button").gameObject, SetSelectedCardButtonCursor);
        AddEventTriggerOnChooseButton(upgradeCardButton.Find("ChooseUnitButton").gameObject, GetUnitNameFromButton(upgradeCardButton.name));
        //AddEventTriggerOnChooseButton(upgradeCardButton.Find("ChooseUnitButton").gameObject, RemoveSpriteBody(upgradeCardButton.Find("ChooseUnitButton").gameObject));
    }

    void CreateUpgradeEffect()
    {
        GameObject effect = Instantiate(upgradeEffect);
        effect.transform.position = selectCursor.transform.position;

    }
    void SetUpgradeButtonName(string unitName, string oldName)
    {
        //string path = "Buttons/UnitsButtonPanel/";
        buttonsParentTransform.Find(oldName).name = "UpgradeCard" + unitName;
    }

    string GetUnlockedUnit(string unitName)
    {
        foreach (string unitNameUnlocked in saveManager.unlockedUnits)
        {
            if (GetUnitNameWithoutNumbers(unitName) == GetUnitNameWithoutNumbers(unitNameUnlocked))
                return unitNameUnlocked;
        }
        return null;
    }




    Transform GetSelectedCard()
    {
        if (selectedCard == "")
            return null;
        return buttonsParentTransform.Find(selectedCard);
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
        if (unitName.Contains("Frog"))
            return saveManager.GetUnit(unitName);

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
        print("HERE:" + upgradeUnitName);
        if (saveManager.IsModeNormalChosen())
        {
            RemoveChosenUnit(unitName);
            saveManager.unlockedUnits.Add(upgradeUnitName);
            saveManager.unlockedUnits.Remove(unitName);
        }
        else
        {
            saveManager.ReplaceUnlockedFrogUnitLevel(unitName);
        }

        saveManager.money -= price;
        ChooseUnit(upgradeUnitName);
    }

    void Unlock(string unitName, float price)
    {
        saveManager.money -= price;
        saveManager.unlockedUnits.Add(unitName);
        ChooseUnit(unitName);
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
            SetActiveUpgradeCardButtonLock(GetSelectedCard(), false);
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
            UnselectCard();
        }
        //UpdateActiveUpgradeCardButtonLevelLockMessage(unitName);
        //EnableLevelLockMessage(GetSelectedCard(), upgradeUnitName);
        AudioManager.instance.PlaySfx(Constants.BUY_SFX_NAME);

    }



    void UpdateActiveUpgradeCardButtonLevelLockMessage(string unitName)
    {
        if (!IsUnitLevelUnlocked(unitName))
            GetSelectedCard().Find("LockMessage").gameObject.SetActive(true);

    }

    bool CanUpgrade(string unitName)
    {
        if (!DoesUnitExist(unitName))
            return false;

        bool enoughMoney = saveManager.money >= GetUnit(unitName).shopPrice;
        if (!saveManager.IsModeNormalChosen() && saveManager.unlockedUnits.Contains(unitName))
        {
            print(unitName);
            int frogLevel = GetFrogUnitLevel(unitName);
            SaveManager.Unit unit = GetUnit(unitName);
            int index = frogLevel - 1;
            float price = unit.shopPrices[index];
            enoughMoney = saveManager.money >= price;
        }
        //return DoesUnitExist(unitName) && enoughMoney && IsUnitUnlocked(unitName);
        return enoughMoney && IsUnitUnlocked(unitName);
    }

    int GetFrogUnitLevel(string frogName)
    {
        frogName = GetUnitNameWithoutNumbers(frogName);
        foreach (string name in SaveManager.instance.unlockedFrogUnitsLevel)
        {
            if (GetUnitNameWithoutNumbers(name) == frogName)
                return (int)GetUnitNameNumbersOnly(name);
        }
        return -1;
    }

    float GetUnitNameNumbersOnly(string unitName)
    {
        string withoutNumbers = GetUnitNameWithoutNumbers(unitName);
        withoutNumbers = unitName.Replace(withoutNumbers, "");
        return float.Parse(withoutNumbers, CultureInfo.InvariantCulture.NumberFormat);
    }

    bool IsUnitUnlocked(string unitName)
    {
        //return !UnitAlreadyUnlocked(unitName) && (IsUnitLevelUnlocked(unitName));
        return !UnitAlreadyUnlocked(unitName) && (!IsUnitOfAnyLevelUnlocked(unitName) || IsUnitLevelUnlocked(unitName));
    }

    bool IsUnitLevelUnlocked(int unitLevel)
    {
        if (unitLevel == 2)
            return saveManager.maxLevelUnlocked >= Constants.UNIT_LEVEL_2_LOCK_UNTIL_STAGE;
        else if (unitLevel == 3)
            return saveManager.maxLevelUnlocked >= Constants.UNIT_LEVEL_3_LOCK_UNTIL_STAGE;

        return saveManager.maxLevelUnlocked >= Constants.UNIT_LEVEL_4_LOCK_UNTIL_STAGE;
    }

    //bool IsUnitNextLevelLocked(string unitName)
    //{
    //    if (!IsUnitOfAnyLevelUnlocked(unitName))
    //        return true;

    //    int unitLevel = int.Parse(GetUnitLevel(unitName)) + 1;
    //    return IsUnitLevelUnlocked(unitLevel);
    //}

    bool IsUnitLevelUnlocked(string unitName)
    {
        if (!IsUnitOfAnyLevelUnlocked(unitName))
            return true;
        int unitLevel = int.Parse(GetUnitLevel(unitName));
        return IsUnitLevelUnlocked(unitLevel);
    }


    void UpdateStarsLock(Transform upgradeCardButton, int level)
    {
        Transform stars = upgradeCardButton.Find("StarsLock");
        int levelMax = 4;
        for (int i = levelMax; i > 0 + 1; i--)
        {
            if (IsUnitLevelUnlocked(i))
                break;
            string starName = "Star" + i.ToString();
            string lockName = starName + "/Lock";
            Transform lockTransform = stars.Find(lockName);
            GameObject starUnlocked = lockTransform.gameObject;
            starUnlocked.SetActive(true);
        }
    }

    void UpdateStarsLock(Transform upgradeCardButton, string unitName)
    {
        UpdateStarsLock(upgradeCardButton, int.Parse(GetUnitLevel(unitName)));
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


    void SetUpgradeButtonImage(Transform upgradeCardButton, string unitName)
    {
        if (saveManager.IsModeNormalChosen())
        {
            upgradeCardButton.Find("UnitSprite").GetComponent<Image>().sprite = (Sprite)Resources.Load(Constants.UNITS_SPRITE_RESOURCES_PATH + '/' + unitName);
            upgradeCardButton.Find("UnitSprite").gameObject.SetActive(true);
        }
        else
        {
            //Skip if already exist (frog unit doesn't need image update when upgrading).
            if (upgradeCardButton.Find("SpriteBody"))
                return;

            upgradeCardButton.Find("UnitSprite").gameObject.SetActive(false);
            InstantiateSpriteBodyFromPrefabWithImage(unitName, upgradeCardButton);

        }
    }
    Transform InstantiateSpriteBodyFromPrefabWithImage(string unitName, Transform upgradeCardButton)
    {

        string path = SaveManager.instance.GetUnitsPrefabRessourcePath() + '/' + unitName;
        GameObject frogPrefab = (GameObject)Resources.Load(path);
        Transform spriteBody = InstantiateSpriteBodyFromPrefab(frogPrefab, upgradeCardButton);
        foreach (SpriteRenderer spriteRenderer in spriteBody.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.transform.parent = spriteBody;
        }
        foreach (Transform child in spriteBody)
        {
            Debug.Log(child.name);
            if (!child.GetComponent<SpriteRenderer>())
                //if (!child.GetComponent<SpriteRenderer>() || child.name.Equals("Shadow"))
                Destroy(child.gameObject);
        }

        SpriteRenderer[] sprites = spriteBody.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sp in sprites)
        {
            sp.sortingLayerName = "UI";
        }
        spriteBody.localScale = new Vector2(1118, 1118);
        spriteBody.localPosition = new Vector2(70, 133);

        ReplaceSpriteComponentByImageComponent(spriteBody);
        return spriteBody;
    }

    Transform InstantiateSpriteBodyFromPrefab(GameObject prefab, Transform parent)
    {
        string spriteName = "SpriteBody";
        if (prefab.transform.Find("SpriteBody") == null)
            spriteName = "Sprite";
        Transform spriteBody = Instantiate(prefab.transform.Find(spriteName), parent.transform);
        if (spriteBody == null)

        spriteBody.name = spriteBody.name.Replace("(Clone)", "");
        return spriteBody;
    }

    void ReplaceSpriteComponentByImageComponent(Transform spriteBody)
    {
        CreateImageFromSprite(spriteBody);
        MoveSpriteBodyAboveLockTransform(spriteBody);
        OrderChildIndexUsingSpriteOrder(spriteBody);
        SetImagesInteractableToFalse(spriteBody);
    }

    void CreateImageFromSprite(Transform spriteBody)
    {
        SpriteRenderer[] sprites = spriteBody.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sp in sprites)
        {
            Image imgComp = sp.gameObject.AddComponent<Image>();
            imgComp.sprite = sp.sprite;
            imgComp.color = sp.color;
            if (sp.flipX)
                sp.transform.RotateAround(sp.transform.position, Vector3.up, 180f);
            if (sp.sortingOrder < 0)
                imgComp.transform.SetAsFirstSibling();
            else
                imgComp.transform.SetSiblingIndex(sp.sortingOrder);
            Destroy(sp);
        }
        if (!SaveManager.instance.IsModeNormalChosen())
            spriteBody.transform.Rotate(0, 180, 0);
    }

    void MoveSpriteBodyAboveLockTransform(Transform spriteBody)
    {
        foreach (Transform t in spriteBody.parent)
        {
            if (t.name.Equals("Lock"))
            {
                spriteBody.SetSiblingIndex(t.GetSiblingIndex() - 1);
                break;
            }
        }
    }


    void OrderChildIndexUsingSpriteOrder(Transform spriteBody)
    {
        SpriteRenderer[] spriteRenderers = spriteBody.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 1; i < spriteRenderers.Length; i++)
        {
            for (int y = 0; y < spriteRenderers.Length; y++)
            {
                float spOrder1 = spriteRenderers[i - 1].sortingOrder;
                float spOrder2 = spriteRenderers[i].sortingOrder;

                if (spOrder1 > spOrder2)
                    spriteRenderers[i].transform.SetSiblingIndex(i - 1);
            }
        }
    }

    void SetImagesInteractableToFalse(Transform imageParent)
    {
        Image[] images = imageParent.GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            image.raycastTarget = false;
        }
    }

    void SetUpgradeButtonTexts(Transform upgradeCardButton, string unitName)
    {
        string nameWithoutNumbers = GetUnitNameWithoutNumbers(unitName);
        //upgradeCardButton.Find("UnitSprite").GetComponent<Image>().sprite = (Sprite)Resources.Load(Constants.UNITS_SPRITE_RESOURCES_PATH + '/' + unitName);
        SetUpgradeButtonImage(upgradeCardButton, unitName);

        upgradeCardButton.Find("Title/TitleText").GetComponent<TextMeshProUGUI>().text = nameWithoutNumbers.Replace("Unit", "");
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
            //upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TMPro.TextMeshProUGUI>().text = "Max";
            upgradeCardButton.Find("UpgradePrice").gameObject.SetActive(false);
            UpdateCardStars(upgradeCardButton, int.Parse(lvl));
            DisableUpgradeCard(upgradeCardButton.Find("Button").GetComponent<Button>());
        }
        else
            UpdateCardStars(upgradeCardButton, int.Parse(lvl));
        //upgradeCardButton.Find("LevelText").GetComponent<TextMeshProUGUI>().text = "Lvl " + lvl + "/4";

        TextMeshProUGUI priceText = upgradeCardButton.Find("UpgradePrice/PriceText").GetComponent<TextMeshProUGUI>();
        if (!IsUnitLevelMax(unitName))
        {
            priceText.text = price;
            //UpdatePriceTextColor(priceText, upgradeUnit.name, upgradeUnit.shopPrice[0]);
        }

    }
    void UpdateCardStars(Transform upgradeCardButton, int level)
    {
        TextMeshProUGUI levelText = upgradeCardButton.Find("Level/LevelText").GetComponent<TextMeshProUGUI>();
        int levelMax = 4;
        if (!saveManager.IsModeNormalChosen())
            levelMax = 10;
        levelText.text = string.Concat("Lvl ", level.ToString(), "/", levelMax.ToString());


        //for (int i = 1; i < level + 1; i++)
        //{
        //    string starName = "Star" + i.ToString();
        //    GameObject starUnlocked = stars.Find(starName + "/StarUnlocked").gameObject;
        //    starUnlocked.SetActive(true);
        //    if (level == levelMax)
        //        starUnlocked.GetComponent<Image>().color = starsMaxColor;
        //}
    }

    void UpdateCardStarsOLD(Transform upgradeCardButton, int level)
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


    void UpdateChosenUnitButtonManaCost(Transform unit, string unitName)
    {
        unit.Find("CostText").GetComponent<TextMeshProUGUI>().text = GetUnit(unitName).cost.ToString();
    }

    void UpdatePriceTextColor(TextMeshProUGUI text, string unitName)
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

    int GetUnitLevelNumber(string unitName)
    {
        return int.Parse(GetUnitLevel(unitName));
    }

    public void ChooseUnit(string unitName)
    {
        unitName = GetUnlockedUnit(unitName);
        if (saveManager.chosenUnits.Contains(unitName) || saveManager.chosenUnits.Count >= Constants.CHOSEN_UNIT_MAX || unitName == null)
            return;
        AddChosenUnit(unitName);
    }

    void AddChosenUnit(string unitName)
    {
        Transform btn = GetUpgradeButton(unitName).Find("ChooseUnitButton");
        btn.Find("ChooseButton").gameObject.SetActive(false);
        btn.Find("RemoveButton").gameObject.SetActive(true);
        saveManager.chosenUnits.Add(unitName);
        RemoveSpriteBody(btn);
        UpdateChosenUnitList();
    }

    public void RemoveChosenUnit(string unitName)
    {
        unitName = GetUnlockedUnit(unitName);
        if (!saveManager.chosenUnits.Contains(unitName))
            return;
        Transform btn = GetUpgradeButton(unitName).Find("ChooseUnitButton");
        btn.Find("ChooseButton").gameObject.SetActive(true);
        btn.Find("RemoveButton").gameObject.SetActive(false);
        saveManager.chosenUnits.Remove(unitName);
        UpdateChosenUnitList();
    }


    public void RemoveChosenUnitUsingButtonName(GameObject button)
    {
        RemoveChosenUnit(button.name.Replace("ChosenUnit", ""));
        RemoveSpriteBody(button.transform);
        UpdateChosenUnitList();
    }

    void RemoveSpriteBody(Transform button)
    {
        Transform spriteBody = button.Find("SpriteBody");
        if (spriteBody)
            Destroy(spriteBody.gameObject);
    }

    void UpdateChosenUnitList()
    {
        int i = 0;
        int count = saveManager.chosenUnits.Count;
        string unitName;
        //string spritePath = Constants.UNITS_SPRITE_RESOURCES_PATH + '/';
        Transform unit;
        Transform chosenUnitButton;
        Transform chosenUnitsParent = transform.Find("Buttons/ChosenUnits");

        foreach (Transform button in chosenUnitsParent.transform)
        {
            chosenUnitButton = chosenUnitsParent.GetChild(i);
            unit = chosenUnitButton.transform.Find("Unit");
            if (i >= count)
            {
                unit.gameObject.SetActive(false);
                RemoveSpriteBody(button);
                //break;
            }
            else
            {
                unitName = saveManager.chosenUnits[i];
                Transform spriteBody = button.Find("SpriteBody");
                foreach (Transform child in button)
                {
                    if (child.name.Equals("SpriteBody"))
                        Destroy(child.gameObject);
                }


                spriteBody = InstantiateSpriteBodyFromPrefabWithImage(unitName, button);
                spriteBody.localScale = new Vector2(228, 228);
                spriteBody.localPosition = new Vector2(4.7f, 13.3f);

                chosenUnitButton.name = "ChosenUnit" + unitName;
                unit.gameObject.SetActive(true);

                //UpdateCardStars(unit.Find("StarsCanvas"), GetUnitLevelNumber(unitName));
                UpdateChosenUnitButtonManaCost(unit, unitName);
                i++;
            }
        }
        if (i == Constants.CHOSEN_UNIT_MAX)
            DisableUnselectedChoseButtons();
        else
            EnableUnselectedChoseButtons();
    }

    void DisableUnselectedChoseButtons()
    {
        foreach (Transform button in buttonsParentTransform)
        //foreach (string unitName in saveManager.unlockedUnits)
        {
            string unitName = GetUnitNameFromButton(button.name);
            if (!saveManager.chosenUnits.Contains(unitName))
            {
                //GetUpgradeButton(unitName).Find("ChooseUnitButton").gameObject.SetActive(false);
                GetUpgradeButton(unitName).Find("ChooseUnitButton/ChooseButton").GetComponent<Button>().interactable = false;
            }

        }
    }

    void EnableUnselectedChoseButtons()
    {
        foreach (string unitName in saveManager.unlockedUnits)
        {
            if (!saveManager.chosenUnits.Contains(unitName))
                //GetUpgradeButton(unitName).Find("ChooseUnitButton").gameObject.SetActive(true);
                GetUpgradeButton(unitName).Find("ChooseUnitButton/ChooseButton").GetComponent<Button>().interactable = true;
        }
    }

    void InitChosenUnitsButtons()
    {
        Transform chosenUnitsParent = transform.Find("Buttons/ChosenUnits");
        for (int i = 0; i < Constants.CHOSEN_UNIT_MAX; i++)
        {
            GameObject btn = Instantiate(chosenUnitPrefab, chosenUnitsParent);
            btn.name = btn.name.Replace("(Clone)", "");

            AddEventTriggerOnChosenUnitButton(btn);
            btn.transform.Find("Unit").gameObject.SetActive(false);
        }

        foreach (string unitName in saveManager.chosenUnits)
        {
            Transform btn = GetUpgradeButton(unitName).Find("ChooseUnitButton");
            btn.Find("ChooseButton").gameObject.SetActive(false);
            btn.Find("RemoveButton").gameObject.SetActive(true);
            UpdateChosenUnitList();
        }
    }

}