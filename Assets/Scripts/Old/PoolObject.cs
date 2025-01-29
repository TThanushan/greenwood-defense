using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolObject : MonoBehaviour
{
    public static PoolObject instance;

    public Transform bin;
    public List<GameObject> Enemies { get => enemies; set => enemies = value; }
    public List<GameObject> Allies { get => allies; set => allies = value; }

    private List<GameObject> enemies = new List<GameObject>();
    private List<GameObject> allies = new List<GameObject>();
    public List<GameObject> pool;
    //public List<GameObject> poolOnHitEffects;
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
    public SFXManager audioManager;

    public DamageNumber damageNumbersProPrefab;
    public DamageNumber goldOnDeathTextPrefab;
    public GameObject damageText;
    public GameObject frogOnDeathEffect;
    public GameObject moneyRewardEffect;
    [SerializeField] private GameObject onHitEffect;

    // Additions to manage the limit and check proximity
    [SerializeField] private int maxActiveOnHitEffects = 5;
    [SerializeField] private float minDistanceBetweenEffects = 1.0f; // Minimum distance to consider playing a new effect
    private List<Transform> activeOnHitEffects = new List<Transform>(); // Track active effects

    [Header("Trap Management")]
    [SerializeField] private float minDistanceBetweenTraps = 1.0f; // Minimum distance between traps
    private Dictionary<string, List<GameObject>> activeTrapsByType = new Dictionary<string, List<GameObject>>();

    PerformanceDisplay PerformanceDisplay;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        pool = new List<GameObject>();
        enemyCaptain = GameObject.Find("EnemyCaptain").GetComponent<Unit>();
        playerCaptain = GameObject.Find("PlayerCaptain").GetComponent<Unit>();
    }

    private void Start()
    {
        playerStatsScript = PlayerStatsScript.instance;
        manaBar = ManaBar.instance;
        stageManager = StageManager.instance;
        audioManager = SFXManager.instance;
        Allies = new List<GameObject>();
        Enemies = new List<GameObject>();
        PerformanceDisplay = PerformanceDisplay.instance;
        FindAllEnemies();
        FindAllAllies();
    }

    // Remove an ally from the list of allies
    public void RemoveAlly(GameObject ally)
    {
        _ = Allies.Remove(ally);
    }

    // Remove an enemy from the list of enemies
    public void RemoveEnemy(GameObject enemy)
    {
        _ = Enemies.Remove(enemy);
    }

    // Add an ally to the list of Allies
    public void AddAlly(GameObject ally)
    {
        Allies.Add(ally);
    }

    // Add an enemy to the list of Enemies
    public void AddEnemy(GameObject enemy)
    {
        Enemies.Add(enemy);
    }


    public GameObject GetPoolTrap(GameObject trapPrefab, Vector3 spawnPosition)
    {
        string trapType = trapPrefab.tag; // Assuming each trap prefab is tagged with a unique identifier

        if (!activeTrapsByType.ContainsKey(trapType))
            activeTrapsByType[trapType] = new List<GameObject>();

        List<GameObject> activeTraps = activeTrapsByType[trapType];

        // Check if any existing traps of the same type are too close
        foreach (GameObject trap in activeTraps)
        {
            if (trap.activeInHierarchy && Vector3.Distance(trap.transform.position, spawnPosition) < minDistanceBetweenTraps)
            {
                return null; // Trap too close, do not spawn a new one
            }
        }

        GameObject newTrap = GetPoolObject(trapPrefab);
        if (newTrap != null)
        {
            newTrap.transform.position = spawnPosition;
            activeTraps.Add(newTrap);
            _ = StartCoroutine(TrapCleanup(newTrap, trapType)); // Manage trap lifetime
        }
        return newTrap;
    }

    private IEnumerator TrapCleanup(GameObject trap, string trapType)
    {
        yield return new WaitForSeconds(10); // Assuming the trap lasts for 10 seconds
        _ = activeTrapsByType[trapType].Remove(trap);
    }

    public Transform GetOnHitEffect(Vector3 hitPosition)
    {
        // Adjust maxActiveOnHitEffects based on FPS
        if (PerformanceDisplay.fps < 40)
        {
            maxActiveOnHitEffects = 15; // Reduced count if FPS is below 40
        }
        else if (PerformanceDisplay.fps >= 40 && PerformanceDisplay.fps < 60)
        {
            maxActiveOnHitEffects = 30; // Set a moderate count if FPS is between 40 and 60
        }
        else
        {
            maxActiveOnHitEffects = 50; // Set higher count if FPS is above 60
        }

        // Early out if there's an effect far enough from others
        bool isFarEnough = true;
        foreach (Transform effect in activeOnHitEffects)
        {
            if (Vector3.Distance(effect.position, hitPosition) < minDistanceBetweenEffects)
            {
                isFarEnough = false;
                break;
            }
        }

        // If too close or max active effects reached, do not play the effect
        if (!isFarEnough || activeOnHitEffects.Count >= maxActiveOnHitEffects)
        {
            return null;
        }

        // If it's far enough or count is within limit, play the effect
        return ActivateEffect(hitPosition);
    }

    private Transform ActivateEffect(Vector3 position)
    {
        Transform newEffectTransform = PoolObject.instance.GetPoolObject(onHitEffect, pool)?.transform;
        if (newEffectTransform != null)
        {
            activeOnHitEffects.Add(newEffectTransform);
            newEffectTransform.position = position;
            _ = StartCoroutine(EffectCleanup(newEffectTransform)); // Cleanup after the effect is done
        }
        return newEffectTransform;
    }

    private IEnumerator EffectCleanup(Transform effect)
    {
        yield return new WaitForSeconds(1); // Assuming effect duration is 1 second
        _ = activeOnHitEffects.Remove(effect);
    }

    public GameObject DisplayDamageText(float damage)
    {
        DamageNumber dmg = damageNumbersProPrefab.Spawn(Vector3.zero, damage);
        return dmg.gameObject;
    }

    public void DisplayGoldText(Vector3 position, float goldAmount)
    {
        _ = goldOnDeathTextPrefab.Spawn(position, goldAmount);
    }

    public GameObject DisplayText(string text, float scale)
    {
        if (!damageText)
            return null;
        GameObject obj = GetPoolObject(damageText);
        obj.transform.localScale = new Vector2(damageText.transform.localScale.x * scale, damageText.transform.localScale.y * scale);
        obj.transform.Find("Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = text;
        return obj;
    }

    public string GetStageNumber()
    {
        return SceneManager.GetActiveScene().name.Split(' ')[1];
    }

    public GameObject GetPoolObject(GameObject prefab, List<GameObject> _pool = null)
    {
        if (!prefab) return null;
        if (_pool == null) _pool = pool;

        GameObject newUnit;
        return (newUnit = FindExistingClone(prefab, _pool)) ? newUnit : CreateNewClone(prefab);
    }

    private GameObject FindExistingClone(GameObject prefab, List<GameObject> _pool = null)
    {
        if (_pool == null) _pool = pool;
        foreach (GameObject currentUnit in _pool)
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
        Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    private void FindAllAllies()
    {
        Allies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Ally"));
    }

    public GameObject[] GetAlliesAsArray()
    {
        return Allies.ToArray();
    }

    public GameObject[] GetEnemiesAsArray()
    {
        return Enemies.ToArray();
    }
}
