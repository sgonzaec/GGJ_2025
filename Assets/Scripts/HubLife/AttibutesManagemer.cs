using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class AttibutesManager : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(3);
    public int attack = 1;

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
        if (!IsServer) return; // Solo el servidor puede infligir da�o
        var targetAttributes = target.GetComponent<AttibutesManager>();
        if (targetAttributes != null)
        {
            targetAttributes.TakeDamage(attack);
        }
    }

    private IEnumerator HandlePlayerDeath()
    {
        // Notifica a los clientes que borren el coraz�n
        RemoveHeartClientRpc();

        // Espera 1 segundo (ajusta seg�n el tiempo necesario para borrar el coraz�n)
        yield return new WaitForSeconds(0.1f);

        // Destruye el jugador
        DestroyPlayerClientRpc();
    }

    [ClientRpc]
    private void RemoveHeartClientRpc()
    {
        // Aqu� puedes implementar la l�gica para borrar el coraz�n
        Debug.Log("Borrando coraz�n en los clientes.");
        // Ejemplo: Animaci�n de quitar coraz�n o esconder el objeto visualmente
        // Puedes interactuar con un componente UI o activar una animaci�n espec�fica.
    }

    [ClientRpc]
    private void DestroyPlayerClientRpc()
    {
        Debug.Log("Destruyendo el jugador en los clientes.");

        // Aqu� puedes a�adir efectos visuales adicionales, como una explosi�n
        // Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Destruir el GameObject en todos los clientes
        Destroy(gameObject);
    }
}
