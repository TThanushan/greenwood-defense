using UnityEngine;

public class PlayerWalkingSfx : MonoBehaviour
{

    private AudioManager audioManager;
    public float nextPlayTime;
    public float timeBetweenPlay = 0.25f;
    public float shakeIntensity = 1.0f;
    public float shakeD = 1.0f;
    string sfxName = "GrassWalking";
    int maxSfx = 9;
    CameraController shakeCamera;
    void Start()
    {
        audioManager = AudioManager.instance;
        shakeCamera = CameraController.instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            shakeCamera.Shake(shakeIntensity, shakeD);
        }
    }
    public void PlayWalkingSound()
    {
        if (nextPlayTime > Time.time)
            return;
        audioManager.PlaySfx(sfxName + Random.Range(1, maxSfx).ToString(), _volume: 0.25f);
        nextPlayTime = Time.time + timeBetweenPlay;
    }



}
