using UnityEngine;
using Unity.Netcode;
public class CameraController : NetworkBehaviour
{
    public GameObject bubble; // Objeto al que seguirá la cámara
    private Vector3 offset; // Desplazamiento inicial entre la cámara y la burbuja
   /* public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }*/
    void Start()
    {
        // Calcula el desplazamiento inicial entre la cámara y la burbuja
        offset = transform.position - bubble.transform.position;
    }

    void LateUpdate()
    {
        // Mantén la posición de la cámara relativa a la burbuja
        Vector3 targetPosition = bubble.transform.position + offset;
        transform.position = targetPosition;

        // Haz que la cámara siempre mire hacia la burbuja
        transform.LookAt(bubble.transform);
    }
}
