using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // set up the fields for the player health.
    private int maxHealth;
    private int currentHealth;
    [SerializeField]
    private UnityEngine.UI.Slider healthbar;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // I want a method that allows me to set the health the player/character has.
    void SetHealth(int amount)
    {
        currentHealth = maxHealth = amount;
        OnHealthChange();
    }

    void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChange();
    }

    // Update the healthbar in the UI.
    void OnHealthChange()
    {
        healthbar.value = (float)currentHealth / (float)maxHealth;
    }

    // set up a corutine that let's me set a delay from being hit until the player takes damage.
    public IEnumerator TakeDamageDelayed(int amount, float delay)
    {
        yield return new WaitForSeconds(delay);
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        if (currentHealth == 0)
        {
            SetHealth(20);
        }
        OnHealthChange();
    }
}
