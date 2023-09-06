using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
#region Boilerplate
    public event EventHandler ManagerInitialized;
    public event EventHandler ManagerRemoved;

    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            return _instance;
        }
        set
        {
            if (_instance == null)
                _instance = new GameManager();
            else
                Debug.LogWarning("GameManager has already been initialized please use the Master Manager instead.");
            _instance.OnManagerInitialized();
        }
    }

    ~GameManager()
    {
        OnManagerRemoved();
    }

    void OnManagerInitialized()
    {
        ManagerInitialized.Invoke(this, EventArgs.Empty);
    }

    void OnManagerRemoved()
    {
        ManagerRemoved.Invoke(this, EventArgs.Empty);
    }
    #endregion

    public void BulletTime(float timeSlowStrength = 0.05f, float duration = 1f)
    {
        Time.timeScale = timeSlowStrength;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        StartCoroutine(BulletTimeTimer(duration));
    }
    IEnumerator BulletTimeTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        ResumeTime();
    }
    public void StopTime()
    {
        Time.timeScale = 0.0f;
        Time.fixedDeltaTime = 0.0f;
    }
    public void ResumeTime()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }



}
