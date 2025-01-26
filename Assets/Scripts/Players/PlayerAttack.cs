using UnityEngine;
using Unity.Netcode;

public class PlayerAttack : NetworkBehaviour
{
    public KeyCode attackKey = KeyCode.Space;
    public float attackRange = 5f;

    private void Update()
    {
        if (!IsOwner) return; // Solo el jugador propietario puede atacar
        if (Input.GetKeyDown(attackKey))
        {
            AttemptAttackServerRpc();
        }
    }

    [ServerRpc]
    private void AttemptAttackServerRpc(ServerRpcParams rpcParams = default)
    {
        // Obtiene todos los jugadores en la escena
        var players = FindObjectsOfType<AttibutesManager>();

        foreach (var player in players)
        {
            if (player != this.GetComponent<AttibutesManager>()) // Evitar que un jugador se ataque a sí mismo
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= attackRange)
                {
                    Debug.Log($"{gameObject.name} está atacando a {player.gameObject.name}");
                    GetComponent<AttibutesManager>().DealDamage(player.gameObject);
                }
            }
        }
    }
}
