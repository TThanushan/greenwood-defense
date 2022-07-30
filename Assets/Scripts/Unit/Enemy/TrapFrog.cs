using UnityEngine;

public class TrapFrog : Unit
{
    public GameObject trap;
    public Transform trapSpawnPosition;
    public float timeBetweenTrap;
    public float enemyDistanceBeforePosingTrap;

    float nextTrapSpawn = 0f;
    protected override void Update()
    {
        base.Update();
        if (nextTrapSpawn <= Time.time && !EnoughRangeToAttackTarget())
            InstantiateTrap(Target);
    }

    protected GameObject InstantiateTrap(GameObject target)
    {
        if (!trap || !target)
            return null;
        GameObject newTrap = poolObject.GetPoolObject(trap);
        newTrap.transform.position = trapSpawnPosition.transform.position;
        //newTrap.GetComponent<Trap>().SetTargetTag(targetTag);
        newTrap.GetComponent<Unit>().SetTargetTag(targetTag);

        nextTrapSpawn = Time.time + timeBetweenTrap;
        return newTrap;
    }
}
