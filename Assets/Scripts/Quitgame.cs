using UnityEngine;

public class Quitgame : MonoBehaviour
{
    public void QuitGame()
    {
        // 1. แสดงข้อความใน Console (เพื่อให้รู้ว่ากดติด)
        Debug.Log("กดออกจากเกมแล้ว! (QUIT GAME)");

        // 2. คำสั่งปิดเกมจริงๆ (ทำงานเฉพาะตอน Build เป็นไฟล์เกมแล้ว)
        Application.Quit();

        // 3. คำสั่งหยุดเล่นใน Unity Editor (ช่วยให้เราเทสปุ่มได้เลยไม่ต้อง Build)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
