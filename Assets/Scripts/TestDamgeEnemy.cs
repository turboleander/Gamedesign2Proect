using UnityEngine;

public class DamageOnKey : MonoBehaviour
{
    public KeyCode key = KeyCode.K;
    public float damage = 5f;
    private EnemyHealthBar target; 

    void Awake()
    {
        target = GetComponent<EnemyHealthBar>() ?? GetComponentInParent<EnemyHealthBar>();
    }

    void Update()
    {
        if (Input.GetKeyDown(key) && target)
            target.TakeDamage(damage);
    }
}
