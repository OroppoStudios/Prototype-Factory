using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem.LowLevel;


public class SoundManager : MonoBehaviour
{
    // replace with #unityevents later
    public event EventHandler ManagerInitialized;
    public event EventHandler ManagerAccessed;

    private static SoundManager _instance;
    public static SoundManager instance
    {
        get
        {
            return _instance;

        }
        set
        {
            if (_instance == null)
                _instance = value;
        }
    }

    [Header("Voice Channel Assignment.")]
    private static FMOD.Studio.VCA VCA_Master;
    private static FMOD.Studio.VCA VCA_Ambient;
    private static FMOD.Studio.VCA VCA_Effects;
    private static FMOD.Studio.VCA VCA_UI;
    private static FMOD.Studio.VCA VCA_Voice;
    private static FMOD.Studio.VCA VCA_Music;

    void OnManagerInitialized ()
    {
        throw new NotImplementedException();
    }
    void OnManagerAccessed()
    {
        throw new NotImplementedException();
    }
}
