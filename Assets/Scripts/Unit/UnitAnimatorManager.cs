using UnityEngine;

public class UnitAnimatorManager : MonoBehaviour
{

    Animator animator;
    Unit unit;
    private void Start()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();
        GetComponent<Unit>().onAttack += PlayAttackAnimation;
        GetComponent<Unit>().OnDeath += PlayDeathAnimation;

    }

    private void Update()
    {
        if (unit.Disabled)
            return;
        string clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        //if (!GetComponent<Unit>().Target && clipName != "Run" && clipName != "Attack")
        if (!unit.Target || !unit.EnoughRangeToAttackTarget() && clipName != "Attack")
            animator.Play("Run");
    }

    private void PlayAttackAnimation()
    {
        animator.Play("Attack");
    }

    private void PlayDeathAnimation()
    {
        animator.Play("Death");
    }
}
