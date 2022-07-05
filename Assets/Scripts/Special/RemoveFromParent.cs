using UnityEngine;

public class RemoveFromParent : MonoBehaviour
{
    private void OnEnable()
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("Bin").transform);
    }
}
