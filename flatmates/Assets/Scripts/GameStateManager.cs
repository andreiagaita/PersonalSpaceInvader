using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : GameScript
{
	private Time GameStartTime;

	private Dictionary<int, PlayerInfo> players;

	private ClientPlayerInfo m_CurrentPlayerInfo;
    public ItemManager itemManager;

	public GameObject PlayerPrefab;
	public bool useNetwork = false;

	private SpawnPoint[] LevelSpawns;

	public ClientPlayerInfo currentPlayerInfo
	{ 
		get { return m_CurrentPlayerInfo; }
	}

	void Start()
	{
		base.Start();

		players = new Dictionary<int, PlayerInfo> ();
		Debug.Log("Waiting for host to join game... Press A to join");
	}

	private void JoinSelfPlayer(int playerID)
	{
		if (useNetwork)
			playerID = PhotonNetwork.player.ID;

		m_CurrentPlayerInfo = CreateMySelf(playerID);
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

		Debug.Log("player: " + player);
		player.name = "Player" + playerID;
		spawnLocation.Available = false;

		player.GetComponentInChildren<SpriteRenderer>().color = m_CurrentPlayerInfo.Color;
		PlayerController controller = player.GetComponentInChildren<PlayerController>();
		controller.controller = m_CurrentPlayerInfo.ID > 4 ? PlayerController.ControllerType.Keyboard : PlayerController.ControllerType.Xbox;
		controller.controllerID = m_CurrentPlayerInfo.ID;

		Debug.Log("Waiting for other players to join game... Press A to join");
	}

	void Update()
	{
		if(!useNetwork && Input.anyKeyDown)
		{
			int playerID = FindControllerID();
			if (playerID != -1)
			{
				if (m_CurrentPlayerInfo == null)
				{
					JoinSelfPlayer(playerID);
				}
				else if (m_CurrentPlayerInfo.ID != playerID && !m_CurrentPlayerInfo.OpponentPlayers.ContainsKey(playerID))
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
			//Debug.Log("checking: " + keyCode + " : " + Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), keyCode)));
			if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), keyCode)))
				return i;
		}
		if (Input.GetKeyDown(KeyCode.Return))
			return 5;
		return controllerID;
	}

	public void PlayerJoined(int id, string name, Vector3 position, Color color, int score)
	{
		
		Debug.Log("Player " + id + " just joined the game");
		RegisterNewPlayer(id, name);
		PlayerInfo newPlayer = SetPlayerInfo(id, name, position, color, score);
		
		GameObject ndplayerTransform = (GameObject)GameObject.Instantiate(PlayerPrefab, newPlayer.Position, Quaternion.identity);
		ndplayerTransform.GetComponentInChildren<SpriteRenderer>().color = newPlayer.Color;
		PlayerController controller = ndplayerTransform.GetComponentInChildren<PlayerController>();
		controller.controller = newPlayer.ID > 4 ? PlayerController.ControllerType.Keyboard : PlayerController.ControllerType.Xbox;
		controller.controllerID = newPlayer.ID;
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

	public void RegisterNewPlayer(int id, string name)
	{
		var player = new PlayerInfo(id, name, Vector3.zero, Color.black, 0);
		players.Add(id, player);
		m_CurrentPlayerInfo.AddOpponent(player);
	}

	public PlayerInfo SetPlayerInfo(int id, string playerName, Vector3 position, Color color, int score)
	{
		if (!players.ContainsKey (id))
		{
			RegisterNewPlayer (id, playerName);
		}
		PlayerInfo player = players[id];
		player.Name = playerName;
		player.Position = position;
		player.Color = color;
		player.Score = score;
		return player;
	}

	public ClientPlayerInfo CreateMySelf(int playerID)
	{
		m_CurrentPlayerInfo = new ClientPlayerInfo(playerID, "Player" + playerID, Vector3.zero, Color.red, 0);
		return m_CurrentPlayerInfo;
	}
}
