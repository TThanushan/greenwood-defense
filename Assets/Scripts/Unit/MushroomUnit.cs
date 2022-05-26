using System.Collections;
using UnityEngine;

public class MushroomUnit : UnitAoeAttack
{
    [Header("Poison")]
    public int dotCount = 4;
    public float timeBetweenDamage = 1f;
    public GameObject poisonParticlesEffect;
    public SpriteRenderer test;
    private void Start()
    {
        transform.Find("SpriteBody/Sprite").GetComponent<SpriteRenderer>().color = new Color(255, 109, 241);
    }
    protected override void DamageEnemiesAroundTarget()
    {

        CreatePoisonEffect();
        GameObject[] enemies = GetEnemies();
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(Target.transform.position, enemy.transform.position);
            if (distance <= areaOfEffectRange)
            {
                StartCoroutine(PoisonDamageTarget(enemy));
                //enemy.GetComponent<Unit>().GetDamage(areaOfEffectDamage);
            }
        }
    }
    void CreatePoisonEffect()
    {
        if (poisonParticlesEffect)
        {
            GameObject newEffect = PoolObject.instance.GetPoolObject(poisonParticlesEffect);
            newEffect.transform.position = transform.position;

        }
    }


    IEnumerator PoisonDamageTarget(GameObject target)
    {
        yield return new WaitForSeconds(timeBetweenDamage);
        //target.transform.Find("SpriteBody/Sprite/UnitSprite").GetComponent<SpriteRenderer>().color = new Color(255, 109, 241);
        test = target.transform.Find("SpriteBody/Sprite/UnitSprite").GetComponent<SpriteRenderer>();
        test.color = new Color(255, 109, 241);
        for (int i = 0; i < dotCount; i++)
        {
            target.GetComponent<Unit>().GetDamage(areaOfEffectDamage);

            yield return new WaitForSeconds(timeBetweenDamage);

        }
        //target.transform.Find("SpriteBody/Sprite/UnitSprite").GetComponent<SpriteRenderer>().color = Color.white;
    }
}
