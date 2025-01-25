using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInitial : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
        Debug.Log("exit ...");
        Application.Quit();
    }
}
