using UnityEngine;
using Unity.Netcode;

public class AttibutesManager : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public int attack = 10;

    public void TakeDamage(int amount)
    {
        if (!IsServer) return; // Solo el servidor puede modificar la salud
        health.Value -= amount;

        if (health.Value <= 0)
        {
            Debug.Log($"{gameObject.name} ha muerto.");
            // Aquí puedes implementar lógica de muerte, como desactivar el objeto
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
}
