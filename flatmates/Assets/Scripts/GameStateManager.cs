using System;
using System.Collections;
using System.Linq;
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

	public PlayerInfo GetPlayerByID (int id)
	{
		return players[id];
	}

	void Start()
	{
		base.Start();

		players = new Dictionary<int, PlayerInfo> ();
		Debug.Log("Waiting for host to join game... Press A to join");
	}

	void OnEnable ()
	{
		LevelSpawns = GameObject.FindObjectsOfType<SpawnPoint>();
		if (LevelSpawns == null || LevelSpawns.Length == 0)
		{
			Debug.LogError("No spawn points found in the level");
		}
		else
		{
			Debug.Log(LevelSpawns.Length + " spawn points found");
		}
	}

	private void JoinSelfPlayer(int playerID)
	{
		if (useNetwork)
			playerID = PhotonNetwork.player.ID;

		m_CurrentPlayerInfo = CreateMySelf(playerID, true);
		Debug.Log("host player created");

		SpawnPoint spawnLocation = GetRandomSpawn();
        if (spawnLocation == null)
            return;

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
                    if (spawnLocation == null)
                        return;

					PlayerJoined(playerID, "Player" + playerID, spawnLocation.transform.position, Color.green, 0);
					spawnLocation.Available = false;
					Debug.Log("player " + playerID + "created");
				}
			}
		}

		if (useNetwork && Input.anyKeyDown && m_CurrentPlayerInfo != null && m_CurrentPlayerInfo.currentState == PlayerInfo.PlayerState.Connected)
		{
			int controllerID = FindControllerID();
			if (controllerID != -1)
			{
				m_CurrentPlayerInfo.controllerID = controllerID;
				SetPlayerReady (m_CurrentPlayerInfo.ID);
				Dispatcher.SendMessage ("Player", "Ready", m_CurrentPlayerInfo.ID);
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

		SpawnPlayer (newPlayer);
	}

	SpawnPoint GetRandomSpawn()
	{
        if (!LevelSpawns.Any(x => x.Available))
        {
            Debug.LogError("No Spawn Locations found!!!");
            return null;
        }
		SpawnPoint sp = LevelSpawns[UnityEngine.Random.Range(0, LevelSpawns.Length)];
		while(!sp.Available)
		{
			sp = LevelSpawns[UnityEngine.Random.Range(0, LevelSpawns.Length)];
		}
		return sp;
	}

	public void RegisterNewPlayer(int id, string name)
	{
		var player = new PlayerInfo(id, name, Vector3.zero, Color.black, 0, false);
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

	public ClientPlayerInfo CreateMySelf(int playerID, bool isMaster)
	{
		m_CurrentPlayerInfo = new ClientPlayerInfo (playerID, "Player" + playerID, Vector3.zero, Color.red, 0, isMaster);
		players.Add(playerID, m_CurrentPlayerInfo);
		return m_CurrentPlayerInfo;
	}

	public void SetPlayerReady (int playerId)
	{
		PlayerInfo player = players[playerId];
		player.currentState = PlayerInfo.PlayerState.Ready;

		bool allPlayersReady = true;
		foreach (KeyValuePair<int, PlayerInfo> playerTuple in players)
		{
			if (playerTuple.Value.currentState != PlayerInfo.PlayerState.Ready)
			{
				allPlayersReady = false;
				break;
			}
		}
		if (allPlayersReady)
			OnAllPlayersReady ();
	}

	private void OnAllPlayersReady ()
	{
		if (!useNetwork)
			throw new NotImplementedException();

		Debug.Log ("currentPlayer " + currentPlayerInfo.ID);
		Debug.Log ("IsMaster? " + currentPlayerInfo.isMaster);
		if (currentPlayerInfo.isMaster)
		{
			SetupWorld ();
		}
	}

	private void SetupWorld ()
	{
		//Setup item spawn points
        itemManager.InitItems(players.Values.ToList());

		//setup player spawn points
		SetupPlayerSpawnPoints ();

		Dispatcher.SendMessage("World", "Ready");
		SetWorldReady ();
	}

	//Sets up all players initial spawn points
	private void SetupPlayerSpawnPoints ()
	{
		foreach (KeyValuePair<int, PlayerInfo> playerTuple in players)
		{
			SpawnPoint spawnLocation = GetRandomSpawn();
            if (spawnLocation == null)
                continue;
			PlayerInfo player = playerTuple.Value;
			player.Position = spawnLocation.transform.position;

			Dispatcher.SendMessage("Player", "SetSpawnPoint", player);

			spawnLocation.Available = false;
		}
	}

	public void SetWorldReady ()
	{
		foreach (KeyValuePair<int, PlayerInfo> playerTuple in players)
		{
			PlayerInfo player = playerTuple.Value;
			player.currentState = PlayerInfo.PlayerState.Playing;
			SpawnPlayer (player);
		}
	}

	private void SpawnPlayer (PlayerInfo player)
	{
		Debug.Log ("Spawning Player: " + player.ID);
		GameObject playerGO = (GameObject)Instantiate(PlayerPrefab, player.Position, Quaternion.identity);
		playerGO.GetComponentInChildren<SpriteRenderer>().color = player.Color;
		playerGO.name = "Player" + player.ID;

		player.gameObject = playerGO;

		ClientPlayerInfo localPlayer = player as ClientPlayerInfo;

		PlayerController controller = playerGO.GetComponentInChildren<PlayerController>();

		if (localPlayer != null)
		{
			controller.controller = localPlayer.controllerID > 4 ? PlayerController.ControllerType.Keyboard : PlayerController.ControllerType.Xbox;
			controller.controllerID = localPlayer.controllerID;
			controller.playerID = player.ID;
		}
		else
		{
			controller.enabled = false;
		}
	}

	public void MoveRemotePlayer (int id, Vector3 position)
	{
		PlayerInfo player = GetPlayerByID (id);
		player.Position = position;
		player.gameObject.transform.position = position;
	}
}
