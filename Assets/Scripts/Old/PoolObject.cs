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
    [HideInInspector]
    public StageManager stageManager;
    [HideInInspector]
    public ManaBar manaBar;
    [HideInInspector]
    public Unit enemyCaptain;
    [HideInInspector]
    public Unit playerCaptain;
    [HideInInspector]
    public AudioManager audioManager;

    public GameObject damageText;
    public GameObject frogOnDeathEffect;
    public GameObject moneyRewardEffect;
    public GameObject onHitEffect;

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

    public Transform GetOnHitEffect()
    {
        return GetPoolObject(onHitEffect).transform;
    }

    public GameObject DisplayDamageText(float damage)
    {
        if (!damageText)
            return null;
        damage = damage * 100.0f / 100.0f;
        damage = (int)damage;
        GameObject obj = GetPoolObject(damageText);
        float scale = damage / 30f;
        scale = Mathf.Min(1.5f, scale);
        scale = Mathf.Max(0.7f, scale);
        obj.transform.localScale = new Vector2(damageText.transform.localScale.x * scale, damageText.transform.localScale.y * scale);
        obj.transform.Find("Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = damage.ToString();
        return obj;
    }
    public GameObject DisplayText(string text, float scale)
    {
        if (!damageText)
            return null;
        //damage = damage * 100.0f / 100.0f;
        //damage = (int)damage;
        GameObject obj = GetPoolObject(damageText);
        //float scale = damage / 30f;
        //scale = Mathf.Min(1.5f, scale);
        //scale = Mathf.Max(0.7f, scale);
        obj.transform.localScale = new Vector2(damageText.transform.localScale.x * scale, damageText.transform.localScale.y * scale);
        obj.transform.Find("Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = text;
        return obj;
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
        GameObject clone = Instantiate(prefab, bin);
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
