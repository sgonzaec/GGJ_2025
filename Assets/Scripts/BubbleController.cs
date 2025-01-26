using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;

public class BubbleController : NetworkBehaviour
{
    public float speed = 5f; // Velocidad del movimiento
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float rotationThreshold = 0.1f; // Umbral mínimo para la rotación.

    float gravedad = -9.8f;
    Vector3 movement;
    [SerializeField] CharacterController pJ;//Rerencia del character controller del personaje

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
    //{
    //    // Crear un vector de movimiento basado en los inputs.
         movement = new Vector3(movementX, 0.0f, movementY);

        //    // Mover al jugador.
        //    if (rb != null)
        //    {
        //        rb.AddForce(movement * speed);
        //    }

        //    // Rotar al jugador hacia la dirección del movimiento.
            RotateCharacter(movement);

        //Gravedad();
        pJ.Move(movement*speed);
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

    void Gravedad()
    {
        if(movement.y >= 0.1)
            movement.y = gravedad*Time.deltaTime;   
    }
}
