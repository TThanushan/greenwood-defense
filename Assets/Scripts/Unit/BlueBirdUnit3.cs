using UnityEngine;

public class BlueBirdUnit3 : BlueBirdUnit2
{

    public float timeBetweenTravel;
    public int nbSpawn = 3;
    public float distBetweenDrop;

    float travelDistCoef = 1;
    protected override void DropEgg()
    {
        float t = 0f;
        float timeBetween = 0.25f;
        for (int i = 0; i < nbSpawn; i++)
        {
            Invoke("SummonEffectBird", t);
            t += timeBetween;
        }
        Invoke("ResetTravelDistCoef", birdSpawnReloadTime);

    }

    void ResetTravelDistCoef()
    {
        travelDistCoef = 1f;
    }

    protected override void SummonEffectBird()
    {
        if (!birdEffectPrefab)
            return;
        GameObject newBird = poolObject.GetPoolObject(birdEffectPrefab);
        newBird.transform.position = spawnPos.transform.position;
        BlueBirdUnit2Effect blueBirdUnit2Effect = newBird.GetComponent<BlueBirdUnit2Effect>();
        blueBirdUnit2Effect.travelDistance.x *= travelDistCoef;
        blueBirdUnit2Effect.SetStats(eggExplosionDamage, targetTag);
        birdSpawnCooldown = Time.time + birdSpawnReloadTime;
        travelDistCoef += distBetweenDrop;
    }



}
