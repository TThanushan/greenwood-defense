using UnityEngine;

public class UnitAoeAttack : Unit
{

    public float effectRange;
    public float effectDamage;



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
            if (distance <= effectRange)
            {
                enemy.GetComponent<Unit>().GetDamage(effectDamage, transform);
            }
        }
    }



}
