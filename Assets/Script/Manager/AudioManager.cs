using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", volume);
    }

    public void SetUIVolume(float volume) {
        audioMixer.SetFloat("UIVolume", volume);
    }

    public void SetSoundEffectVolume(float volume)
    {
        audioMixer.SetFloat("SEVolume", volume);
    }
}
