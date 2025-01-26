using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour

{

    public TMP_InputField inputText;

    public TMP_Text textName;
    public Image light;
    public GameObject HostButton;
    public GameObject ClientButton;
    public GameObject ServerButton;
    public GameObject ReturnButton;
    private string startMode;


    private void Awake()
    {
        light.color = Color.red;

    }

    private void Update()
    {
        if (textName.text.Length < 4)
        {
            light.color = Color.red;
            HostButton.SetActive(false);
            ClientButton.SetActive(false);
            ServerButton.SetActive(false);
            ReturnButton.SetActive(false);
        }

        if (textName.text.Length >= 4)
        {
            light.color = Color.green;
            HostButton.SetActive(true);
            ClientButton.SetActive(true);
            ServerButton.SetActive(true);
            ReturnButton.SetActive(true);
            
        }
    }

    public void Host()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            startMode = "Host";
            SceneManager.sceneLoaded += OnSceneLoaded; // Suscribe el callback
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetString("Jugador 1", inputText.text);
            SceneManager.LoadScene("SelecCharter");
        }
    }

    public void Client()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            startMode = "Client";
            SceneManager.sceneLoaded += OnSceneLoaded; // Suscribe el callback
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetString("Jugador 1", inputText.text);
            SceneManager.LoadScene("SelecCharter");
        }
    }

    public void Server()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            startMode = "Server";
            SceneManager.sceneLoaded += OnSceneLoaded; // Suscribe el callback
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetString("Jugador 1", inputText.text);
            SceneManager.LoadScene("SelecCharter");
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


    public void acept()
    {
        PlayerPrefs.SetString("Jugador 1", inputText.text);
        SceneManager.LoadScene("SelecCharter");
    }
}
