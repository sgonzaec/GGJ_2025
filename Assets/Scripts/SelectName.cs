using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectName : MonoBehaviour
{
    public TMP_InputField inputText;

    public TMP_Text textName;
    public Image light;
    public GameObject bottonAcept;


    private void Awake()
    {
        light.color = Color.red;

    }

    private void Update()
    {
        if (textName.text.Length < 4)
        {
            light.color = Color.red;
            bottonAcept.SetActive(false);
        }

        if (textName.text.Length >= 4)
        {
            light.color = Color.green;
            bottonAcept.SetActive(true);
            return;
        }
    }

        public void acept()
    {
        PlayerPrefs.SetString("Jugador 1",inputText.text);
        SceneManager.LoadScene("SelecCharter");
    }

}

