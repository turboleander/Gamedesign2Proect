using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("UI")]
    public Scrollbar healthBar;   // drag Scrollbar เข้ามาใน Inspector
    public GameObject deadScreen; // หน้าตาย (เช่น Panel)

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        UpdateHealthBar();
    }

    /// <summary>
    /// รับ damage
    /// </summary>
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

    /// <summary>
    /// Heal HP
    /// </summary>
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
        {
            healthBar.size = (float)currentHP / maxHP;
        }
    }

    private void Die()
    {
        isDead = true;

        if (deadScreen != null)
            deadScreen.SetActive(true);

        // disable movement หรือยิง event
        var move = GetComponent<PlayerMovement>();
        if (move != null)
            move.enabled = false;

        // ถ้าอยาก reset game ก็เรียก SceneManager.LoadScene(...)
    }
}
