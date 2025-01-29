using UnityEngine;

public class Trap : MonoBehaviour
{
    public float damage;
    private string targetTag = "Ally";
    private GameObject target;

    public GameObject triggerEffect;
    public float triggerRange;
    public bool destroyOnTrigger;

    private Transform _transform;
    private GameObject[] enemies;
    private float triggerRangeSqr;

    private void Awake()
    {
        _transform = transform;
        triggerRangeSqr = triggerRange * triggerRange;
    }

    public void SetTargetTag(string _targetTag)
    {
        targetTag = _targetTag;
        enemies = GetEnemies();
    }

    void Update()
    {
        // Cache enemies array periodically (every second)
        if (Time.frameCount % 60 == 0)
        {
            enemies = GetEnemies();
        }

        target = GetClosestEnemy();
        if (!target)
            return;
        if (IsTargetInRange())
        {
            DestroyEffect();
            DamageEnemiesAround();
            if (destroyOnTrigger)
            {
                target = null;
                gameObject.SetActive(false);
            }
        }
    }

    protected void DestroyEffect()
    {
        if (triggerEffect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(triggerEffect);
            newEffect.transform.position = (_transform.position + (target.transform.position * 1.25f)) / 2;
            RotateObjAwayFrom(newEffect, target);
        }
    }

    private void DamageEnemiesAround()
    {
        foreach (GameObject enemy in enemies)
        {
            float distanceSqr = (_transform.position - enemy.transform.position).sqrMagnitude;
            if (distanceSqr <= triggerRangeSqr)
            {
                DamageTarget(enemy, damage);
            }
        }
    }

    public void GetDamage(float damage, Transform caller, string hitSoundName = "")
    {
        // Implementation here
    }

    private void RotateObjAwayFrom(GameObject obj, GameObject _target)
    {
        Vector3 dir = _target.transform.position - _transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }

    protected void DamageTarget()
    {
        target.GetComponent<HealthBar>().GetDamage(damage, _transform, "");
    }

    protected void DamageTarget(GameObject target, float damage)
    {
        target.GetComponent<HealthBar>().GetDamage(damage, _transform, "");
    }

    protected bool IsTargetInRange()
    {
        float distanceSqr = (_transform.position - target.transform.position).sqrMagnitude;
        return distanceSqr < triggerRangeSqr;
    }

    private GameObject GetClosestEnemy()
    {
        if (enemies == null) return null;

        GameObject closestEnemy = null;
        float lowestDistanceSqr = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<Unit>().Disabled)
                continue;
            float distanceSqr = (_transform.position - enemy.transform.position).sqrMagnitude;
            if (distanceSqr < lowestDistanceSqr)
            {
                lowestDistanceSqr = distanceSqr;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    protected GameObject[] GetEnemies()
    {
        return targetTag == "Enemy" ? PoolObject.instance.GetEnemiesAsArray() : PoolObject.instance.GetAlliesAsArray();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRange);
    }
}
