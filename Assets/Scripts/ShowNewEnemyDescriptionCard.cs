using System.Collections.Generic;
using UnityEngine;

public class ShowNewEnemyDescriptionCard : MonoBehaviour
{
    public static ShowNewEnemyDescriptionCard instance;
    GameObject descriptionCard;

    SaveManager saveManager;

    List<UnitDescription> unitDescriptions;

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
        //SceneManager.sceneLoaded += CheckOnSceneLoad;
    }

    public bool IsDescriptionCardEnabled()
    {
        return descriptionCard.activeSelf;
    }
    //void CheckOnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    //{
    //    print("check1");
    //    if (Time.timeSinceLevelLoad > 3)
    //        Check();
    //}


    private void Update()
    {
        if (Time.timeSinceLevelLoad > Constants.TIME_BEFORE_FIRST_ENEMY_SPAWN - 1 && Time.timeSinceLevelLoad < 10)
            Check();
    }
    public void DisableDescriptionCard()
    {
        descriptionCard.SetActive(false);
    }

    public void CloseCard()
    {
        DisableDescriptionCard();

    }
    void Check()
    {
        int currentLevel = StageInfosManager.instance.GetCurrentStageNumber();
        if (saveManager.maxLevelUnlocked != currentLevel)
            return;
        print("ShowCard");

        print(currentLevel);
        int index = (currentLevel) / 5;
        if (saveManager.newEnemyCardDescriptionShownedIndex == index)
            if (currentLevel == 2 || (currentLevel % 5 == 0 && saveManager.GetLevelScore(currentLevel - 1) == 0))
            {
                GetEnemySprite(saveManager.newEnemyCardDescriptionShownedIndex);
                descriptionCard.SetActive(true);
                saveManager.newEnemyCardDescriptionShownedIndex++;
            }
    }

    void GetEnemySprite(int index)
    {
        GameObject enemy;
        if (EnemySpawner.instance.GetStage().enemyTypes[0].Enemy.name == "MeleeFrogUnit")
            enemy = EnemySpawner.instance.GetStage().enemyTypes[index + 1].Enemy;
        else
            enemy = EnemySpawner.instance.GetStage().enemyTypes[index].Enemy;

        //if (enemy.name == "MeleeFrogUnit")
        //{
        //    enemy = EnemySpawner.instance.GetStage().enemyTypes[index + 1].Enemy;
        //}

        Transform spriteBody = Instantiate(enemy.transform.Find("SpriteBody"), transform.Find("NewEnemyDescriptionCard"));
        spriteBody.name = spriteBody.name.Replace("(Clone)", "");

        Transform spriteBodyTransformModel = descriptionCard.transform.Find("SpriteBodyModel");
        spriteBody.transform.localPosition = spriteBodyTransformModel.localPosition;
        spriteBody.transform.localEulerAngles = spriteBodyTransformModel.localEulerAngles;
        spriteBody.transform.localScale = spriteBodyTransformModel.localScale;
        SpriteRenderer[] sprites = spriteBody.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sp in sprites)
        {
            sp.sortingLayerName = "UI";
            sp.sortingOrder = 1002;
        }
        if (index > saveManager.unitDescriptions.Count)
            index = saveManager.unitDescriptions.Count;
        descriptionCard.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = saveManager.unitDescriptions[index].name;
        descriptionCard.transform.Find("Description").GetComponent<TMPro.TextMeshProUGUI>().text = saveManager.unitDescriptions[index].description;
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
