using UnityEngine;

public class ParticleGlobal : MonoBehaviour
{
    public bool LoopSwitch = true;
    private bool prevLoopSwitch = true;

    [Range(0, 5)]
    public float AnimationSpeed = 0.5f;
    private float prevAnimationSpeed;

    [Range(-38, 476)]
    public float CameraX;

    [Range(-20, -117)]
    public float CameraY;

    [Range(10, 200)]
    public float CameraZoom;

    public Color Background;

    ParticleSystem[] allParticles;

    void Start()
    {
        CameraX = Camera.main.transform.position.x;
        CameraY = Camera.main.transform.position.y;
        CameraZoom = Camera.main.orthographicSize;
        Background = Camera.main.backgroundColor;

        allParticles = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (AnimationSpeed != prevAnimationSpeed)
        {
            foreach (ParticleSystem particle in allParticles)
                particle.playbackSpeed = AnimationSpeed;
        }
        prevAnimationSpeed = AnimationSpeed;

        if (LoopSwitch != prevLoopSwitch)
        {
            foreach (ParticleSystem particle in allParticles)
            {
                if (!LoopSwitch)
                    particle.Stop();
                else
                    particle.Play();

                particle.loop = LoopSwitch;
            }
        }
        prevLoopSwitch = LoopSwitch;


        Camera.main.transform.position = new Vector3(CameraX, CameraY, -10);
        Camera.main.orthographicSize = CameraZoom;

        Camera.main.backgroundColor = Background;
    }
}
