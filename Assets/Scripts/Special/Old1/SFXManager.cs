using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 0.25f;
    [Range(.1f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
    public int priority = 1;  // Default priority
    [HideInInspector]
    public AudioSource source;
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    [SerializeField] private AudioMixerGroup hitsMixerGroup;
    public List<Sound> sounds;
    public bool SFXMuted = false;

    private List<AudioSource> allSources = new List<AudioSource>();
    const int maxAudioSources = 20; // Maximum number of AudioSource instances allowed

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeAudioSources();
        InitializeSounds();
    }

    private void InitializeAudioSources()
    {
        for (int i = 0; i < maxAudioSources; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            allSources.Add(source);
        }
    }

    private void InitializeSounds()
    {
        foreach (var s in sounds)
        {
            s.source = GetAvailableSource(s.priority);
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private AudioSource GetAvailableSource(int requestedPriority)
    {
        AudioSource source = allSources.Find(s => !s.isPlaying);

        if (source == null)
        {
            // Find the lowest priority sound that is playing and can be stopped
            Sound lowestPrioritySound = sounds.Find(s => s.source.isPlaying && s.priority < requestedPriority);
            if (lowestPrioritySound != null)
            {
                lowestPrioritySound.source.Stop();
                source = lowestPrioritySound.source;
            }
            else
            {
                // No available sources and no lower priority sound playing
                return null;
            }
        }

        return source;
    }

    public void Play(string name, bool randomPitch = false, float pitchPower = 0.1f, float volume = -1f, bool useHardValuePitch = false)
    {
        if (SFXMuted) return;

        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }

        AudioSource availableSource = GetAvailableSource(s.priority);
        if (availableSource == null) return; // No available source, do not play sound

        availableSource.clip = s.clip;
        availableSource.volume = volume > 0 ? volume : s.volume;
        availableSource.pitch = useHardValuePitch ? pitchPower : randomPitch ? Random.Range(1f - pitchPower, 1f + pitchPower) : s.pitch;
        availableSource.loop = s.loop;
        availableSource.outputAudioMixerGroup = availableSource.clip.name.Contains("Hit") ? hitsMixerGroup : audioMixerGroup;
        availableSource.Play();
        s.source = availableSource;
    }

    // Make a method like Play that will take an audio clip instead of a name, if the sound is not in the list, add it to the list
    public void Play(AudioClip clip, bool randomPitch = false, float pitchPower = 0.1f, float volume = -1f, bool useHardValuePitch = false)
    {
        if (SFXMuted) return;

        Sound s = sounds.Find(sound => sound.clip == clip);
        if (s == null)
        {
            s = new Sound
            {
                name = clip.name,
                clip = clip,
                volume = volume > 0 ? volume : 0.5f,
                pitch = 1f,
                loop = false,
                priority = 1
            };
            sounds.Add(s);
        }

        AudioSource availableSource = GetAvailableSource(s.priority);
        if (availableSource == null) return; // No available source, do not play sound

        availableSource.clip = s.clip;
        availableSource.volume = volume > 0 ? volume : s.volume;
        availableSource.pitch = useHardValuePitch ? pitchPower : randomPitch ? Random.Range(1f - pitchPower, 1f + pitchPower) : s.pitch;
        availableSource.loop = s.loop;
        availableSource.outputAudioMixerGroup = availableSource.clip.name.Contains("Hit") ? hitsMixerGroup : audioMixerGroup;
        availableSource.Play();
        s.source = availableSource;
    }

    public void PlayHitSound(string name = "")
    {
        if (name != "")
        {
            Play(name);
            return;
        }

        string[] names = { "Hit1", "Hit2", "Hit3" };
        int i = Random.Range(0, names.Length);
        Play(names[i]);
    }

    public void MuteSFX(bool mute = true)
    {
        SFXMuted = mute;
    }

    public void ChangeMainVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
