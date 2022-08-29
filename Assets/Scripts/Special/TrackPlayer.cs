using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackPlayer : MonoBehaviour
{
    public AudioSource myAudio;
    public AudioClip[] myMusic; // declare this as Object array
    public static TrackPlayer instance;
    const string MAIN_THEME_NAME = "Triumphant ZakharValaha";
    const string MENU_THEME_NAME = "TriumphantMenuTheme";

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
        if (SceneManager.GetActiveScene().name != "Stage")
            PlayMenuTheme();
        //SceneManager.sceneLoaded += PlayCorrectThemeOnSceneLoaded;
    }

    void Update()
    {
        //if (!myAudio.isPlaying)
        //    PlayCorrectTheme();
    }

    void PlayCorrectThemeOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!myAudio.isPlaying)
            PlayMenuTheme();
    }

    void PlayCorrectTheme()
    {
        if (SceneManager.GetActiveScene().name != "Stage")
            PlayMenuTheme();
        else
            PlayMainTheme();
    }


    public void PlayMainTheme()
    {
        PlayTrack(MAIN_THEME_NAME);
    }

    public void PlayMenuTheme()
    {
        PlayTrack(MENU_THEME_NAME);
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
