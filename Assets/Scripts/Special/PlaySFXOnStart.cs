using UnityEngine;

public class PlaySFXOnStart : MonoBehaviour
{
    public string sFXName;
    void Start()
    {
        AudioManager.instance.PlaySfx(sFXName);
    }
}
