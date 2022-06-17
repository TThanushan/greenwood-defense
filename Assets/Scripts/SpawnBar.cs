using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnBar : MonoBehaviour
{
    public Vector2 spawnPosition;
    public UnitButton[] unitButtons;
    public GameObject buttonPrefab;
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
        int i = 0;
        foreach (string unitName in saveManager.unlockedUnits)
        {
            SaveManager.Unit unitS = saveManager.GetUnit(unitName);
            UnitButton unitButton = new UnitButton(unitS.name, unitS.cost, unitS.reloadTime);
            unitButtons[i] = unitButton;
            i++;
        }
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

    public void SpawnUnit(string name)
    {
        foreach (UnitButton unitButton in unitButtons)
        {
            if (unitButton.name == name && unitButton.ReadyToSpawn() && unitButton.HasEnoughMana())
            {
                SpawnUnit(unitButton.prefab);
                unitButton.ResetCurrentReloadTime();
                ManaBar.instance.UseMana(unitButton.cost);
            }
        }
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
        for (int i = 0; i < 5; i++)
        {
            spriteName = spriteName.Replace(i.ToString(), "");
        }

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(spawnPosition, 0.25f);
    }
}
