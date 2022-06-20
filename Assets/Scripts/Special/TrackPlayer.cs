using UnityEngine;

public class TrackPlayer : MonoBehaviour
{
    public AudioSource myAudio;
    public AudioClip[] myMusic; // declare this as Object array
    public static TrackPlayer instance;
    const string MAIN_THEME_NAME = "Triumphant ZakharValaha";
    int trackHistory;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (!instance)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        myAudio = GetComponent<AudioSource>();
    }
    void Start()
    {
        if (!myAudio.isPlaying)
            PlayRandomMusic();
    }

    void Update()
    {
        if (!myAudio.isPlaying)
            PlayRandomMusic();
    }

    public void PlayMainTheme()
    {
        PlayTrack(MAIN_THEME_NAME);
    }
    public void PlayBossTrack()
    {
        PlayTrack("BOSS_THEME_cinematic-space-marine-3508");
    }

    public void PlayTrack(string trackName)
    {
        foreach (AudioClip audioClip in myMusic)
        {
            if (audioClip.name == trackName)
                myAudio.clip = audioClip;
        }
        myAudio.Play();
    }

    public void MuteMusic()
    {
        myAudio.mute = !myAudio.mute;
    }

    public void PlayRandomMusic()
    {


        if (myAudio.enabled)
        {
            int rand = Random.Range(0, myMusic.Length);
            while (rand == trackHistory)
                rand = Random.Range(0, myMusic.Length);

            myAudio.clip = myMusic[rand] as AudioClip;
            myAudio.Play();
            trackHistory = rand;
        }
    }
}
