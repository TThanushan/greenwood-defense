using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnBar : MonoBehaviour
{
    public static SpawnBar instance;
    public Vector2 spawnPosition;
    public UnitButton[] unitButtons;
    public GameObject buttonPrefab;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitUnitButtons();
        GenerateButtons();
        OrderChildButtonsByCost();
    }

    private void Update()
    {
        UpdateUnitButtons();
    }

    void InitUnitButtons()
    {
        SaveManager saveManager = SaveManager.instance;
        unitButtons = new UnitButton[saveManager.unlockedUnits.Count];
        float cooldownReduction = 1 - GetCooldownReductionShop() / 100;
        int i = 0;
        foreach (string unitName in saveManager.unlockedUnits)
        {
            SaveManager.Unit unitS = saveManager.GetUnit(unitName);
            UnitButton unitButton = new UnitButton(unitS.name, unitS.cost, unitS.reloadTime * cooldownReduction);
            unitButtons[i] = unitButton;
            i++;
        }
    }

    public void ReduceMaxCooldown(float percentageReduction)
    {
        foreach (UnitButton unitButton in unitButtons)
        {
            unitButton.reloadTime *= 1f - percentageReduction / 100f;
        }
    }
    public void IncreaseMaxCooldown(float percentageReduction)
    {
        foreach (UnitButton unitButton in unitButtons)
        {
            unitButton.reloadTime *= 1f + percentageReduction / 100f;
        }
    }

    public void ResetUnitButtonsMaxReloadTime()
    {
        foreach (UnitButton unitButton in unitButtons)
        {
            unitButton.reloadTime = unitButton.reloadTimeInitial;
        }
    }

    float GetCooldownReductionShop()
    {

        foreach (string name in SaveManager.instance.unlockedHeroUpgrades)
        {
            if (GetUpgradeNameWithoutNumbers(name) == "UnitCooldownReduction")
                return GetUpgradeNameNumbersOnly(name);
        }
        return 0f;

    }

    string GetUpgradeNameWithoutNumbers(string upgradeName)
    {
        string withoutNumbers = Regex.Replace(upgradeName, @"[\d-]", string.Empty);
        withoutNumbers = withoutNumbers.Replace(".", "");
        return withoutNumbers;
    }

    float GetUpgradeNameNumbersOnly(string upgradeName)
    {
        string withoutNumbers = GetUpgradeNameWithoutNumbers(upgradeName);
        withoutNumbers = upgradeName.Replace(withoutNumbers, "");
        return float.Parse(withoutNumbers, CultureInfo.InvariantCulture.NumberFormat);
    }

    private Transform GetCorrespondingChild(string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains(name))
                return child;
        }
        return null;
    }

    string GetSimplifiedName(string name)
    {
        return GetUpgradeNameWithoutNumbers(name).Replace("UnitButton", "");
    }

    public void SpawnUnit(string name)
    {
        name = GetSimplifiedName(name);
        foreach (UnitButton unitButton in unitButtons)
        {
            if (GetSimplifiedName(unitButton.name) == name && unitButton.ReadyToSpawn() && unitButton.HasEnoughMana())
            {
                SpawnUnit(unitButton.prefab);
                unitButton.ResetCurrentReloadTime();
                ManaBar.instance.UseMana(unitButton.cost);
            }
        }
    }

    UnitButton GetUnitButton(string name)
    {
        name = GetSimplifiedName(name);
        foreach (UnitButton unitButton in unitButtons)
        {
            if (GetSimplifiedName(unitButton.name) == name)
                return unitButton;
        }
        return null;
    }

    public void SpawnUnitForFree(string name)
    {
        SpawnUnit(GetUnitButton(name).prefab);
    }
    private void SpawnUnit(GameObject prefab)
    {
        GameObject newUnit = PoolObject.instance.GetPoolObject(prefab);
        float randomYPos = spawnPosition.y + UnityEngine.Random.Range(-0.1f, 0.3f);
        newUnit.transform.position = new Vector2(spawnPosition.x, randomYPos);
    }

    private void UpdateUnitButtons()
    {
        foreach (UnitButton unitButton in unitButtons)
        {
            unitButton.Update();
        }
    }

    public void OrderChildButtonsByCost()
    {
        float lowest = Mathf.Infinity;
        foreach (UnitButton unitButton in unitButtons)
        {
            if (unitButton.cost <= lowest)
            {
                lowest = unitButton.cost;
                GetCorrespondingChild(unitButton.name).SetAsFirstSibling();
            }
        }
    }

    void GenerateButtons()
    {

        foreach (UnitButton unitButton in unitButtons)
        {
            GameObject button = Instantiate(buttonPrefab, transform);
            SetButtonName(button, unitButton);
            SetButtonSprite(button, unitButton.name);
            SetButtonPrefab(unitButton);
            SetButtonPrice(button, unitButton.cost);
            SetButtonReloadBarAndEnougManaShade(button, unitButton);
            AddEventTriggerOnButton(button, unitButton);
            EnableButtonStars(button, unitButton);
        }
    }
    void SetButtonReloadBarAndEnougManaShade(GameObject button, UnitButton unitButton)
    {
        unitButton.reloadBar = button.transform.Find("ReloadBar/Bar");
        unitButton.enoughManaShade = button.transform.Find("EnoughManaShade").gameObject;

    }

    void SetButtonPrice(GameObject button, float cost)
    {
        button.transform.Find("CostText").GetComponent<TMPro.TextMeshProUGUI>().text = cost.ToString();
    }

    void AddEventTriggerOnButton(GameObject button, UnitButton unitButton)
    {
        EventTrigger eventTrigger = button.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        entry.callback.AddListener((eventData) => { SpawnUnit(unitButton.name); });
        entry.callback.AddListener((eventData) => { AudioManager.instance.PlaySfx(Constants.BUTTON_CLICK_SFX_NAME); });
        eventTrigger.triggers.Add(entry);
    }
    void SetButtonName(GameObject button, UnitButton unitButton)
    {
        button.name = buttonPrefab.name;
        button.name += unitButton.name;
    }

    void SetButtonPrefab(UnitButton unitButton)
    {
        string path = Constants.UNITS_PREFAB_RESOURCES_PATH + '/' + unitButton.name;
        unitButton.prefab = (GameObject)Resources.Load(path);
    }

    void SetButtonSprite(GameObject button, string spriteName)
    {
        button.transform.Find("UnitSprite").GetComponent<Image>().sprite = (Sprite)Resources.Load(Constants.UNITS_SPRITE_RESOURCES_PATH + '/' + spriteName);
    }
    void EnableButtonStars(GameObject button, UnitButton unitButton)
    {
        Transform panel = button.transform.Find("StarsCanvas/Panel");
        if (unitButton.name.Contains("2"))
        {
            panel.Find("Star2").gameObject.SetActive(true);
        }
        if (unitButton.name.Contains("3"))
        {
            panel.Find("Star2").gameObject.SetActive(true);
            panel.Find("Star3").gameObject.SetActive(true);
        }
        if (unitButton.name.Contains("4"))
        {
            panel.Find("Star2").gameObject.SetActive(true);
            panel.Find("Star3").gameObject.SetActive(true);
            panel.Find("Star4").gameObject.SetActive(true);
        }
    }

    [System.Serializable]
    public class UnitButton
    {
        public string name;
        public float cost;
        public float reloadTime;
        public float reloadTimeInitial;

        [HideInInspector]
        public GameObject prefab;
        [HideInInspector]
        public Transform reloadBar;
        [HideInInspector]
        public GameObject enoughManaShade;

        private float currentReloadTime = 0f;

        public UnitButton(string name, float cost, float reloadTime)
        {
            this.name = name;
            this.cost = cost;
            this.reloadTime = reloadTime;
            reloadTimeInitial = reloadTime;
        }

        public void Update()
        {
            UpdateCurrentReloadTime();
            UpdateReloadBarLength();
            UpdateEnoughManaShade();
        }

        public bool ReadyToSpawn()
        {
            return currentReloadTime <= 0f;
        }

        public void ResetCurrentReloadTime()
        {
            currentReloadTime = reloadTime;
        }

        public bool HasEnoughMana()
        {
            return ManaBar.instance.currentMana >= cost;
        }

        private void UpdateEnoughManaShade()
        {
            if (!enoughManaShade) return;
            if (HasEnoughMana() && enoughManaShade.activeSelf)
                enoughManaShade.SetActive(false);
            else if (!HasEnoughMana() && !enoughManaShade.activeSelf)
                enoughManaShade.SetActive(true);
        }

        private void UpdateCurrentReloadTime()
        {
            if (currentReloadTime > 0f)
                currentReloadTime -= Time.deltaTime;
            else
                currentReloadTime = 0f;
        }

        private void UpdateReloadBarLength()
        {
            if (!reloadBar) return;
            reloadBar.localScale = new Vector2(1f, currentReloadTime / reloadTime);
        }
    }

}
