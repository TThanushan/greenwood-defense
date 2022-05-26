using System.Collections.Generic;

public class BulletGoThrough : BulletScript
{
    public int piercingCount = 3;


    int currentPiercing;
    List<int> enemyAlreadyDamaged;

    private void Start()
    {
        enemyAlreadyDamaged = new List<int>();
    }
    protected override void AttackTarget()
    {
        if (target == null && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }
        print("hi");
        if (IsTargetInRange() && !enemyAlreadyDamaged.Contains(target.GetInstanceID()))
        //if (IsTargetInRange())
        {
            enemyAlreadyDamaged.Add(target.GetInstanceID());

            DamageTarget();
            DestroyEffect();
            //target = null;
            currentPiercing++;
            if (currentPiercing >= piercingCount)
            {
                print("hi2");
                gameObject.SetActive(false);
                Reset();
            }
        }
    }

    private void OnDisable()
    {
        Reset();
    }
    private void Reset()
    {
        currentPiercing = 0;
        enemyAlreadyDamaged = new List<int>();

    }
}
