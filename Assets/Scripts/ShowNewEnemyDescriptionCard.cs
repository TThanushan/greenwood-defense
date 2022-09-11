using UnityEngine;

public class ShowNewEnemyDescriptionCard : MonoBehaviour
{
    public static ShowNewEnemyDescriptionCard instance;
    GameObject descriptionCard;

    SaveManager saveManager;


    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
        descriptionCard = transform.Find("NewEnemyDescriptionCard").gameObject;

    }
    private void Start()
    {
        saveManager = SaveManager.instance;
        Invoke(nameof(Check), Constants.TIME_BEFORE_FIRST_ENEMY_SPAWN - 1);
    }

    public bool IsDescriptionCardEnabled()
    {
        return descriptionCard.activeSelf;
    }


    private void Update()
    {
        //if (Time.timeSinceLevelLoad > Constants.TIME_BEFORE_FIRST_ENEMY_SPAWN - 1 && Time.timeSinceLevelLoad < 10)
        //    Check();
    }
    public void DisableDescriptionCard()
    {
        descriptionCard.SetActive(false);
    }

    public void CloseCard()
    {
        DisableDescriptionCard();
        EnableObjects();
        DisableAllUnits(false);
    }

    void EnableObjects(bool value = true)
    {
        GameObject.Find("ManaBody").GetComponent<ManaBar>().enabled = value;
        GameObject.Find("SpawnBar").GetComponent<SpawnBar>().enabled = value;
    }

    void DisableAllUnits(bool value = true)
    {
        GameObject[] allies = PoolObject.instance.Allies;
        if (allies is null || allies.Length == 0)
            return;
        foreach (GameObject ally in PoolObject.instance.Allies)
        {
            if (ally.name == "PlayerCaptain")
                continue;
            ally.GetComponent<Unit>().Disabled = value;
        }
    }

    bool DoesCardNeedToBeShowned()
    {
        int currentLevel = StageInfosManager.instance.GetCurrentStageNumber();
        int index = (currentLevel) / 5;
        if (currentLevel == Constants.MAX_STAGE_NUMBER)
            return false;
        return saveManager.maxLevelUnlocked == currentLevel && (saveManager.newEnemyCardDescriptionShownedIndex == index) &&
            (currentLevel == 2 || (currentLevel % 5 == 0 && saveManager.GetLevelScore(currentLevel - 1) == 0));
    }



    void Check()
    {
        if (!DoesCardNeedToBeShowned())
            return;
        DisableAllUnits();

        GameObject.Find("ManaBody").GetComponent<ManaBar>().enabled = false;
        GameObject.Find("SpawnBar").GetComponent<SpawnBar>().enabled = false;
        AudioManager.instance.PlaySfx(Constants.NEW_ENEMY_SFX);
        GetEnemySprite(saveManager.newEnemyCardDescriptionShownedIndex);
        descriptionCard.SetActive(true);
        saveManager.newEnemyCardDescriptionShownedIndex++;
    }
    void SetSprite(GameObject enemy)
    {
        Transform spriteBody = Instantiate(enemy.transform.Find("SpriteBody"), transform.Find("NewEnemyDescriptionCard/Popup"));
        spriteBody.name = spriteBody.name.Replace("(Clone)", "");
        Transform spriteBodyTransformModel = descriptionCard.transform.Find("Popup/SpriteBodyModel");
        spriteBody.transform.localPosition = spriteBodyTransformModel.localPosition;
        spriteBody.transform.localEulerAngles = spriteBodyTransformModel.localEulerAngles;
        spriteBody.transform.localScale = spriteBodyTransformModel.localScale;
        SpriteRenderer[] sprites = spriteBody.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sp in sprites)
        {
            sp.sortingLayerName = "UI";
            sp.sortingOrder = 1002 + sp.sortingOrder;
        }
    }
    void GetEnemySprite(int index)
    {
        int descriptionIndex = index;
        GameObject enemy;
        Stage.EnemyType[] enemyTypes = EnemySpawner.instance.GetStage().enemyTypes;
        enemy = enemyTypes[^1].Enemy;
        if (enemy.name.Contains("Ultimate"))
            enemy = enemyTypes[^2].Enemy;


        SetSprite(enemy);


        if (descriptionIndex > saveManager.unitDescriptions.Count)
            descriptionIndex = saveManager.unitDescriptions.Count - 1;

        string name = saveManager.unitDescriptions[descriptionIndex].name;
        string description = saveManager.unitDescriptions[descriptionIndex].description;

        SetDescriptionInfos(name, description);


    }

    void SetDescriptionInfos(string name, string description)
    {
        descriptionCard.transform.Find("Popup/NameGroup/Name").GetComponent<TMPro.TextMeshProUGUI>().text = name;
        descriptionCard.transform.Find("Popup/DescriptionGroup/Description").GetComponent<TMPro.TextMeshProUGUI>().text = description;
    }


    public class UnitDescription
    {
        public string name;
        public string description;

        public UnitDescription(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
}
