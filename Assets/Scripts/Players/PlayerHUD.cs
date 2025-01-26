using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Netcode;
using TMPro;
using System.Linq;
using System;

public class PlayerHUD : MonoBehaviour
{
    public GameObject playerLifePrefab; // Prefab de UI para mostrar la vida de cada jugador
    public Transform playerListContainer; // Contenedor en el UI donde se añadirán los elementos de vida

    private Dictionary<ulong, TMP_Text> playerLivesTexts = new Dictionary<ulong, TMP_Text>();
    private Dictionary<ulong, CanvasGroup[]> playerLivesHearts = new Dictionary<ulong, CanvasGroup[]>();

    private void Start()
    {
        Debug.Log($"PlayerHUD activo: {gameObject.name}");
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager no está configurado.");
            return;
        }
        // Si es cliente, añadir a sí mismo al HUD
        if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Cliente conectado, notificando al servidor.");
            NotifyServerOfConnectionServerRpc();
            AddPlayerToHUDClientRpc(NetworkManager.Singleton.LocalClientId, 3);
            Debug.Log("Es cliente. Suscribiéndose al evento OnClientConnectedCallback.");
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
            Debug.Log("Es el servidor. Suscribiéndose al evento OnClientConnectedCallback.");
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
            Debug.Log("No es el servidor. No se suscribirá al evento.");
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
            // Después de que un jugador se conecta, notificarle la lista completa de jugadores
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
            // Nota: Asegúrate de que no le envíes su propia barra de vida (aunque esto ya se maneja por el ClientRpc)
            AddPlayerToHUDClientRpc(newPlayerId, player.OwnerClientId, player.health.Value);
        }
    }
    [ClientRpc]

    private void AddPlayerToHUDClientRpc(ulong requestingPlayerId, ulong playerId, int health)
    {
        Debug.Log($"Intentando añadir jugador {playerId} con {health} de vida para el cliente {requestingPlayerId}.");

        if (!playerListContainer)
        {
            Debug.LogError("playerListContainer no está asignado.");
            return;
        }

        if (!playerLifePrefab)
        {
            Debug.LogError("playerLifePrefab no está asignado.");
            return;
        }

        Debug.Log($"Ya esta en la lista? {playerLivesTexts.ContainsKey(playerId)} ");
        // Solo añadir a la lista si no está ya presente
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

            // Obtener los CanvasGroup de los corazones
            CanvasGroup[] heartCanvasGroups = newLifeElement.GetComponentsInChildren<CanvasGroup>();

            Debug.Log($"Cambas group{heartCanvasGroups.Length}");

            // Asegúrate de que haya tres CanvasGroup (uno por cada corazón)
            if (heartCanvasGroups.Length == 3)
            {
                // Cambiar la visibilidad u opacidad de los corazones
                for (int i = 0; i < 3; i++)
                {
                    Debug.Log($"indice {i}");
                    Debug.Log($"health {health}");
                    if (i >= health) // Si el índice es mayor o igual a la vida, desactiva el corazón
                    {
                        //heartCanvasGroups[i].gameObject.SetActive(false);
                        heartCanvasGroups[i].alpha = 0.3f; // Hacerlo más transparente
                        // O si prefieres desactivarlo completamente, puedes usar:
                        // heartCanvasGroups[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        heartCanvasGroups[i].alpha = 1f; // Hacerlo completamente visible
                                                         // O si prefieres activarlo de nuevo, puedes usar:
                                                         // heartCanvasGroups[i].gameObject.SetActive(true);
                    }
                }
            }

            // Forzar la actualización del layout para evitar solapamientos
            LayoutRebuilder.ForceRebuildLayoutImmediate(playerListContainer.GetComponent<RectTransform>());
            Array.Reverse(heartCanvasGroups);
            // Guardar la referencia para actualizaciones posteriores
            playerLivesTexts.Add(playerId, lifeText);
            playerLivesHearts.Add(playerId, heartCanvasGroups);
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
        List<ulong> playersToRemove = new List<ulong>(); // Lista para jugadores que han muerto

        foreach (var player in players)
        {
            if (playerLivesTexts.ContainsKey(player.OwnerClientId))
            {
                playerLivesTexts[player.OwnerClientId].text = $"Player {player.OwnerClientId}: {player.health.Value}";
            }

            if (playerLivesHearts.ContainsKey(player.OwnerClientId))
            {
                for (int i = 0; i < playerLivesHearts[player.OwnerClientId].Length; i++)
                {
                    if (i >= player.health.Value)
                    {
                        playerLivesHearts[player.OwnerClientId][i].alpha = 0.3f; // Más transparente
                    }
                    else
                    {
                        playerLivesHearts[player.OwnerClientId][i].alpha = 1f; // Totalmente visible
                    }
                }
            }

            // Si la vida es 0, marcar para eliminar
            if (player.health.Value <= 0)
            {
                playersToRemove.Add(player.OwnerClientId);
            }
        }

        // Eliminar las barras de vida de los jugadores muertos
        foreach (var playerId in playersToRemove)
        {
            RemovePlayerFromHUD(playerId);
        }
    }

    private void RemovePlayerFromHUD(ulong playerId)
    {
        if (playerLivesTexts.ContainsKey(playerId))
        {
            // Destruir el objeto de UI
            TMP_Text lifeText = playerLivesTexts[playerId];
            Destroy(lifeText.transform.parent.gameObject);

            // Eliminar de los diccionarios
            playerLivesTexts.Remove(playerId);
            playerLivesHearts.Remove(playerId);

            Debug.Log($"Jugador {playerId} eliminado del HUD.");
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
        Debug.Log($"Intentando añadir jugador {playerId} con {health} de vida.");

        if (!playerListContainer)
        {
            Debug.LogError("playerListContainer no está asignado.");
            return;
        }

        if (!playerLifePrefab)
        {
            Debug.LogError("playerLifePrefab no está asignado.");
            return;
        }

        if (!playerLivesTexts.ContainsKey(playerId))
        {
            // Instanciar la nueva barra de vida
            GameObject newLifeElement = Instantiate(playerLifePrefab, playerListContainer);
            TMP_Text lifeText = newLifeElement.GetComponentInChildren<TMP_Text>();

            if (lifeText == null)
            {
                Debug.LogError("El prefab no tiene un componente TMP_Text.");
                return;
            }

            lifeText.text = $"Player {playerId}: {health}";

            // Obtener los CanvasGroup de los corazones
            CanvasGroup[] heartCanvasGroups = newLifeElement.GetComponentsInChildren<CanvasGroup>();
            Debug.Log($"Cambas group{heartCanvasGroups.Length}");
            // Asegúrate de que haya tres CanvasGroup (uno por cada corazón)
            if (heartCanvasGroups.Length == 3)
            {
                // Cambiar la visibilidad u opacidad de los corazones
                for (int i = 0; i < 3; i++)
                {
                    Debug.Log($"indice {i}");
                    Debug.Log($"health {health}");
                    if (i >= health) // Si el índice es mayor o igual a la vida, desactiva el corazón
                    {
                        //heartCanvasGroups[i].gameObject.SetActive(false);
                        heartCanvasGroups[i].alpha = 0.3f; // Hacerlo más transparente
                        // O si prefieres desactivarlo completamente, puedes usar:
                        // heartCanvasGroups[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        heartCanvasGroups[i].alpha = 1f; // Hacerlo completamente visible
                                                         // O si prefieres activarlo de nuevo, puedes usar:
                                                         // heartCanvasGroups[i].gameObject.SetActive(true);
                    }
                }
            }

            // Forzar la actualización del layout para evitar solapamientos
            LayoutRebuilder.ForceRebuildLayoutImmediate(playerListContainer.GetComponent<RectTransform>());
            Array.Reverse(heartCanvasGroups);
            // Guardar la referencia para actualizaciones posteriores
            playerLivesTexts.Add(playerId, lifeText);
            playerLivesHearts.Add(playerId, heartCanvasGroups);
        }
    }
}
