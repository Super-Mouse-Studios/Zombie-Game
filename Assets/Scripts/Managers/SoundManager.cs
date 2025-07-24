using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    // Enum for sound types
    public enum SoundType
    {
        SOUND_SFX,
        SOUND_MUSIC
    }

    public static SoundManager Instance { get; private set; } // 

    // Create Dictionary for sfx.
    // Dictionary to hold sound effects
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();

    // Create Dictionary for music.
    // Dictionary to hold music tracks
    private Dictionary<string, AudioClip> musicDictionary = new Dictionary<string, AudioClip>();

    // Create AudioSource for sfx.
    // AudioSource to play sound effects in unity
    private AudioSource sfxSource; // Create AudioSource for sfx.

    // Create AudioSource for music.
    // AudioSource to play music in unity
    private AudioSource musicSource; // Create AudioSource for music.
    private AudioSource loopedSFXSource; // Separate AudioSource for looped SFX

    // Check if the SoundManager instance already exists.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject); // Destroy this object if an instance already exists.
        }
    }

    void Start()
    {
        // Adds all sounds to the library on Start
        AddSound("Shotgun", Resources.Load<AudioClip>("Shotgun"), SoundType.SOUND_SFX);
        AddSound("Knife", Resources.Load<AudioClip>("Knife"), SoundType.SOUND_SFX);
        AddSound("ZombieDeath", Resources.Load<AudioClip>("ZombieDeath"), SoundType.SOUND_SFX);
        AddSound("Chaingun", Resources.Load<AudioClip>("Chaingun"), SoundType.SOUND_SFX);
        AddSound("Dash", Resources.Load<AudioClip>("Dash"), SoundType.SOUND_SFX);
        AddSound("Explosion", Resources.Load<AudioClip>("Explosion"), SoundType.SOUND_SFX);
        AddSound("Rocket", Resources.Load<AudioClip>("Rocket"), SoundType.SOUND_SFX);
        AddSound("Charger", Resources.Load<AudioClip>("Charger"), SoundType.SOUND_SFX);
        AddSound("AR", Resources.Load<AudioClip>("AR"), SoundType.SOUND_SFX);
        AddSound("Reload", Resources.Load<AudioClip>("Reload"), SoundType.SOUND_SFX);
        AddSound("Drink", Resources.Load<AudioClip>("Drink"), SoundType.SOUND_SFX);
        AddSound("FireRate", Resources.Load<AudioClip>("FireRate"), SoundType.SOUND_SFX);
        AddSound("ItemDrop", Resources.Load<AudioClip>("ItemDrop"), SoundType.SOUND_SFX);
        AddSound("DamageUp", Resources.Load<AudioClip>("DamageUp"), SoundType.SOUND_SFX);
        AddSound("AttackSize", Resources.Load<AudioClip>("AttackSize"), SoundType.SOUND_SFX);
        AddSound("Rain", Resources.Load<AudioClip>("Rain"), SoundType.SOUND_SFX);
        AddSound("UnlimitedFireRate", Resources.Load<AudioClip>("UnlimitedFireRate"), SoundType.SOUND_SFX);
        AddSound("Heal", Resources.Load<AudioClip>("Heal"), SoundType.SOUND_SFX);
        AddSound("PlayerHurt", Resources.Load<AudioClip>("PlayerHurt"), SoundType.SOUND_SFX);
    }

    // Initialize the SoundManager. I just put this functionality here instead of in the static constructor.
    // Creates a new GameObject to hold the AudioSource components.
    private void Initialize()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = 1.0f;

        loopedSFXSource = gameObject.AddComponent<AudioSource>();
        loopedSFXSource.volume = 1.0f;
        loopedSFXSource.loop = true;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.volume = 1.0f;
        musicSource.loop = true;
    }

    // Add a sound to the dictionary.
    public void AddSound(string soundKey, AudioClip audioClip, SoundType soundType)
    {
        // Fill in for lab.
        Dictionary<string, AudioClip> targetDictionary = GetDictionaryByType(soundType);

        if (!targetDictionary.ContainsKey(soundKey))
        {
            targetDictionary.Add(soundKey, audioClip);
        }
        else
        {
            Debug.LogWarning("Sound key " + soundKey + " already exists in the " + soundType + " dictionary.");
        }
    }

    // Play a sound by key interface.
    public void PlaySound(string soundKey)
    {
        // Fill in for lab.
        Play(soundKey, SoundType.SOUND_SFX);
    }

    // Play music by key interface.
    public void PlayMusic(string soundKey)
    {
        // Fill in for lab.
        musicSource.Stop();
        Play(soundKey, SoundType.SOUND_MUSIC);
    }

    // Play utility.
    private void Play(string soundKey, SoundType soundType)
    {
        // Fill in for lab.
        Dictionary<string, AudioClip> targetDictionary;
        AudioSource targetSource;

        SetTargetsByType(soundType, out targetDictionary, out targetSource);

        if (targetSource == null)
        {
            Debug.LogWarning("SoundManager: targetSource is null for sound type: " + soundType);
            return;
        }
        if (targetDictionary == null)
        {
            Debug.LogWarning("SoundManager: targetDictionary is null for sound type: " + soundType);
            return;
        }
        if (!targetDictionary.ContainsKey(soundKey))
        {
            Debug.LogWarning("SoundManager: Sound key " + soundKey + " does not exist in the " + soundType + " dictionary.");
            return;
        }
        if (targetDictionary[soundKey] == null)
        {
            Debug.LogWarning("SoundManager: AudioClip for key " + soundKey + " is null.");
            return;
        }

        targetSource.PlayOneShot(targetDictionary[soundKey]);
        if (targetDictionary.ContainsKey(soundKey))
        {
        }
        else
        {
            Debug.LogWarning("Sound key " + soundKey + " does not exist in the " + soundType + " dictionary.");
        }

    }


    private void SetTargetsByType(SoundType soundType, out Dictionary<string, AudioClip> targetDictionary, out AudioSource targetSource)
    {
        // Fill in for lab.
        switch (soundType)
        {
            case SoundType.SOUND_SFX:
                targetDictionary = sfxDictionary;
                targetSource = sfxSource;
                break;
            case SoundType.SOUND_MUSIC:
                targetDictionary = musicDictionary;
                targetSource = musicSource;
                break;
            default:
                Debug.LogError("Unknown sound type: " + soundType);
                targetDictionary = null;
                targetSource = null;
                break;
        }
    }


    private Dictionary<string, AudioClip> GetDictionaryByType(SoundType soundType)
    {
        // Fill in for lab.
        switch (soundType)
        {
            case SoundType.SOUND_SFX:
                return sfxDictionary;
            case SoundType.SOUND_MUSIC:
                return musicDictionary;
            default:
                Debug.LogError("Unknown sound type: " + soundType);
                return null;
        }
    }

    // Volume variables
    private float masterVolume = 1.0f; // Master volume
    private float sfxVolume = 1.0f; // SFX volume
    private float musicVolume = 1.0f; // Music volume

    // Master volume control
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume; // Set the master volume.
        sfxSource.volume = (sfxVolume * masterVolume); // Set the volume of the sfx source.
        musicSource.volume = (musicVolume * masterVolume); // Set the volume of the music source.
    }

    // Volume control for sfx and music
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume * masterVolume;
        loopedSFXSource.volume = volume * masterVolume;
    }

    public void StereoPanning(float pan)
    {
        sfxSource.panStereo = pan;
        loopedSFXSource.panStereo = pan;
        musicSource.panStereo = pan;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume; // Set the volume of the music source.
        musicSource.volume = (volume * masterVolume); // Set the volume of the music source.
    }

    public void PlayLoopedSound(string soundKey)
    {
        if (loopedSFXSource.isPlaying) return;
        if (sfxDictionary.ContainsKey(soundKey))
        {
            loopedSFXSource.clip = sfxDictionary[soundKey];
            loopedSFXSource.loop = true;
            loopedSFXSource.Play();
        }
    }

    public void StopLoopedSound()
    {
        loopedSFXSource.Stop();
        loopedSFXSource.clip = null;
        loopedSFXSource.loop = false;
    }
}