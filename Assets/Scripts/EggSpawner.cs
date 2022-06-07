using UnityEngine;

public class EggSpawner : MonoBehaviour
{
    public float timeBeforeExplosion;
    public GameObject explosionEffect;
    public GameObject birdPrefab;

    public string targetTag = "Enemy";
    float currentTimeBeforeExplosion;
    private void Update()
    {
        if (currentTimeBeforeExplosion <= Time.time)
        {
            SpawnBird();

        }
    }
    private void OnEnable()
    {
        currentTimeBeforeExplosion = Time.time + timeBeforeExplosion;

    }

    void SpawnBird()
    {
        if (!birdPrefab)
            return;
        GameObject newBirdPrefab = PoolObject.instance.GetPoolObject(birdPrefab);
        newBirdPrefab.transform.position = transform.position;
        newBirdPrefab.GetComponent<Unit>().SetTargetTag(targetTag);
        CreateEffect();
        gameObject.SetActive(false);
    }


    void CreateEffect()
    {
        if (!explosionEffect)
            return;
        GameObject newEgg = PoolObject.instance.GetPoolObject(explosionEffect);
        newEgg.transform.position = transform.position;
    }
    public void SetTargetTag(string tag)
    {
        targetTag = tag;
    }


}
