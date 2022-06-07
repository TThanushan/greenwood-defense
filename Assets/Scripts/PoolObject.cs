using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public static PoolObject instance;

    public Transform bin;
    public GameObject[] Enemies { get => enemies; set => enemies = value; }
    public GameObject[] Allies { get => allies; set => allies = value; }

    private GameObject[] enemies;
    private GameObject[] allies;
    public List<GameObject> pool;

    void Awake()
    {
        if (instance == null)
            instance = this;
        pool = new List<GameObject>();
    }

    private void Update()
    {
        FindAllEnemies();
        FindAllAllies();
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
