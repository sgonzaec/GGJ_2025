using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class BubbleController : NetworkBehaviour
{
    public float speed = 5f; // Velocidad del movimiento
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float rotationThreshold = 0.1f; // Umbral mínimo para la rotación.

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void FixedUpdate()
    {
        // Crear un vector de movimiento basado en los inputs.
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        // Mover al jugador.
        if (rb != null)
        {
            rb.AddForce(movement * speed);
        }

        // Rotar al jugador hacia la dirección del movimiento.
        RotateCharacter(movement);
    }

    private void RotateCharacter(Vector3 movement)
    {
        // Comprobar si el vector de movimiento es lo suficientemente grande.
        if (movement.magnitude > rotationThreshold)
        {
            // Calcular la nueva rotación basada en el movimiento.
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            // Interpolar suavemente la rotación actual hacia la nueva rotación.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Normalizar la dirección y aplicar la fuerza
            Vector3 knockbackDirection = direction.normalized;
            rb.AddForce(knockbackDirection * force, ForceMode.Impulse);
        }
    }

    public void HitPlayer(GameObject attacker, float knockbackForce)
    {
        // Obtener la dirección del golpe
        Vector3 direction = transform.position - attacker.transform.position;

        // Aplicar knockback
        ApplyKnockback(direction, knockbackForce);
    }
}
