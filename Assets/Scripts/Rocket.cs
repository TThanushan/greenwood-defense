using UnityEngine;

public class Rocket : BulletScript
{
    public float explosionRange;

    protected override void AttackTarget()
    {
        if (target == null && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }
        if (IsTargetInRange())
        {
            DamageEnemiesAround();
            DestroyEffect();
            target = null;
            gameObject.SetActive(false);
        }
    }

    private void DamageEnemiesAround()
    {
        GameObject[] enemies = GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= explosionRange)
            {
                DamageTarget(enemy);
            }
        }

    }



    //private void ExplosionSFX()
    //{
    //    AudioManager.instance.PlaySfx("Boom");
    //}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
