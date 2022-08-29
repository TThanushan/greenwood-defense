using UnityEngine;

public class UnitAoeAttack : Unit
{

    public float effectRange;
    public float effectDamage;
    public string damageSFXName = "Classic";


    public override void Attack()
    {
        base.Attack();
        DamageEnemiesAroundTarget();

    }
    protected virtual void DamageEnemiesAroundTarget()
    {
        GameObject[] enemies = GetEnemies();
        //poolObject.audioManager.Play(damageSFXName);
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(Target.transform.position, enemy.transform.position);
            if (distance <= effectRange)
            {
                enemy.GetComponent<Unit>().GetDamage(effectDamage, transform, damageSFXName);
            }
        }
    }



}
