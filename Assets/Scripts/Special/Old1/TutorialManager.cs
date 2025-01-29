using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{

    public int tutorialIndex = 0;
    GameObject mainCanvas;
    Camera mainCamera;
    int lastIndexShown = -1;
    EnemySpawner enemySpawner;
    PoolObject poolObject;
    bool enemySpawned;
    GameObject levelCompleteMenuButton;
    GameObject duckUnitRef;
    List<GameObject> objectsToDisable;
    private void Start()
    {
        if (SaveManager.instance.IsTutorialDone())
        {
            Destroy(gameObject);
            return;
        }

        transform.Find("SkipTutorialButton").gameObject.SetActive(true);
        PoolObject.instance.enemyCaptain.currentHealth = 25;
        PoolObject.instance.enemyCaptain.maxHealth = 25;

        //mainCanvas = GameObject.Find("InGameMainCanvas");
        //transform.parent = mainCanvas.transform;
        //transform.Find("SkipTutorialButton").position = Vector3.zero;

        //ResetTutoPartCanvasScale();
        //levelCompleteMenuButton = mainCanvas.transform.Find("MiddleGroup/LevelCompletePanel/NextLevelButton").gameObject;
        //levelCompleteMenuButton.transform.parent.Find("NextLevelButton").localScale = Vector3.zero;
        objectsToDisable = new List<GameObject>();

        objectsToDisable.Add(GameObject.Find("CameraMoveRight"));
        objectsToDisable.Add(GameObject.Find("CameraMoveLeft"));
        objectsToDisable.Add(GameObject.Find("SpawnBar"));
        objectsToDisable.Add(GameObject.Find("MiniMap"));
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj.name == "MiniMap")
                obj.GetComponent<Camera>().enabled = false;
            else
                obj.SetActive(false);
        }

        mainCamera = Camera.main;
        enemySpawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<EnemySpawner>();
        enemySpawner.gameObject.SetActive(false);
        poolObject = PoolObject.instance;
        ShowTutoPart();

    }

    void ResetTutoPartCanvasScale()
    {
        transform.localScale = Vector3.one;
        foreach (Transform tutoPart in transform)
        {
            if (tutoPart.name == "SkipTutorialButton")
                continue;
            tutoPart.Find("Canvas").localScale = Vector3.one;
            tutoPart.Find("Canvas").position = Vector3.zero;
            if (tutoPart.name == name)
                tutoPart.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        UpdateTutorial();
    }
    //void HideUI(bool value = false)
    //{
    //    mainCanvas.transform.Find("TopGroup").gameObject.SetActive(value);
    //    mainCanvas.transform.Find("LeftGroup").gameObject.SetActive(value);
    //    mainCanvas.transform.Find("RightGroup").gameObject.SetActive(value);
    //}

    public void SkipTutorial()
    {
        HideAllTutoPart();
        EndTutorial();
    }

    public void EndTutorial()
    {
        SaveManager.instance.SetTutorialDone();
        enemySpawner.gameObject.SetActive(true);
        foreach (GameObject obj in objectsToDisable)
        {
            if (obj.name == "MiniMap")
                obj.GetComponent<Camera>().enabled = true;
            else
                obj.SetActive(true);
        }

        Destroy(gameObject);
    }



    void ShowTutoPart()
    {
        if (lastIndexShown == tutorialIndex)
            return;
        string name = "P" + tutorialIndex;
        HideAllTutoPart(name);
        lastIndexShown = tutorialIndex;
    }

    public void HideAllTutoPart(string name = "")
    {
        foreach (Transform tutoPart in transform)
        {
            if (tutoPart.name == "SkipTutorialButton")
                continue;
            tutoPart.gameObject.SetActive(false);
            if (tutoPart.name == name)
                tutoPart.gameObject.SetActive(true);
        }
    }

    void UpdateTutorial()
    {
        //// Hide UI element.
        //HideUI();
        if (Time.time < 2)
            return;
        IsConditionFilled();


        //if (tutorialIndex == 8 && Input.GetKeyUp(KeyCode.Mouse0) || Input.touchCount == 1)
        //{ tutorialIndex++; return; }

        // msg "You passively regen mana, killing enemy units also regen mana.".
        // Wait until collision between units.
        // Wait a bit until duck has fight a bit.
        // Highligth the chicken unit buttom, with msg "You soldier need help, summon a chicken.".


    }

    bool IsConditionFilled()
    {
        // Highlight the ally captain, with msg "Protect the king"
        // Tell player to click on the right of the screen (until its all the way on the right)
        // Highlight the enemy captain, with msg "Defeat the frog captain to win"
        // Make two melee frog spawn, with msg "Carefull, the enemy is sending soldiers."
        // Tell player to click on the left of the screen (until its all the way on the left).
        // Set player current mana to 30.
        // Highligth the duck unit button, with msg "Summon a unit".
        // Highligth mana bar with msg "Summoning unit cost mana.".
        // msg "Summon more units to defeat the enemy"
        // Wait until menu button of levelCompletePanel appeared.
        //  > highligth menu button
        //  > hide next level button

        if (
           (tutorialIndex == 0 && Input.GetKeyUp(KeyCode.Mouse0))
        || (tutorialIndex == 1 && Input.GetKeyUp(KeyCode.Mouse0))
        || (tutorialIndex == 2 && Input.GetKeyUp(KeyCode.Mouse0))
        || (tutorialIndex == 3 && Input.GetKeyUp(KeyCode.Mouse0))
        || (tutorialIndex == 4 && Input.GetKeyUp(KeyCode.Mouse0))
        || (tutorialIndex == 5 && Input.GetKeyUp(KeyCode.Mouse0))
        || (tutorialIndex == 6 && Input.GetKeyUp(KeyCode.Mouse0))
        || (tutorialIndex == 7 && Input.GetKeyUp(KeyCode.Mouse0))
        )
        {
            if (tutorialIndex == 6)
                enemySpawner.gameObject.SetActive(true);
            tutorialIndex++;
            HideAllTutoPart();
            ShowTutoPart();
            return true;
        }
        else if (tutorialIndex == 7)
            EndTutorial();

        //else if (tutorialIndex == 8 && levelCompleteMenuButton.activeSelf)
        //{
        //    //SwitchParent();
        //    HideAllTutoPart();
        //    RemoveParentAndDontDestroyOnLoad();
        //    ShowTutoPart();
        //    tutorialIndex++;
        //}
        //else if (tutorialIndex == 9 && SceneManager.GetActiveScene().name == "LevelSelect")
        //{
        //    //SwitchParent();
        //    transform.parent = GameObject.Find("MainCanvas").transform;
        //    //Disable other button.
        //    transform.parent.Find("MiddleGroup/Buttons/Battle").gameObject.SetActive(false);
        //    transform.parent.Find("MiddleGroup/Buttons/Menu").gameObject.SetActive(false);
        //    transform.parent.Find("MiddleGroup/Buttons/Reset").gameObject.SetActive(false);
        //    transform.position = Vector3.zero;
        //    transform.localScale = Vector3.one;

        //    RemoveParentAndDontDestroyOnLoad();

        //    ShowTutoPart();
        //    tutorialIndex++;

        //}
        //else if (tutorialIndex == 10 && SceneManager.GetActiveScene().name == "Upgrades")
        //{
        //    //Remove after.
        //    EndTutorial();
        //}

        // TODO
        return false;
    }

    void RemoveParentAndDontDestroyOnLoad()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    void SwitchParent()
    {
        if (SceneManager.GetActiveScene().name == "Stage")
        {

        }
        else if (SceneManager.GetActiveScene().name == "LevelSelect")
            transform.parent = GameObject.Find("MainCanvas").transform;
    }
}
