using UnityEngine;

public class PlaySFXOnStart : MonoBehaviour
{
    public string sFXName;
    void Start()
    {
        SFXManager.instance.Play(sFXName);
    }
}
