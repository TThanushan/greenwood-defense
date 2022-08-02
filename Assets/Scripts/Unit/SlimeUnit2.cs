using UnityEngine;

public class SlimeUnit2 : SlimeUnit1
{
    [Header("SpawnOnDeath")]
    public int spawnNumber;
    public GameObject[] mediumSlimes;

    public override void Disable()
    {
        base.Disable();
        SpawnSlimes();
    }


    void SpawnSlimes()
    {
        CreateEffect(spawnEffect);
        for (int i = 0; i < spawnNumber; i++)
        {
            for (int y = 0; y < mediumSlimes.Length; y++)
            {
                SpawnSlime(mediumSlimes[y]);

            }
        }

        //ShakeCamera.instance.Shake(0.025f, 0.25f);
    }

}
