using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    [Header("UI")]
    public Slider healthBar;   // เปลี่ยนจาก Scrollbar → Slider
    public GameObject deadScreen;

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHP;
            healthBar.value = currentHP;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
        UpdateHealthBar();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.value = currentHP;
    }

    private void Die()
    {
        isDead = true;
        if (deadScreen != null)
            deadScreen.SetActive(true);
    }
}
