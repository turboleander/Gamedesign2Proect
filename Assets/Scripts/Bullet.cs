using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        /*if (collision.gameObject.CompareTag("Target"))
        {
            print("Hit" +  collision.gameObject.name + "!");
            Destroy(gameObject);
        }*/
        if (collision.gameObject.CompareTag("Player"))
        {

        }
        else 
        {
            print("Hit" + collision.gameObject.name + "!");
            Destroy(gameObject);
        }
    }
}
