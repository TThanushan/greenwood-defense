using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroAbilitiesManager : MonoBehaviour
{

    public AbilityButton[] abilityButtons;
    public GameObject buttonPrefab;
    public GameObject lightingEffect;
    public GameObject damageBuffEffect;
    public GameObject stunEffect;

    private void Start()
    {
        InitAbilityButtons();
        GenerateButtons();
        //OrderChildButtonsByCost();
    }

    private void Update()
    {
        UpdateAbilityButtons();
    }

    void InitAbilityButtons()
    {
        SaveManager saveManager = SaveManager.instance;
        List<string> abilitiesName = GetUnlockedAbilitiesName();
        abilityButtons = new AbilityButton[abilitiesName.Count];
        float cooldownReduction = 1 - GetCooldownReductionShop() / 100;
        int i = 0;
        foreach (string AbilityName in abilitiesName)
        {
            SaveManager.HeroUpgrade Ability = saveManager.GetHeroUpgrade(AbilityName);
            AbilityButton AbilityButton = new AbilityButton(Ability.name, Ability.cost, Ability.reloadTime * cooldownReduction);
            abilityButtons[i] = AbilityButton;
            i++;
        }
    }

    List<string> GetUnlockedAbilitiesName()
    {
        List<string> abilities = new List<string>();
        foreach (string unlockedHeroUpgrade in SaveManager.instance.unlockedHeroUpgrades)
        {
            if (unlockedHeroUpgrade.Contains("Ability") && !unlockedHeroUpgrade.Contains("0"))
                abilities.Add(unlockedHeroUpgrade);
        }
        return abilities;
    }
    float GetCooldownReductionShop()
    {

        foreach (string name in SaveManager.instance.unlockedHeroUpgrades)
        {
            if (GetUpgradeNameWithoutNumbers(name) == "AbilityCooldownReduction")
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

    //private Transform GetCorrespondingChild(string name)
    //{
    //    foreach (Transform child in transform)
    //    {
    //        if (child.name.Contains(name))
    //            return child;
    //    }
    //    return null;
    //}

    private void UpdateAbilityButtons()
    {
        foreach (AbilityButton AbilityButton in abilityButtons)
        {
            AbilityButton.Update();
        }
    }

    string GetSimplifiedAbilityName(string name)
    {
        return GetUpgradeNameWithoutNumbers(name).Replace("Ability", "");
    }
    void ResetAbilityCooldown(string name)
    {
        AbilityButton abilityButton = GetAbility(name);
        if (abilityButton != null)
            abilityButton.ResetCurrentReloadTime();
    }

    void GenerateButtons()
    {

        foreach (AbilityButton AbilityButton in abilityButtons)
        {
            GameObject button = Instantiate(buttonPrefab, transform);
            SetButtonName(button, AbilityButton);
            SetButtonSprite(button, GetUpgradeNameWithoutNumbers(AbilityButton.name));
            //SetButtonPrice(button, AbilityButton.cost);
            SetButtonReloadBarAndEnougManaShade(button, AbilityButton);
            AddEventTriggerOnButton(button, AbilityButton);
            EnableButtonStars(button, AbilityButton);
        }
    }


    void SetButtonReloadBarAndEnougManaShade(GameObject button, AbilityButton AbilityButton)
    {
        AbilityButton.reloadBar = button.transform.Find("ReloadBar/Bar");
        //AbilityButton.enoughManaShade = button.transform.Find("EnoughManaShade").gameObject;

    }

    //void SetButtonPrice(GameObject button, float cost)
    //{
    //    button.transform.Find("CostText").GetComponent<TMPro.TextMeshProUGUI>().text = cost.ToString();
    //}

    System.Action GetAbilityFunctionToAdd(string name)
    {
        name = GetSimplifiedAbilityName(name);
        if (name == "RandomSpawn")
            return RandomSpawn;
        else if (name == "DamageBuff")
            return DamageBuff;
        else if (name == "Paralysis")
            return Paralysis;
        else if (name == "Lightning")
            return Lightning;
        print("null func");
        return null;

    }

    void UseAbility(string name)
    {
        if (!GetAbility(name).ReadyToUse())
            return;

        System.Action callAbility = GetAbilityFunctionToAdd(name);
        callAbility();
        ResetAbilityCooldown(name);
    }

    void DamageBuff()
    {
        print("DamageBuff");
        PoolObject poolObject = PoolObject.instance;
        float damageCoef = 2.5f;
        float damageBonus = GetUpgradeNameNumbersOnly(GetAbility("DamageBuff").name) * damageCoef;

        foreach (GameObject ally in PoolObject.instance.Allies)
        {
            Unit unit = ally.GetComponent<Unit>();
            if (!unit || unit.Disabled)
                continue;
            float duration = 10f;
            unit.BuffAttackDamage(damageBonus, duration);
            GameObject effect = poolObject.GetPoolObject(damageBuffEffect);
            effect.GetComponent<MoveTowardTarget>().target = unit.gameObject;
            effect.GetComponent<DisableScript>().disableTime = duration;

            // Reset disable time;
            effect.SetActive(false);
            effect.SetActive(true);

        }
    }

    void Paralysis()
    {
        print("Paralysis");
        PoolObject poolObject = PoolObject.instance;
        float duration = 1.5f;
        float paralyseDuration = GetUpgradeNameNumbersOnly(GetAbility("Paralysis").name) * duration;

        foreach (GameObject enemy in PoolObject.instance.Enemies)
        {
            Unit unit = enemy.GetComponent<Unit>();
            if (!unit || unit.Disabled || enemy.name.Contains("FrogTrap"))
                continue;

            unit.ParalyseEffect(true);
            StartCoroutine(Paralyse(unit, paralyseDuration));

            GameObject effect = poolObject.GetPoolObject(stunEffect);
            effect.GetComponent<MoveTowardTarget>().target = unit.gameObject;
            effect.GetComponent<DisableScript>().disableTime = paralyseDuration;

            // Reset disable time;
            effect.SetActive(false);
            effect.SetActive(true);
        }
    }

    IEnumerator Paralyse(Unit unit, float duration)
    {
        unit.ParalyseEffect(true);
        yield return new WaitForSeconds(duration);
        unit.ParalyseEffect(false);

    }

    void Lightning()
    {
        PoolObject poolObject = PoolObject.instance;
        print("Lightning");
        float lightningDamage = 10f;
        float damage = GetUpgradeNameNumbersOnly(GetAbility("Lightning").name) * lightningDamage;
        foreach (GameObject enemy in PoolObject.instance.Enemies)
        {
            print(enemy.name);
            Unit unit = enemy.GetComponent<Unit>();
            if (!unit || unit.Disabled || enemy.name == "EnemyCaptain")
                continue;
            unit.GetDamage(damage, null);
            poolObject.GetPoolObject(lightingEffect).transform.position = new Vector2(unit.transform.position.x, unit.transform.position.y + 0.5f);

        }
    }
    void RandomSpawn()
    {
        int count = (int)GetUpgradeNameNumbersOnly(GetAbility("RandomSpawn").name);
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, SaveManager.instance.unlockedUnits.Count);
            string name = SaveManager.instance.unlockedUnits[index];
            GameObject.Find("SpawnBar").GetComponent<SpawnBar>().SpawnUnitForFree(name);

        }
    }

    void AddEventTriggerOnButton(GameObject button, AbilityButton abilityButton)
    {
        EventTrigger eventTrigger = button.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };
        //System.Action ability = GetAbilityFunctionToAdd(GetUpgradeNameWithoutNumbers(abilityButton.name));
        entry.callback.AddListener((eventData) => { UseAbility(GetSimplifiedAbilityName(abilityButton.name)); });
        entry.callback.AddListener((eventData) => { AudioManager.instance.PlaySfx(Constants.BUTTON_CLICK_SFX_NAME); });
        eventTrigger.triggers.Add(entry);
    }
    void SetButtonName(GameObject button, AbilityButton AbilityButton)
    {
        button.name = buttonPrefab.name;
        button.name += AbilityButton.name;
    }

    void SetButtonSprite(GameObject button, string spriteName)
    {
        button.transform.Find("AbilitySprite").GetComponent<Image>().sprite = (Sprite)Resources.Load(Constants.HERO_ABILITIES_SPRITE_RESOURCES_PATH + '/' + spriteName);
    }
    void EnableButtonStars(GameObject button, AbilityButton AbilityButton)
    {
        Transform panel = button.transform.Find("StarsCanvas/Panel");
        if (AbilityButton.name.Contains("2"))
        {
            panel.Find("Star2").gameObject.SetActive(true);
        }
        if (AbilityButton.name.Contains("3"))
        {
            panel.Find("Star2").gameObject.SetActive(true);
            panel.Find("Star3").gameObject.SetActive(true);
        }
        if (AbilityButton.name.Contains("4"))
        {
            panel.Find("Star2").gameObject.SetActive(true);
            panel.Find("Star3").gameObject.SetActive(true);
            panel.Find("Star4").gameObject.SetActive(true);
        }
    }

    AbilityButton GetAbility(string name)
    {
        name = GetSimplifiedAbilityName(name);
        foreach (AbilityButton abilityButton in abilityButtons)
        {
            if (GetSimplifiedAbilityName(abilityButton.name) != name)
                continue;
            return abilityButton;
        }
        return null;
    }

    [System.Serializable]
    public class AbilityButton
    {
        public string name;
        public float cost;
        public float reloadTime;

        [HideInInspector]
        public GameObject prefab;
        [HideInInspector]
        public Transform reloadBar;
        [HideInInspector]
        public GameObject enoughManaShade;

        private float currentReloadTime = 0f;

        public AbilityButton(string name, float cost, float reloadTime)
        {
            this.name = name;
            this.cost = cost;
            this.reloadTime = reloadTime;
        }

        public void Update()
        {
            UpdateCurrentReloadTime();
            UpdateReloadBarLength();
            //UpdateEnoughManaShade();
        }

        public bool ReadyToUse()
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

        //private void UpdateEnoughManaShade()
        //{
        //    if (!enoughManaShade) return;
        //    if (HasEnoughMana() && enoughManaShade.activeSelf)
        //        enoughManaShade.SetActive(false);
        //    else if (!HasEnoughMana() && !enoughManaShade.activeSelf)
        //        enoughManaShade.SetActive(true);
        //}

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
