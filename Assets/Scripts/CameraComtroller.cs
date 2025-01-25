using UnityEngine;
using Unity.Netcode;
public class CameraController : NetworkBehaviour
{
    public GameObject bubble; // Objeto al que seguir� la c�mara
    private Vector3 offset; // Desplazamiento inicial entre la c�mara y la burbuja
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
        // Calcula el desplazamiento inicial entre la c�mara y la burbuja
        offset = transform.position - bubble.transform.position;
    }

    void LateUpdate()
    {
        // Mant�n la posici�n de la c�mara relativa a la burbuja
        Vector3 targetPosition = bubble.transform.position + offset;
        transform.position = targetPosition;

        // Haz que la c�mara siempre mire hacia la burbuja
        transform.LookAt(bubble.transform);
    }
}
