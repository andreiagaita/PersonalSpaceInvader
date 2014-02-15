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
	public bool useNetwork = false;

    bool ndPlayerJoin = false;

    private SpawnPoint[] LevelSpawns;

    void Start()
    {
        base.Start();

		if (useNetwork)
			Subscribe ("Network", "OnJoinedRoom", JoinSelfPlayer);
		else
			JoinSelfPlayer();
    }

	public void JoinSelfPlayer ()
	{

		int playerID = 1;
		if (useNetwork)
			playerID = PhotonNetwork.player.ID;

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
		GameObject player = null;
		if (useNetwork)
			player = PhotonNetwork.Instantiate(PlayerPrefab.name, spawnLocation.transform.position, Quaternion.identity, 0);
		else
		
			player = Instantiate (PlayerPrefab, spawnLocation.transform.position, Quaternion.identity) as GameObject;

		player.name = "Player" + PhotonNetwork.player.ID;
        spawnLocation.Available = false;

		player.GetComponentInChildren<SpriteRenderer>().color = currentPlayerInfo.Color;
		PlayerController controller = player.GetComponentInChildren<PlayerController>();
        controller.controller =  PlayerController.ControllerType.Keyboard;
        controller.playerNumber = currentPlayerInfo.ID;
    }

    void Update()
    {
        if (fake2Player && Time.realtimeSinceStartup > 2 && !ndPlayerJoin)
        {
            SpawnPoint spawnLocation = GetRandomSpawn();
            PlayerJoined(2, "Player2", spawnLocation.transform.position, Color.green, 0);
            spawnLocation.Available = false;
            Debug.Log("2nd player created");
            ndPlayerJoin = true;
        }
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
