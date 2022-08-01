using UnityEngine;

public class Ghost1 : Unit
{
    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        base.GetDamage(damage, caller, HitSoundName);
    }

    protected override void Update()
    {
        base.Update();
        if (!ProjectileAffectMe())
            ChangeSpriteColor(new Color(1, 1, 1, 0.5372549f));
        else
            ChangeSpriteColor(Color.white);
    }


    public override bool ProjectileAffectMe()
    {
        if (!EnoughRangeToAttackTarget())
            return false;
        return base.ProjectileAffectMe();
    }
}
