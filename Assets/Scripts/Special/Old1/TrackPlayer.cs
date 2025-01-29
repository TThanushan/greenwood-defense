using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackPlayer : MonoBehaviour
{
    public static TrackPlayer instance;

    public AudioClip menuMusic;
    public AudioClipInfo[] stageMusicInfo; // Replace the stageMusic array with this

    private AudioSource audioSource;
    private string currentSceneName = "";
    private AudioClip lastPlayedClip;
    private bool isMenuMusicPlaying = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();

        if (currentSceneName == scene.name && isMenuMusicPlaying) return;

        currentSceneName = scene.name;
        switch (scene.name)
        {
            case "Menu":
            case "LevelSelect":
            case "Upgrades":
                PlayMenuMusic();
                break;
            case "Stage":
                _ = StartCoroutine(PlayRandomStageMusicWithDelay());
                break;
            default:
                isMenuMusicPlaying = false;
                break;
        }
    }

    void PlayMenuMusic()
    {
        if (!isMenuMusicPlaying)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
            isMenuMusicPlaying = true;
        }
    }

    IEnumerator PlayRandomStageMusicWithDelay()
    {
        isMenuMusicPlaying = false;

        while (currentSceneName == "Stage")
        {
            AudioClipInfo selectedTrack = GetRandomClipNotPlayedRecently();
            audioSource.clip = selectedTrack.clip;
            audioSource.loop = selectedTrack.loop;
            audioSource.Play();
            lastPlayedClip = audioSource.clip;

            if (selectedTrack.loop)
            {
                break; // Exit the coroutine if the track is looping.
            }
            else
            {
                yield return new WaitForSeconds(audioSource.clip.length);
                float randomSilenceDuration = Random.Range(3f, 10f);
                yield return new WaitForSeconds(randomSilenceDuration);
            }
        }
    }

    AudioClipInfo GetRandomClipNotPlayedRecently()
    {
        // Calculate the total weight
        int totalWeight = stageMusicInfo.Sum(info => info.chanceRate);
        int randomNumber = Random.Range(0, totalWeight);
        int cumulative = 0;

        // Weighted random selection
        foreach (var info in stageMusicInfo)
        {
            cumulative += info.chanceRate;
            if (randomNumber < cumulative)
            {
                lastPlayedClip = info.clip; // Update the lastPlayedClip
                return info;
            }
        }

        return stageMusicInfo[0]; // Fallback
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    [System.Serializable]
    public class AudioClipInfo
    {
        public AudioClip clip;
        [Range(1, 100)]
        public int chanceRate = 1;
        public bool loop = false;
    }
}
