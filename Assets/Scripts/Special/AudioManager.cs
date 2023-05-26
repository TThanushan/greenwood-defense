using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public List<Sound> sounds;

    public bool SFXMuted = false;

    void Start()
    {
        AudioListener.volume = 0.10f;
    }

    int GetNumberOfAudioPlaying()
    {
        AudioSource[] audios = GetComponents<AudioSource>();
        int i = 0;
        foreach (AudioSource audioSource in audios)
        {
            if (audioSource.isPlaying)
                i++;
        }

        return i;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        InitSources();
    }

    void InitSources()
    {
        //for (int i = 0; i < 1; i++)
        //{
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        //}
    }



    /// <summary>
    /// Use this to add more audio source in case the sound risks to be used a lot before the audio has finished playing.
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="count"></param>
    public void AddSoundAudioSource(string soundName, int count = 1, float volume = -1)
    {
        Sound s = sounds.Find(Sound => Sound.name == soundName);
        if (s == null)
            return;
        for (int i = 0; i < count; i++)
        {
            s = AddSound(s.name, s.clip, s.volume, s.pitch, s.loop);
            if (volume != -1)
                s.volume = volume;
            //s.source = gameObject.AddComponent<AudioSource>();
            //s.source.clip = s.clip;
            //s.source.volume = s.volume;
            //s.source.pitch = s.pitch;
            //s.source.loop = s.loop;
        }
    }

    public void PlaySfx(string sfxName, float _volume = -1)
    {
        if (sfxName == "")
            sfxName = Constants.BUTTON_CLICK_SFX_NAME;
        Play(sfxName, false, volume: _volume);
    }

    public void PlaySfxWithPitch(string sfxName)
    {
        if (sfxName == "")
            sfxName = Constants.BUTTON_CLICK_SFX_NAME;
        Play(sfxName, true);
    }

    public void Play(string _name, bool randomPitch = false, float pitchPower = 0.1f, float volume = -1f)
    {
        if (SFXMuted || GetNumberOfAudioPlaying() > 25)
            return;
        Sound soundNotPlaying;
        if (volume != -1f)
            soundNotPlaying = sounds.Find(Sound => Sound.name == _name && Sound.source.isPlaying == false && Sound.source.volume == volume);
        else
            soundNotPlaying = sounds.Find(Sound => Sound.name == _name && Sound.source.isPlaying == false);
        //sound not playing is found ?
        if (soundNotPlaying != null)
        {
            if (randomPitch == true)
                soundNotPlaying.source.pitch = Random.Range(1f - pitchPower, 1f + pitchPower);
            soundNotPlaying.source.Play();
        }
        else
        {
            //Get a model of the sound.
            Sound s = sounds.Find(Sound => Sound.name == _name);
            if (s != null)
            {
                float newVolume = s.volume;
                if (volume != -1f)
                    newVolume = volume;
                soundNotPlaying = AddSound(s.name, s.clip, newVolume, s.pitch, s.loop);
                soundNotPlaying.source.Play();
            }


        }
    }

    public void PlayHitSound(string name = "")
    {
        if (name != "")
        {
            PlaySfx(name);
            return;
        }

        string[] names = { "Hit1", "Hit2", "Hit3" };
        int i = Random.Range(0, names.Length);
        PlaySfx(names[i]);
    }

    public void MuteSfxVolume()
    {
        SFXMuted = !SFXMuted;
    }

    public void MuteSfxVolume(bool value)
    {
        SFXMuted = value;
    }


    public void ChangeMainVolume(float _volume)
    {
        AudioListener.volume = _volume;

    }

    Sound AddSound(string name, AudioClip clip, float volume, float pitch, bool loop)
    {
        Sound s = new Sound();

        s.source = gameObject.AddComponent<AudioSource>();
        s.name = name;
        s.clip = clip;
        s.volume = volume;
        s.pitch = pitch;
        s.loop = loop;

        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;

        sounds.Add(s);
        return s;
    }
}
