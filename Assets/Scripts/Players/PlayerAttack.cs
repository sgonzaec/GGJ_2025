using UnityEngine;
using Unity.Netcode;

public class PlayerAttack : NetworkBehaviour
{
    public KeyCode attackKey = KeyCode.Space; // Tecla de ataque.
    public float attackRange = 5f; // Distancia máxima para atacar.
    public int attackDamage = 1; // Daño del ataque.

    private void Update()
    {
        if (!IsOwner) return; // Asegurarse de que solo el jugador local pueda atacar.

        if (Input.GetKeyDown(attackKey))
        {
            AttemptAttackServerRpc();
        }
    }

    [ServerRpc]
    private void AttemptAttackServerRpc(ServerRpcParams rpcParams = default)
    {
        // Encontrar todos los jugadores en la escena.
        var players = FindObjectsOfType<PlayerAttack>();

        foreach (var player in players)
        {
            if (player != this) // No atacar al jugador que está ejecutando el ataque.
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= attackRange)
                {
                    player.TakeDamageServerRpc(attackDamage);
                }
            }
        }
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int damage)
    {
        var lifeHub = GetComponent<LifeHub>();
        if (lifeHub != null)
        {
            lifeHub.LoseLife();
        }
    }
}
