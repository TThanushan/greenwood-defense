using UnityEngine;

public class Trap : MonoBehaviour
{
    public float damage;
    string targetTag = "Ally";
    GameObject target;

    public GameObject triggerEffect;
    public float triggerRange;
    public bool destroyOnTrigger;
    public void SetTargetTag(string _targetTag)
    {
        targetTag = _targetTag;
    }

    // Update is called once per frame
    void Update()
    {
        target = GetClosestEnemy();
        if (!target)
            return;
        if (IsTargetInRange())
        {
            //DamageTarget();
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
            newEffect.transform.position = (transform.position + (target.transform.position * 1.25f)) / 2;
            RotateObjAwayFrom(newEffect, target);
        }
    }

    private void DamageEnemiesAround()
    {
        GameObject[] enemies = GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance <= triggerRange)
            {
                DamageTarget(enemy, damage);
            }
        }

    }

    public void GetDamage(float damage, Transform caller, string hitSoundName = "")
    {

    }

    private void RotateObjAwayFrom(GameObject obj, GameObject _target)
    {
        Vector3 dir = _target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }
    protected void DamageTarget()
    {
        target.GetComponent<HealthBar>().GetDamage(damage, transform, "");
    }

    protected void DamageTarget(GameObject target, float damage)
    {
        target.GetComponent<HealthBar>().GetDamage(damage, transform, "");
    }
    protected bool IsTargetInRange()
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);
        return distance < triggerRange;
    }
    private GameObject GetClosestEnemy()
    {
        GameObject[] enemies;
        if ((enemies = GetEnemies()) == null) return null;

        GameObject closestEnemy = null;
        float lowestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<Unit>().Disabled)
                continue;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    protected GameObject[] GetEnemies()
    {
        if (targetTag == "Enemy")
            return PoolObject.instance.Enemies;
        else
            return PoolObject.instance.Allies;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, triggerRange);
    }
}
