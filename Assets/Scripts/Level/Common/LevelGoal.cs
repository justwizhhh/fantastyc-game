using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    // ------------------------------
    //
    //   Class for defining the goal point all players will need to reach in gameplay mode
    //
    //   Created: 04/06/2024
    //
    // ------------------------------

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Keep track of every player who has reached the end
        if (collision.GetComponent<PlayerGameplay>() != null)
        {
            PlayerGameplay player = collision.GetComponent<PlayerGameplay>();

            player.stateMachine.ChangeState(typeof(PlayerWin));
            GameController.instance.UpdateWinningPlayers(player);
        }
    }
}
