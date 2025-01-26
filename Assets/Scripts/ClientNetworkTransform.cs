using Unity.Netcode.Components;
using UnityEngine;


[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

    /*public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log("Player spawned: " + OwnerClientId);
        }
    }*/
}
