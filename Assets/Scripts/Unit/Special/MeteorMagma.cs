using UnityEngine;

public class MeteorMagma : Rocket
{
    [Header("Explosion")]
    public GameObject magma;
    public GameObject triggerEffect4;
    public float magmaDamage;

    protected override void DamageEnemiesAround()
    {
        base.DamageEnemiesAround();
        MagmaOnEnemies();
    }

    private void MagmaOnEnemies()
    {
        GameObject[] enemies = GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= explosionRange)
            {
                //enemy.GetComponent<HealthBar>().GetDamage(explosionDamage, transform, "Classic");
                _ = InstantiateTrap(enemy);
            }
        }

    }
    protected GameObject InstantiateTrap(GameObject target)
    {
        if (!magma || !target)
            return null;
        GameObject newTrap = PoolObject.instance.GetPoolTrap(magma, target.transform.position);

        if (newTrap is null)
            return null;

        newTrap.GetComponent<Trap>().SetTargetTag(targetTag);
        newTrap.GetComponent<Trap>().damage = magmaDamage;

        return newTrap;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
