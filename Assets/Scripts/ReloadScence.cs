using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScreen : MonoBehaviour
{
    public void ResetScence()
    {
        
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
