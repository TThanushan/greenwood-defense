using UnityEngine;
[DisallowMultipleComponent]
public class DisableIfUnitDisabled : MonoBehaviour
{
    Unit unit;

    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
    }

    private void Update()
    {
        DisableLoop();
    }

    void DisableLoop()
    {
        if (unit.Disabled)
            EnableChild(false);
        else
            EnableChild(true);
    }

    void EnableChild(bool value)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if (child != null)
                child.SetActive(value);
        }
    }

}
