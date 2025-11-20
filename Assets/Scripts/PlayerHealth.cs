using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100;
    public float currentHP;

    [Header("UI")]
    public Slider healthBar;
    public GameObject deadScreen;



    public UnityEvent OnTakeDamage;
    public UnityEvent OnDead;

    void Start()
    {
        currentHP = maxHP;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHP;
            healthBar.value = currentHP;
        }
    }

    public void TakeDamage(float amount)
    {
        if (!IsDead())
        {
            OnTakeDamage.Invoke();
            currentHP -= amount;

            if (IsDead())
            {
                Die();
                OnDead.Invoke();
            }
        }
    }

    public void Heal(int amount)
    {
        if (IsDead()) return;

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
        if (deadScreen != null)
            deadScreen.SetActive(true);


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }
}
