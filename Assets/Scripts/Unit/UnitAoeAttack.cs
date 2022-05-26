using UnityEngine;

public class UnitAoeAttack : Unit
{

    public float areaOfEffectRange;
    public float areaOfEffectDamage;



    public override void Attack()
    {
        base.Attack();
        DamageEnemiesAroundTarget();

    }
    protected virtual void DamageEnemiesAroundTarget()
    {
        GameObject[] enemies = GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(Target.transform.position, enemy.transform.position);
            if (distance <= areaOfEffectRange)
            {
                enemy.GetComponent<Unit>().GetDamage(areaOfEffectDamage);
            }
        }
    }



}
