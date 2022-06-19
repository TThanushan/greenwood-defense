using UnityEngine;

public class BlueBirdUnit2Effect : MonoBehaviour
{
    [Header("Egg")]
    public GameObject eggPrefab;
    public Transform eggSpawnPos;
    public float eggExplosionDamage;
    public Vector2 travelDistance;
    public float travelSpeed;

    Vector2 initialTravelDistance;
    Vector2 travelDestination;
    PoolObject poolObject;
    string targetTag = "Enemy";

    public float timeBetweenDrop = 0.25f;
    float currentTimeBetweenDrop;
    private void Start()
    {
        poolObject = PoolObject.instance;
    }

    private void OnDisable()
    {
        travelDistance = initialTravelDistance;
    }
    private void OnEnable()
    {
        currentTimeBetweenDrop = timeBetweenDrop;
        initialTravelDistance = travelDistance;
        DetermineTravelDestination();
    }

    void DetermineTravelDestination()
    {
        travelDestination = (Vector2)transform.position + travelDistance;
    }

    private void Update()
    {
        DoTravel();
    }

    void DoTravel()
    {
        if (!DestinationReached())
        {
            transform.position = Vector2.MoveTowards(transform.position, travelDestination, travelSpeed);
            DropLoop();
        }
        else if (DestinationReached())
        {
            Disable();
            DropEgg();
        }
    }

    bool DestinationReached()
    {
        return Vector2.Distance(transform.position, travelDestination) < 0.1f;
    }

    void DropLoop()
    {
        if (currentTimeBetweenDrop <= Time.time)
        {
            currentTimeBetweenDrop = Time.time + timeBetweenDrop;
            DropEgg();
        }
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }

    void DropEgg()
    {
        if (!eggPrefab)
            return;
        GameObject newEgg = poolObject.GetPoolObject(eggPrefab);
        newEgg.transform.position = eggSpawnPos.transform.position;
        if (newEgg.GetComponent<EggBomb>())
        {
            EggBomb newEggScript = newEgg.GetComponent<EggBomb>();
            newEggScript.SetTargetTag(targetTag);
            newEggScript.explosionDamage = eggExplosionDamage;
        }
        else if (newEgg.GetComponent<EggSpawner>())
            newEgg.GetComponent<EggSpawner>().targetTag = targetTag;
    }

    public void SetStats(float _eggExplosionDamage, string _targetTag = "Enemy")
    {
        eggExplosionDamage = _eggExplosionDamage;
        targetTag = _targetTag;
        DetermineTravelDestination();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(travelDestination, 0.1f);
    }
}