using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class AttibutesManager : NetworkBehaviour
{
    // La salud es manejada como una NetworkVariable
    public NetworkVariable<int> health = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private static List<int> usedSpawnIndices = new List<int>();
    public int attack = 1;
    public GameObject[] respawnPoints;
    private int deathCount = 0;
    public override void OnNetworkSpawn()
    {
        if (IsServer) // Host y servidor manejan el spawn inicial
        {
            AssignUniqueSpawnPointServerRpc(); // Aseguramos que la lógica pase por el servidor
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AssignUniqueSpawnPointServerRpc(ServerRpcParams rpcParams = default)
    {
        // Resetea la vida y ejecuta el respawn
        health.Value = 3; // Esto sincroniza automáticamente la salud con los clientes
        Debug.Log($"Reseteando la salud del jugador {gameObject.name} a {health.Value}");
        int spawnIndex = GetAvailableSpawnIndex();
        RespawnInitialPlayer(spawnIndex); // Lógica de respawn en el servidor

        // Notifica a los clientes que el jugador ha respawneado
        RespawnInitialPlayerClientRpc(transform.position, transform.rotation);
    }

    [ClientRpc]
    private void RespawnInitialPlayerClientRpc(Vector3 newPosition, Quaternion newRotation)
    {
        Debug.Log($"El jugador {gameObject.name} ha respawneado en la posición {newPosition}");

        // Actualiza la posición y rotación del jugador en el cliente
        transform.position = newPosition;
        transform.rotation = newRotation;

        // Asegurarse de que el jugador esté activo
        gameObject.SetActive(true);
    }

    private void RespawnInitialPlayer(int spawnIndex)
    {
        // El servidor elige un punto de respawn
        if (respawnPoints.Length > 0)
        {

            // Ajusta la posición y rotación del jugador
            transform.position = respawnPoints[spawnIndex].transform.position;
            transform.rotation = respawnPoints[spawnIndex].transform.rotation;
        }
        else
        {
            Debug.LogError("No hay puntos de respawn asignados.");
        }

        // Asegurarse de que el jugador esté activo nuevamente
        gameObject.SetActive(true);
    }

    private int GetAvailableSpawnIndex()
    {
        // Recorre los puntos de spawn y encuentra uno que no esté usado
        for (int i = 0; i < respawnPoints.Length; i++)
        {
            if (!usedSpawnIndices.Contains(i))
            {
                usedSpawnIndices.Add(i); // Marca este punto como usado
                return i; // Devuelve el índice disponible
            }
        }
        return -1; // Si no hay puntos disponibles, devuelve -1
    }

    public override void OnNetworkDespawn()
    {
        // Cuando un jugador abandona el juego, libera su punto de spawn
        ReleaseSpawnPoint();
    }

    private void ReleaseSpawnPoint()
    {
        // Encuentra el índice del punto de spawn más cercano al jugador
        for (int i = 0; i < respawnPoints.Length; i++)
        {
            if (Vector3.Distance(transform.position, respawnPoints[i].transform.position) < 1f) // Margen de error para comparar
            {
                usedSpawnIndices.Remove(i); // Libera el punto de spawn
                Debug.Log($"Punto de spawn {i} liberado por el jugador {OwnerClientId}");
                break;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (!IsServer) return; // Solo el servidor puede modificar la salud

        health.Value -= amount;

        if (health.Value <= 0)
        {
            Debug.Log($"{gameObject.name} ha muerto.");
            StartCoroutine(HandlePlayerDeath());
        }
    }

    public void DealDamage(GameObject target)
    {
        if (!IsServer) return; // Solo el servidor puede infligir daño
        var targetAttributes = target.GetComponent<AttibutesManager>();
        if (targetAttributes != null)
        {
            targetAttributes.TakeDamage(attack);
        }
    }

    private IEnumerator HandlePlayerDeath()
    {
        // Espera 1 segundo antes del respawn
        yield return new WaitForSeconds(1f);

        // Aumenta el contador de muertes
        deathCount++;

        if (deathCount >= 3)
        {
            // Si el personaje ha muerto 3 veces, lo destruye
            DestroyPlayerClientRpc();
        }
        else
        {
            // El servidor controla el respawn
            RespawnPlayerServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RespawnPlayerServerRpc()
    {
        // Resetea la vida y ejecuta el respawn
        health.Value = 3; // Esto sincroniza automáticamente la salud con los clientes
        Debug.Log($"Reseteando la salud del jugador {gameObject.name} a {health.Value}");

        RespawnPlayer(); // Lógica de respawn en el servidor

        // Notifica a los clientes que el jugador ha respawneado
        RespawnPlayerClientRpc(transform.position, transform.rotation);
    }

    [ClientRpc]
    private void RespawnPlayerClientRpc(Vector3 newPosition, Quaternion newRotation)
    {
        Debug.Log($"El jugador {gameObject.name} ha respawneado en la posición {newPosition}");

        // Actualiza la posición y rotación del jugador en el cliente
        transform.position = newPosition;
        transform.rotation = newRotation;

        // Asegurarse de que el jugador esté activo
        gameObject.SetActive(true);
    }

    [ClientRpc]
    private void DestroyPlayerClientRpc()
    {
        Debug.Log("Destruyendo el jugador en los clientes.");
        Destroy(gameObject);
    }

    private void RespawnPlayer()
    {
        // El servidor elige un punto de respawn
        if (respawnPoints.Length > 0)
        {
            GameObject randomRespawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];

            // Ajusta la posición y rotación del jugador
            transform.position = randomRespawnPoint.transform.position;
            transform.rotation = randomRespawnPoint.transform.rotation;
        }
        else
        {
            Debug.LogError("No hay puntos de respawn asignados.");
        }

        // Asegurarse de que el jugador esté activo nuevamente
        gameObject.SetActive(true);
    }
}
