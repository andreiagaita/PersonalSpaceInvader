using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateManager : GameScript
{
    private Time GameStartTime;
    private ClientPlayerInfo currentPlayerInfo;

    void Start()
    {
        base.Start();

        // create myself
        currentPlayerInfo = new ClientPlayerInfo(PlayerID.Player1, PlayerID.Player1.ToString(), Vector3.zero, Color.red, 0);
    }

    void PlayerJoined(PlayerID id, string name, Vector3 position, Color color, int score)
    {
        Debug.Log("Player " + id + " just joined the game");
        PlayerInfo newPlayer = new PlayerInfo(id, name, position, color, score);
        currentPlayerInfo.AddOpponent(newPlayer);
    }

    void PlayerLeft(PlayerID id)
    {
        Debug.Log("Player " +  id + " just left the game");
        currentPlayerInfo.RemoveOpponent(id);
    }
}
