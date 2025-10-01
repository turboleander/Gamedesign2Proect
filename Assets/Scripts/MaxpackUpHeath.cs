using UnityEngine;

public class MaxpackUpHeath : MonoBehaviour
{
    public int healthRestore = 60;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Heal(healthRestore);
            Destroy(gameObject);
        }
    }
}
