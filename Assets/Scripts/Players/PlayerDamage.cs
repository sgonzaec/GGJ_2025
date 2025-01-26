using Unity.Netcode;
using UnityEngine;

public class PlayerDamage : NetworkBehaviour
{
    public AttibutesManager playerAtm;
    public AttibutesManager enemyAtm;
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12)) 
        {
            playerAtm.DealDamage(enemyAtm.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            enemyAtm.DealDamage(playerAtm.gameObject);
        }
    }
}
