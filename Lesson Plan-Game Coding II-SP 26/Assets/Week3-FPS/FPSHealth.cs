using System;
using UnityEngine;

public class FPSHealth : MonoBehaviour
{ 
    public event Action<float, float> OnHealthChanged;
    
    public float maxHealth = 100f;
    public float currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(maxHealth <= 0) maxHealth = 100f; //default max health if it is not set
        currentHealth = maxHealth;
        Debug.Log("health reset to max");
    }

    public void ChangeHealth(float amount)
    {
        Debug.Log($"before change - current health: {currentHealth}, max health: {maxHealth}");
        
        float oldHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        Debug.Log($"After change - current health: {currentHealth}, max health: {maxHealth}");
        OnHealthChanged?.Invoke(oldHealth, maxHealth);
        Debug.Log($"health changed {oldHealth} to {currentHealth}   ");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HealthPickup"))
        {
            ChangeHealth(+20);
            Debug.Log($"health picked up {currentHealth}");
            Destroy(other.gameObject);
        }
    }
}
