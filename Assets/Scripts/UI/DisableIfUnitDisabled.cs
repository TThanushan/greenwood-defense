using UnityEngine;

public class DisableIfUnitDisabled : MonoBehaviour
{
    Unit unit;

    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
    }

    private void Update()
    {
        if (unit.Disabled)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        gameObject.SetActive(true);
    }
}
