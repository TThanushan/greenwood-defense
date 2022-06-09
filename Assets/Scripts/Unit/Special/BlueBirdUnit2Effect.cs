using UnityEngine;

public class BlueBirdUnit2Effect : MonoBehaviour
{
    [Header("Egg")]
    public GameObject eggPrefab;
    public Transform eggSpawnPos;
    public float eggExplosionDamage;
    public float eggReloadTime;
    bool once;
    PoolObject poolObject;
    float eggCooldown;


    private void Start()
    {
        poolObject = PoolObject.instance;
    }
    private void OnEnable()
    {
        if (!once)
        {
            DropEgg();
            once = true;
        }
    }

    void DropEgg()
    {
        InstantiateEgg();
    }

    protected GameObject InstantiateEgg()
    {
        if (!eggPrefab)
            return null;
        GameObject newEgg = poolObject.GetPoolObject(eggPrefab);
        newEgg.transform.position = eggSpawnPos.transform.position;

        if (newEgg.GetComponent<EggBomb>())
        {
            EggBomb newEggScript = newEgg.GetComponent<EggBomb>();
            newEggScript.explosionDamage = eggExplosionDamage;
        }
        eggCooldown = eggReloadTime + Time.time;

        return newEgg;
    }
}
