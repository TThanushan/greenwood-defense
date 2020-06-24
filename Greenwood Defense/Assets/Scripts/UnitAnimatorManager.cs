using UnityEngine;
using System.Collections;

public class UnitAnimatorManager : MonoBehaviour
{

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GetComponent<Unit>().onAttack += PlayAttackAnimation;
        GetComponent<Unit>().OnDeath += PlayDeathAnimation;

    }

    private void Update()
    {
        string clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (!GetComponent<Unit>().Target && clipName != "Run" && clipName != "Attack")
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
