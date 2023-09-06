using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMOD.Studio;

public sealed class AudioManager
{
    // replace with #unityevents later
    public event EventHandler ManagerInitialized;
    public event EventHandler ManagerRemoved;
    public event EventHandler ManagerLoaded;

    private static AudioManager _instance;
    public static AudioManager instance
    {
        get
        {
            return _instance;
        }
        set
        {
            if (_instance == null)
                _instance = new AudioManager();
            else
                Debug.LogWarning("SoundManager has already been initialized please use the Master Manager instead.");
            _instance.OnManagerInitialized();
        }      
    }

    ~AudioManager()
    {
        OnManagerRemoved();
    }

    [Header("Voice Channel Assignment")]
    private static VCA VCA_Master;
    private static VCA VCA_Ambient;
    private static VCA VCA_Effects;
    private static VCA VCA_UI;
    private static VCA VCA_Voice;
    private static VCA VCA_Music;

    [Header("Volume Sliders")]
    [SerializeField]
    [Range(-80f, 10f)]
    private float MasterVolume;

    [SerializeField]
    [Range(-80f, 10f)]
    private float SoundFXVolume;

    [SerializeField]
    [Range(-80f, 10f)]
    private float AmbientVolume;

    [SerializeField]
    [Range(-80f, 10f)]
    private float DialogueVolume;

    [SerializeField]
    [Range(-80f, 10f)]
    private float MusicVolume;

    [SerializeField]
    [Range(-80f, 10f)]
    private float UIVolume;

    void OnManagerInitialized()
    {
        InitVCA();
        ManagerInitialized.Invoke(this, EventArgs.Empty);
    }

    void OnManagerRemoved()
    {
        ManagerRemoved.Invoke(this, EventArgs.Empty);
    }
    void InitVCA()
    {
        VCA_Master = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
        VCA_Voice = FMODUnity.RuntimeManager.GetVCA("vca:/VoiceLines");
        VCA_Effects = FMODUnity.RuntimeManager.GetVCA("vca:/SFX");
        VCA_Music = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
        VCA_Ambient = FMODUnity.RuntimeManager.GetVCA("vca:/Ambience");
        VCA_UI = FMODUnity.RuntimeManager.GetVCA("vca:/UI");
    }

    public void SetVolume()
    {
        VCA_Master.setVolume(MasterVolume);
        VCA_Voice.setVolume(DialogueVolume);
        VCA_Effects.setVolume(SoundFXVolume);
        VCA_Music.setVolume(MusicVolume);
        VCA_Ambient.setVolume(AmbientVolume);
        VCA_UI.setVolume(UIVolume);
    }

    public void SetParameter(EventInstance audioEvent, string parameterName, float value)
    {
        audioEvent.setParameterByName(parameterName, value);      
    }

    public void SetParameter(EventInstance audioEvent, PARAMETER_ID ID, float value)
    {
        audioEvent.setParameterByID(ID, value);
    }
}
