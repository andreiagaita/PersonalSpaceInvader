using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : GameScript
{
    private Time GameStartTime;
    private ClientPlayerInfo currentPlayerInfo;
    private Dictionary<Guid, PickupObject> GamePickups = new Dictionary<Guid, PickupObject>();

    public GameObject PlayerPrefab;
	public bool fake2Player = true;

    private SpawnPoint[] LevelSpawns;

    void Start()
    {
        base.Start();

	    //Subscribe ("Network", "OnJoinedRoom", JoinSelfPlayer);

		//JoinSelfPlayer();

        Debug.Log("Waiting for host to join game... Press A to join");
    }

	public void JoinSelfPlayer (int playerID)
	{

        currentPlayerInfo = new ClientPlayerInfo(playerID, "Player" + playerID, Vector3.zero, Color.red, 0);
        Debug.Log("host player created");

        LevelSpawns = GameObject.FindObjectsOfType<SpawnPoint>();
        if(LevelSpawns == null || LevelSpawns.Length == 0)
        {
            Debug.LogError("No spawn points found in the level");
        }
        else
        {
            Debug.Log(LevelSpawns.Length + " spawn points found");
        }

        SpawnPoint spawnLocation = GetRandomSpawn();
        GameObject player = PhotonNetwork.Instantiate(PlayerPrefab.name, spawnLocation.transform.position, Quaternion.identity, 0);
        spawnLocation.Available = false;

		player.GetComponentInChildren<SpriteRenderer>().color = currentPlayerInfo.Color;
		PlayerController controller = player.GetComponentInChildren<PlayerController>();
        controller.controller =  PlayerController.ControllerType.Xbox;
        controller.playerNumber = currentPlayerInfo.ID;

        Debug.Log("Waiting for other players to join game... Press A to join");
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            int playerID = FindControllerID();
            if (playerID != -1)
            {
                if (currentPlayerInfo == null)
                {
                    JoinSelfPlayer(playerID);
                }
                else if (currentPlayerInfo.ID != playerID && !currentPlayerInfo.OpponentPlayers.ContainsKey(playerID))
                {
                    SpawnPoint spawnLocation = GetRandomSpawn();
                    PlayerJoined(playerID, "Player" + playerID, spawnLocation.transform.position, Color.green, 0);
                    spawnLocation.Available = false;
                    Debug.Log("player " + playerID + "created");
                }
            }
        }
    }

    int FindControllerID()
    {
        int controllerID = -1;
        for (int i = 1; i < 4; i++)
        {
            string keyCode = "Joystick" + (i == 0 ? "" : i.ToString()) + "Button0";
            Debug.Log("checking: " + keyCode + " : " + Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), keyCode)));
            if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), keyCode)))
                return i;
        }
        return controllerID;
    }

    void PlayerJoined(int id, string name, Vector3 position, Color color, int score)
    {
        Debug.Log("Player " + id + " just joined the game");
        PlayerInfo newPlayer = new PlayerInfo(id, name, position, color, score);
        currentPlayerInfo.AddOpponent(newPlayer);
        Transform ndplayerTransform = (Transform)GameObject.Instantiate(PlayerPrefab, newPlayer.Position, Quaternion.identity);
        ndplayerTransform.GetComponentInChildren<SpriteRenderer>().color = newPlayer.Color;
        PlayerController controller = ndplayerTransform.GetComponentInChildren<PlayerController>();
        controller.controller = PlayerController.ControllerType.Xbox;
        controller.playerNumber = newPlayer.ID;
    }

    void PlayerLeft(int id)
    {
        Debug.Log("Player " +  id + " just left the game");
        currentPlayerInfo.RemoveOpponent(id);
    }

    SpawnPoint GetRandomSpawn()
    {
        SpawnPoint sp = LevelSpawns[UnityEngine.Random.Range(0, LevelSpawns.Length)];
        while(!sp.Available)
        {
            sp = LevelSpawns[UnityEngine.Random.Range(0, LevelSpawns.Length)];
        }
        return sp;
    }
}
