using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] Sound[] bubbleSound, sfxSound, clickSound, coinSound;
    [SerializeField] AudioSource bubbleSource, sfxSource, clickSource, coinSource;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBubbleSound(int size)
    {
        string bubbleSoundName = "bubble{0}";
        Sound s = Array.Find(bubbleSound, x => x.name == string.Format(bubbleSoundName, size));

        if(s == null)
        {
            Debug.Log($"Sound not found: ");
        }
        else
        {
            bubbleSource.clip = s.clip;
            bubbleSource.Play();
        }
    }

    public void PlaySFXSound(string name)
    {
        Sound s = Array.Find(sfxSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.Play();
        }
    }

    public void PlayClickSound(string name)
    {
        Sound s = Array.Find(clickSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            clickSource.clip = s.clip;
            clickSource.Play();
        }
    }

    public void PlayCoinSound(string name)
    {
        Sound s = Array.Find(coinSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            coinSource.clip = s.clip;
            coinSource.Play();
        }
    }

    public void ToggleSound(bool soundAvailable)
    {
        bubbleSource.mute = !soundAvailable;
        sfxSource.mute = !soundAvailable;
        clickSource.mute = !soundAvailable;
        coinSource.mute = !soundAvailable;
    }
}
