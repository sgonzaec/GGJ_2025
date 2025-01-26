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
        if (!IsServer) return; // Solo el servidor puede infligir daño
        var targetAttributes = target.GetComponent<AttibutesManager>();
        if (targetAttributes != null)
        {
            targetAttributes.TakeDamage(attack);
        }
    }

    private IEnumerator HandlePlayerDeath()
    {
        // Notifica a los clientes que borren el corazón
        RemoveHeartClientRpc();

        // Espera 1 segundo (ajusta según el tiempo necesario para borrar el corazón)
        yield return new WaitForSeconds(0.1f);

        // Destruye el jugador
        DestroyPlayerClientRpc();
    }

    [ClientRpc]
    private void RemoveHeartClientRpc()
    {
        // Aquí puedes implementar la lógica para borrar el corazón
        Debug.Log("Borrando corazón en los clientes.");
        // Ejemplo: Animación de quitar corazón o esconder el objeto visualmente
        // Puedes interactuar con un componente UI o activar una animación específica.
    }

    [ClientRpc]
    private void DestroyPlayerClientRpc()
    {
        Debug.Log("Destruyendo el jugador en los clientes.");

        // Aquí puedes añadir efectos visuales adicionales, como una explosión
        // Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Destruir el GameObject en todos los clientes
        Destroy(gameObject);
    }
}
