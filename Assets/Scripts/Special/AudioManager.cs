using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public List<Sound> sounds;

    public bool musicMuted = false;

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
        for (int i = 0; i < 1; i++)
        {
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }
    }

    public void PlaySfx(string sfxName)
    {
        if (sfxName == "")
            sfxName = Constants.BUTTON_CLICK_SFX_NAME;
        Play(sfxName, false);
    }

    public void PlaySfxWithPitch(string sfxName)
    {
        if (sfxName == "")
            sfxName = Constants.BUTTON_CLICK_SFX_NAME;
        Play(sfxName, true);
    }

    public void Play(string _name, bool randomPitch = false, float pitchPower = 0.1f)
    {
        if (SFXMuted || GetNumberOfAudioPlaying() > 25)
            return;
        Sound newS = sounds.Find(Sound => Sound.name == _name && Sound.source.isPlaying == false);
        //sound not playing wasn't found ?
        if (newS == null)
        {
            //Get a model of the sound.
            Sound s = sounds.Find(Sound => Sound.name == _name);

            if (s != null)
            {
                //Create and add the new Sound to the list.
                newS = AddSound(s.name, s.clip, s.volume, s.pitch, s.loop);
            }
            //It means that the name is wrong !
            else
            {
                Debug.LogWarning("Sound name : " + _name + " not found !");
                return;
            }
        }
        else
        {
            if (randomPitch == true)
                newS.source.pitch = UnityEngine.Random.Range(1f - pitchPower, 1f + pitchPower);
            newS.source.Play();
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

    public void MuteSfxVolume(GameObject _image)
    {
        SFXMuted = !SFXMuted;
        if (_image != null)
            _image.SetActive(SFXMuted);
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
