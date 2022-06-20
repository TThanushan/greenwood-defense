using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolObject : MonoBehaviour
{
    public static PoolObject instance;

    public Transform bin;
    public GameObject[] Enemies { get => enemies; set => enemies = value; }
    public GameObject[] Allies { get => allies; set => allies = value; }

    private GameObject[] enemies;
    private GameObject[] allies;
    public List<GameObject> pool;
    public PlayerStatsScript playerStatsScript;
    public StageManager stageManager;
    public ManaBar manaBar;
    public Unit enemyCaptain;
    public Unit playerCaptain;

    public GameObject moneyRewardEffect;
    public AudioManager audioManager;
    void Awake()
    {
        if (instance == null)
            instance = this;
        pool = new List<GameObject>();
        enemyCaptain = GameObject.Find("EnemyCaptain").GetComponent<Unit>();
        playerCaptain = GameObject.Find("PlayerCaptain").GetComponent<Unit>();
    }

    private void Start()
    {
        playerStatsScript = PlayerStatsScript.instance;
        manaBar = ManaBar.instance;
        stageManager = StageManager.instance;
        audioManager = AudioManager.instance;
    }
    private void Update()
    {
        FindAllEnemies();
        FindAllAllies();
    }

    public string GetStageNumber()
    {
        return SceneManager.GetActiveScene().name.Split(' ')[1];
    }

    public GameObject GetPoolObject(GameObject prefab)
    {
        if (!prefab) return null;

        GameObject newUnit;
        if (newUnit = FindExistingClone(prefab))
            return newUnit;
        return CreateNewClone(prefab);
    }

    private GameObject FindExistingClone(GameObject prefab)
    {
        foreach (GameObject currentUnit in pool)
        {
            if (currentUnit && currentUnit.activeInHierarchy == false && currentUnit.name == prefab.name)
            {
                currentUnit.SetActive(true);
                return currentUnit;
            }
        }
        return null;
    }

    private GameObject CreateNewClone(GameObject prefab)
    {
        GameObject clone = (GameObject)Instantiate(prefab, bin);
        clone.name = prefab.name;
        clone.SetActive(true);
        pool.Add(clone);
        return clone;
    }

    private void FindAllEnemies()
    {
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private void FindAllAllies()
    {
        Allies = GameObject.FindGameObjectsWithTag("Ally");
    }
}
