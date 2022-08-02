using UnityEngine;

public class UnitAnimatorManager : MonoBehaviour
{

    Animator animator;
    Unit unit;
    Vector3 startPos;
    Transform unitSpriteTransform;
    private void Awake()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();
        GetComponent<Unit>().OnAttack += PlayAttackAnimation;
        GetComponent<Unit>().OnDeath += PlayDeathAnimation;
        unitSpriteTransform = GetSpriteBody();
    }

    void Start()
    {
        startPos = unitSpriteTransform.localPosition;
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
        if (!unit.Target || !unit.EnoughRangeToAttackTarget() && clipName != "Attack")
            animator.Play("Run");
    }

    private void OnEnable()
    {
        animator.keepAnimatorControllerStateOnDisable = true;
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

    private void PlayDeathAnimation()
    {
        //if (IsUnitDisabled())
        //    return;
        animator.Play("Death");
    }
}
