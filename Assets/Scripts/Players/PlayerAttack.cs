using UnityEngine;
using Unity.Netcode;

public class PlayerAttack : NetworkBehaviour
{
    public KeyCode attackKey = KeyCode.Space;
    public float attackRange = 5f;
    public int attackDamage = 1;

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(attackKey))
        {
            AttemptAttackServerRpc();
        }
    }

    [ServerRpc]
    private void AttemptAttackServerRpc(ServerRpcParams rpcParams = default)
    {
        var players = FindObjectsOfType<PlayerAttack>();


        foreach (var player in players)
        {
            if (player != this)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= attackRange)
                {
                    Debug.Log("Atacando!");
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
