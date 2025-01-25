using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class BubbleController : NetworkBehaviour
{
    public float speed = 0;
    private Rigidbody rb;
    private float movementX;
    private float movementY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { 
            enabled = false; 
            return;
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        if (rb != null) {
            rb.AddForce(movement * speed);
        }
    }
}
