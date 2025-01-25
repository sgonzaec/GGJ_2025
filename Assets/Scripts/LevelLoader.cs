using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public void LoadNewLevel(string LevelName)
    {
        SceneManager.LoadScene(LevelName);
    }
}
