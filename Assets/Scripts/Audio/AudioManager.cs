using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource pitchedSfxSource;
    [SerializeField] private List<SFXData> soundEffects = new List<SFXData>();

    private Dictionary<SFXNames, AudioClip> sfxDictionary = new Dictionary<SFXNames, AudioClip>();
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else if(instance!=this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach(var sfx in soundEffects)
        {
            sfxDictionary[sfx.name] = sfx.clip;
        }
    }

    /// <summary>
    /// Plays a dictionary audio if manager finds it
    /// </summary>
    /// <param name="sfxName"></param>
    public void PlaySFX(SFXNames sfxName)
    {
        if (sfxDictionary.ContainsKey(sfxName))
        {
            sfxSource.PlayOneShot(sfxDictionary[sfxName]);
        }
    }

    /// <summary>
    /// Plays dictionary audio if manager finds it
    /// Modifies audio source pitch
    /// </summary>
    /// <param name="sfxName"></param>
    /// <param name="pitch"></param>
    public void PlayPitchedSfx(SFXNames sfxName, float pitch)
    {
        if (sfxDictionary.ContainsKey(sfxName))
        {
            pitchedSfxSource.pitch = pitch;
            pitchedSfxSource.clip = sfxDictionary[sfxName];
            pitchedSfxSource.Play();
        }
    }
}
[System.Serializable]
public class SFXData
{
    public SFXNames name;
    public AudioClip clip;
}