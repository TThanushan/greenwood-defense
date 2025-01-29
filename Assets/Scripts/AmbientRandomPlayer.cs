using System.Linq;
using UnityEngine;

public class AmbientRandomPlayer : MonoBehaviour
{

    public AudioClipInfo[] stageMusicInfo; // Replace the stageMusic array with this

    private AudioSource audioSource;
    private AudioClip lastPlayedClip;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        PlayRandomStageMusic();
    }

    // Make a method that will play a random track with no delay.
    public void PlayRandomStageMusic()
    {
        AudioClipInfo selectedTrack = GetRandomClipNotPlayedRecently();
        audioSource.clip = selectedTrack.clip;
        audioSource.loop = selectedTrack.loop;
        audioSource.Play();
        lastPlayedClip = audioSource.clip;
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


    [System.Serializable]
    public class AudioClipInfo
    {
        public AudioClip clip;
        [Range(1, 100)]
        public int chanceRate = 1;
        public bool loop = false;
    }
}
