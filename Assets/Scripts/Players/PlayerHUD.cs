using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Netcode;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public GameObject playerLifePrefab; // Prefab de UI para mostrar la vida de cada jugador
    public Transform playerListContainer; // Contenedor en el UI donde se añadirán los elementos de vida

    private Dictionary<ulong, TMP_Text> playerLivesTexts = new Dictionary<ulong, TMP_Text>();

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            UpdatePlayerListServerRpc();
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
