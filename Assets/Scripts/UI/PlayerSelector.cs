using UnityEngine;
using Unity.Netcode;

public class PlayerSelector : MonoBehaviour
{
    public int selectedCharacterIndex = 0; // �ndice del personaje seleccionado.

    public void ConnectAsSelectedCharacter()
    {
        byte[] payload = { (byte)selectedCharacterIndex };

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;

        NetworkManager.Singleton.StartClient();
    }
}
