using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class AttibutesManager : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(3);
    public int attack = 1;
    public GameObject[] respawnPoints;
    private int deathCount = 0;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = 3; // Valor inicial de salud
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
            // Respawn del jugador (a través de un RPC que lo invoque el propietario)
            RespawnPlayerClientRpc();
        }
    }

    // RPC para el cliente propietario que invoca al servidor
    [ClientRpc]
    private void RespawnPlayerClientRpc()
    {
        // Solo el propietario invoca el ServerRpc
        if (IsOwner)
        {
            RespawnPlayer();  // Solo el propietario invoca el ServerRpc
        }
    }

    // ServerRpc para el servidor que respawnea al jugador
    [ServerRpc]
    private void RespawnPlayerServerRpc()
    {
        // Lógica de respawn solo ejecutada en el servidor
        RespawnPlayer();
    }


    [ClientRpc]
    private void DestroyPlayerClientRpc()
    {
        Debug.Log("Destruyendo el jugador en los clientes.");
        Destroy(gameObject);
    }
    private void RespawnPlayer()
    {
        // Restablecer la vida del jugador
        health.Value = 3;

        // Elegir un punto de respawn aleatorio
        if (respawnPoints.Length > 0)
        {
            // Seleccionamos un punto de respawn aleatorio
            GameObject randomRespawnPoint = respawnPoints[Random.Range(0, respawnPoints.Length)];
            gameObject.SetActive(false);
            // Restablecer la posición del jugador al punto de respawn aleatorio
            transform.position = randomRespawnPoint.transform.position;
            transform.rotation = randomRespawnPoint.transform.rotation; // Si también quieres alinear la rotación
        }
        else
        {
            Debug.LogError("No hay puntos de respawn asignados.");
        }

        // Asegurarse de que el jugador esté activo nuevamente
        gameObject.SetActive(true);
    }

}
