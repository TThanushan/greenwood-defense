using UnityEngine;

public class UnitAnimatorManager : MonoBehaviour
{

    Animator animator;
    Unit unit;
    Transform unitSpriteTransform;
    private void Awake()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();

        unit.OnAttack += PlayAttackAnimation;
        unit.OnDeath += PlayDeathAnimation;
        unit.OnHit += PlayHitAnimation;
    }



    Transform GetSpriteBody()
    {
        string[] spritePaths = { "SpriteBody" };
        Transform spriteT = null;
        foreach (string path in spritePaths)
        {
            spriteT = transform.Find(path);
            if (spriteT)
                break;
        }
        return spriteT;
    }

    private void Update()
    {
        if (IsUnitDisabled())
            return;
        AnimatorClipInfo[] animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);

        //AnimatorClipInfo animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
        if (animatorClipInfo.Length <= 0)
            return;
        string clipName = animatorClipInfo[0].clip.name;
        //if (!GetComponent<Unit>().Target && clipName != "Run" && clipName != "Attack")
        if (!unit.Target || (!unit.EnoughRangeToAttackTarget() && clipName != "Attack"))
        {
            if (animator.HasState(0, Animator.StringToHash("Run")))
                animator.Play("Run");
        }
    }

    private void OnEnable()
    {
        animator.keepAnimatorStateOnDisable = true;
        GetSpriteBody().rotation = Quaternion.identity;
    }

    bool IsUnitDisabled()
    {
        return !unit.gameObject.activeSelf || unit.Disabled || unit.paralysed;
    }
    private void PlayAttackAnimation()
    {
        if (IsUnitDisabled())
            return;
        animator.Play("Attack");
    }

    void PlayHitAnimation()
    {
        if (animator.HasState(0, Animator.StringToHash("Hit")))
            animator.Play("Hit");
    }

    private void PlayDeathAnimation()
    {
        //if (IsUnitDisabled())
        //    return;
        animator.Play("Death");
    }
}
