using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOptions : MonoBehaviour
{
    private string startMode;

    public void Host()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            startMode = "Host";
            SceneManager.sceneLoaded += OnSceneLoaded; // Suscribe el callback
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void Client()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            startMode = "Client";
            SceneManager.sceneLoaded += OnSceneLoaded; // Suscribe el callback
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void Server()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            startMode = "Server";
            SceneManager.sceneLoaded += OnSceneLoaded; // Suscribe el callback
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Una vez que la escena está cargada, inicia el modo correspondiente
        if (startMode == "Host")
        {
            NetworkManager.Singleton.StartHost();
        }
        else if (startMode == "Client")
        {
            NetworkManager.Singleton.StartClient();
        }
        else if (startMode == "Server")
        {
            NetworkManager.Singleton.StartServer();
        }

        // Limpia el evento para evitar múltiples suscripciones
        SceneManager.sceneLoaded -= OnSceneLoaded;
        startMode = null;
    }
}
