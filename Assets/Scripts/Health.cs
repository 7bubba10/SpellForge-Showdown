using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    bool isDead = false;

    public int CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        var controller = GetComponent<FirstPersonController>();
        if (controller) controller.enabled = false;

        var cam = GetComponentInChildren<Camera>();
        if (cam) cam.enabled = false;

        SceneManager.LoadScene("TestScene");
    }
}