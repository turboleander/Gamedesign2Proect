using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("UI")]
    public Scrollbar healthBar;   // drag Scrollbar ������ Inspector
    public GameObject deadScreen; // ˹�ҵ�� (�� Panel)

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        UpdateHealthBar();
    }

    /// <summary>
    /// �Ѻ damage
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

        // disable movement �����ԧ event
        var move = GetComponent<PlayerMovement>();
        if (move != null)
            move.enabled = false;

        // �����ҡ reset game �����¡ SceneManager.LoadScene(...)
    }
}
