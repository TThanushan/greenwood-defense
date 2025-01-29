using UnityEngine;

public class EnableAGameobjectWhenIAmEnabled : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private bool targetValueWhenIAmEnabled = true;

    [Header("Optional OnDisable")]
    [SerializeField] private bool disableWhenIAmDisabled = false;
    [SerializeField] private bool targetValueWhenIAmDisabled = false;

    private void OnEnable()
    {
        if (target != null)
        {
            target.SetActive(targetValueWhenIAmEnabled);
        }
    }

    private void OnDisable()
    {
        if (disableWhenIAmDisabled && target != null)
        {
            target.SetActive(targetValueWhenIAmDisabled);
        }
    }

}
