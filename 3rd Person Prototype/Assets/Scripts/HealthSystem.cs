using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;

    private int _currentHealth;
    public int currentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;
            if (currentHealth > maxHealth) currentHealth = maxHealth;

            float currentHealthPercent = (float)currentHealth / (float)maxHealth;
            OnHealthPercentChanged(currentHealthPercent);
        }
    }

    public bool Invulnerable = false;
    public event Action<float> OnHealthPercentChanged = delegate { };
    public event Action<int> OnTakeDamage = delegate { };
    public event Action<GameObject> OnObjectDeath = delegate { };

    [Header("Low Health Vignette")]
    static public VolumeProfile volumeProfile;
    UnityEngine.Rendering.Universal.Vignette vignette;

    public float fadeInTime = 0.5f;

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
            ModifyHealth(-1);
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }
    public void SetInvulnerable(bool foo)
    {
        Invulnerable = foo;
    }
    public int GetHealth()
    {
        return currentHealth;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetHealth(int health)
    {
        currentHealth = health;
    }

    public void SetMaxHealth(int foo)
    {
        maxHealth = foo;
        currentHealth = foo;
    }

    public void ModifyHealth(int amount)
    {
        if (Invulnerable == false && currentHealth >= 0)
        {
            if (transform.tag == "Player")
            {
                currentHealth += amount;

                float currentHealthPercent = (float)currentHealth / (float)maxHealth;
                Debug.Log("Current Health Percent: " + currentHealthPercent);
                OnHealthPercentChanged(currentHealthPercent);

                //Check if health has fallen below zero
                if (currentHealth <= 0.0f)
                {
                    OnObjectDeath?.Invoke(gameObject);
                    Debug.Log("Player has Died");
                }
            }
            OnTakeDamage(amount);
        }
    }
}