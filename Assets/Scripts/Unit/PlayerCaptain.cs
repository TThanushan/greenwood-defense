using UnityEngine;

public class PlayerCaptain : Unit
{
    public GameObject phoenixEffect;

    bool isPhoenix;
    protected override void Start()
    {
        base.Start();
        foreach (string name in SaveManager.instance.unlockedHeroUpgrades)
        {
            if (name == "Phoenix1")
            {
                isPhoenix = true;
                transform.Find("SpriteBody/PhoenixEnabled").gameObject.SetActive(true);
            }
        }
    }
    public override void GetDamage(float damage, Transform caller, string HitSoundName = "")
    {
        if (isPhoenix && damage >= currentHealth)
        {
            TriggerPhoenix();
            return;
        }

        base.GetDamage(damage, caller, HitSoundName);

    }

    void TriggerPhoenix()
    {
        isPhoenix = false;
        currentHealth = maxHealth;
        SaveManager.instance.unlockedHeroUpgrades.Remove("Phoenix1");
        SaveManager.instance.unlockedHeroUpgrades.Add("Phoenix0");
        poolObject.GetPoolObject(phoenixEffect).transform.position = transform.position;
        transform.Find("SpriteBody/PhoenixEnabled").gameObject.SetActive(false);
    }


}
