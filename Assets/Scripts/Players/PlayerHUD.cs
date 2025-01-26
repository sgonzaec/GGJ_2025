using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Netcode;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public GameObject playerLifePrefab; // Prefab de UI para mostrar la vida de cada jugador
    public Transform playerListContainer; // Contenedor en el UI donde se a�adir�n los elementos de vida
    public float verticalSpacing = 50f; // Espaciado entre las barras de vida
    private float currentYPosition = 0f; // Para controlar la posici�n Y de cada barra de vida

    private Dictionary<ulong, TMP_Text> playerLivesTexts = new Dictionary<ulong, TMP_Text>();

    private void Start()
    {
        Debug.Log($"PlayerHUD activo: {gameObject.name}");
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager no est� configurado.");
            return;
        }
        // Si es cliente, a�adir a s� mismo al HUD
        if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Cliente conectado, notificando al servidor.");
            NotifyServerOfConnectionServerRpc();
            AddPlayerToHUDClientRpc(NetworkManager.Singleton.LocalClientId, 100); // A�adir el cliente a s� mismo con 100 de vida
            Debug.Log("Es cliente. Suscribi�ndose al evento OnClientConnectedCallback.");
            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
            // Registrar jugadores conectados en el momento en que la escena carga
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Debug.Log($"Jugador ya conectado: {clientId}");
                OnPlayerConnected(clientId);
            }
        }
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Es el servidor. Suscribi�ndose al evento OnClientConnectedCallback.");
            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;

            // Registrar jugadores conectados en el momento en que la escena carga
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Debug.Log($"Jugador ya conectado: {clientId}");
                OnPlayerConnected(clientId);
            }
        }
        else
        {
            Debug.Log("No es el servidor. No se suscribir� al evento.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyServerOfConnectionServerRpc()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        Debug.Log($"Notificando al servidor que el cliente {clientId} se ha conectado.");

        // Llama a OnPlayerConnected manualmente para el servidor
        OnPlayerConnected(clientId);
    }

    private void OnDestroy()
    {
        Debug.Log($"On distroi.");
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerConnected;
        }
    }
    private void OnPlayerConnected(ulong playerId)
    {
        Debug.Log($"Player conectado: {playerId}");

        // Actualizar la lista de jugadores para todos los clientes
        if (NetworkManager.Singleton.IsServer)
        {
            // Despu�s de que un jugador se conecta, notificarle la lista completa de jugadores
            UpdatePlayerListForNewPlayerServerRpc(playerId);
        }
        else {
            UpdatePlayerListForNewPlayerServerRpc(playerId);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerListForNewPlayerServerRpc(ulong newPlayerId)
    {
        var players = FindObjectsOfType<AttibutesManager>();

        // Enviar a este jugador la lista completa de jugadores con sus barras de vida
        foreach (var player in players)
        {
            // Nota: Aseg�rate de que no le env�es su propia barra de vida (aunque esto ya se maneja por el ClientRpc)
            AddPlayerToHUDClientRpc(newPlayerId, player.OwnerClientId, player.health.Value);
        }
    }
    [ClientRpc]

    private void AddPlayerToHUDClientRpc(ulong requestingPlayerId, ulong playerId, int health)
    {
        Debug.Log($"Intentando a�adir jugador {playerId} con {health} de vida para el cliente {requestingPlayerId}.");

        if (!playerListContainer)
        {
            Debug.LogError("playerListContainer no est� asignado.");
            return;
        }

        if (!playerLifePrefab)
        {
            Debug.LogError("playerLifePrefab no est� asignado.");
            return;
        }

        // Solo a�adir a la lista si no est� ya presente
        if (!playerLivesTexts.ContainsKey(playerId))
        {
            GameObject newLifeElement = Instantiate(playerLifePrefab, playerListContainer);
            TMP_Text lifeText = newLifeElement.GetComponentInChildren<TMP_Text>();

            if (lifeText == null)
            {
                Debug.LogError("El prefab no tiene un componente TMP_Text.");
                return;
            }

            // Asignar el texto de vida al jugador
            lifeText.text = $"Player {playerId}: {health}";

            // Guardar la referencia para actualizaciones posteriores
            playerLivesTexts.Add(playerId, lifeText);
        }
    }

    private void Update()
    {
        UpdateLivesDisplay();
    }

    // Actualiza las vidas en el UI
    private void UpdateLivesDisplay()
    {
        var players = FindObjectsOfType<AttibutesManager>();
        foreach (var player in players)
        {
            if (playerLivesTexts.ContainsKey(player.OwnerClientId))
            {
                playerLivesTexts[player.OwnerClientId].text = $"Player {player.OwnerClientId}: {player.health.Value}";
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerListServerRpc(ServerRpcParams rpcParams = default)
    {
        var players = FindObjectsOfType<AttibutesManager>();
        foreach (var player in players)
        {
            AddPlayerToHUDClientRpc(player.OwnerClientId, player.health.Value);
        }
    }

    [ClientRpc]
    private void AddPlayerToHUDClientRpc(ulong playerId, int health)
    {
        Debug.Log($"Intentando a�adir jugador {playerId} con {health} de vida.");

        if (!playerListContainer)
        {
            Debug.LogError("playerListContainer no est� asignado.");
            return;
        }

        if (!playerLifePrefab)
        {
            Debug.LogError("playerLifePrefab no est� asignado.");
            return;
        }

        if (!playerLivesTexts.ContainsKey(playerId))
        {
            GameObject newLifeElement = Instantiate(playerLifePrefab, playerListContainer);
            TMP_Text lifeText = newLifeElement.GetComponentInChildren<TMP_Text>();

            if (lifeText == null)
            {
                Debug.LogError("El prefab no tiene un componente TMP_Text.");
                return;
            }

            lifeText.text = $"Player {playerId}: {health}";

            // Guardar la referencia para actualizaciones posteriores
            playerLivesTexts.Add(playerId, lifeText);
        }
    }
}
