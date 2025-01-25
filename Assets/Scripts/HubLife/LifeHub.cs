using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class LifeHub : NetworkBehaviour
{
    public Image[] lifeIcons;

    private NetworkVariable<int> lives = new NetworkVariable<int>(
        3,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Start()
    {
        lives.OnValueChanged += (oldLives, newLives) =>
        {
            UpdateLivesUI(newLives);
        };
    }

    public override void OnNetworkSpawn()
    {
        UpdateLivesUI(lives.Value);
    }

    public void LoseLife()
    {
        Debug.Log("Into lostLife");
        //if (IsServer)
        //{
            lives.Value = Mathf.Max(lives.Value - 1, 0);
        //}
    }

    public void GainLife()
    {
        Debug.Log("Into GainLife");
        //if (IsServer)
        //{
            lives.Value = Mathf.Min(lives.Value + 1, lifeIcons.Length);
        //}
    }

    private void UpdateLivesUI(int currentLives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].enabled = i < currentLives;
        }
    }
}
