using UnityEngine;

public class Rocket : BulletScript
{
    public float explosionRange;
    public float maxTargetDistance;

    protected override void AttackTarget()
    {
        if ((target == null && gameObject.activeSelf) || (maxTargetDistance > 0 && Vector2.Distance(transform.position, target.transform.position) > maxTargetDistance))
        {
            DisableDestroyEffect();
            gameObject.SetActive(false);
            return;
        }
        if (IsTargetInRange() && target.GetComponent<Unit>().ProjectileAffectMe())
        {
            DamageEnemiesAround();
            DestroyEffect();
            target = null;
            gameObject.SetActive(false);
        }
    }

    void DisableDestroyEffect()
    {
        if (effect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(effect);
            newEffect.transform.position = transform.position;
        }
    }


    protected virtual void DamageEnemiesAround()
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
