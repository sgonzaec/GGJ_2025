using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    public Transform[] respawnPoints; // Lista de puntos de respawn
    private bool isRespawning = false; // Evita múltiples respawns simultáneos

    public void Respawn()
    {
        if (isRespawning) return; // Evita reentradas

        isRespawning = true;

        // Elegir un punto de respawn al azar
        int randomIndex = Random.Range(0, respawnPoints.Length);
        Transform newRespawnPoint = respawnPoints[randomIndex];

        // Mover al enemigo al nuevo punto
        transform.position = newRespawnPoint.position;

        // Resetear respawn después de un breve delay para evitar glitches
        Invoke(nameof(ResetRespawn), 0.1f);
    }

    private void ResetRespawn()
    {
        isRespawning = false;
    }
}
