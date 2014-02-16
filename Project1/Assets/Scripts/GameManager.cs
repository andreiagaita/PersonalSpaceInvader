using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum PlayerColor
{
	Red = 0,
	Blue = 1,
	Green = 2,
	Orange = 3
}

public class GameManager : MonoBehaviour {

	public event Action<PlayerBehaviour> PlayerCreated;
	public event Action<PlayerBehaviour> PlayerDestroyed;
	public event Action LevelStart;
	public event Action ColorChangeWarning;
	public event Action GameEnded;

	static GameManager gameManager = null;
	public static GameManager instance {
		get { return gameManager; }
		private set {
			if (gameManager != null)
			{
				Destroy (value.gameObject);
				value.enabled = false;
			}
			else
			{
				gameManager = value;
				DontDestroyOnLoad (gameManager.gameObject);
			}
		}
	}
	
	public GameObject playerPrefab;
	public int playerCount = 4;
	public GameObject soundManagerPrefab;
	public GUIText scoreTextPrefab;
	public int scoreToWin = 20;
	[HideInInspector]
	public PlayerColor winningPlayerColor;
	private bool gameRoundEnded = false;
	
	public Dictionary<PlayerColor, int> scoreDict = new Dictionary<PlayerColor, int> ();
	public Dictionary<PlayerColor, GUIText> scoreTexts = new Dictionary<PlayerColor, GUIText> ();
	
	public Color[] playerColors;
	public float assignNewTargetsDelay = 20.0f;
	private bool aurasPulsating = false;
	private float timeSinceLastTargetReassign = 0f;
	private float pulsatingAuraNotificationLength = 3.0f;
	private int currentLevel = -1;

	public List<string> levels = new List<string> ();

	[HideInInspector]
	public List<GameObject> spawnPoints = new List<GameObject> ();
	[HideInInspector]
	public List<PlayerBehaviour> players = new List<PlayerBehaviour> ();
	[HideInInspector]
	public List<Transform> powerUpLocations = new List<Transform> ();
	public GameObject[] powerUps;
	private float powerUpDuration = 10f;
	private float timeSincePowerUpSpawn = 0f;
	private GameObject currentPowerUp;
	
	public void Awake () {
		var old = gameManager;
		instance = this;
		if (old == null)
			Instantiate (soundManagerPrefab);
	}

	public void Start ()
	{
		currentLevel = levels.FindIndex ((k) => Application.loadedLevelName == k);
		InitLevel ();
	}
	
	public void OnLevelWasLoaded (int level)
	{
		currentLevel = levels.FindIndex ((k) => Application.loadedLevelName == k);
		InitLevel ();
	}

	private void InitLevel ()
	{
		if (currentLevel < 0)
			return;

		gameRoundEnded = false;
		
		for (int i=0; i<playerCount; i++)
			Instantiate (playerPrefab);
		SpawnPlayers ();
		AssignTargets ();
		
		for (int i=0; i<players.Count; i++) {
			var player = players[i];
			GUIText text = Instantiate (scoreTextPrefab) as GUIText;
			text.pixelOffset = new Vector2 (0, 0.032f*Screen.height);
			text.fontSize = Mathf.RoundToInt (Screen.height * 0.03f);
			Color col = player.GetActualPlayerColor ();
			col.a = 0.7f;
			text.color = col;
			text.text = "0";
			
			scoreDict[player.playerColor] = 0;
			scoreTexts[player.playerColor] = text;
		}
	}

	public void Update()
	{
		for (int i=0; i<players.Count; i++) {
			Vector3 playerPos = players[i].transform.position;
			GUIText text = scoreTexts[players[i].playerColor];
			text.transform.position = new Vector3 ((playerPos.x + 0.5f) / 32f, (playerPos.y + 0.5f) / 24f);
		}
		
		if (currentLevel < 0)
			return;

		if (!gameRoundEnded)
		{
			timeSinceLastTargetReassign += Time.deltaTime;
			if (!aurasPulsating && (timeSinceLastTargetReassign > assignNewTargetsDelay - pulsatingAuraNotificationLength))
			{
				NotifyIncomingTargetReassignments();
			}

			if (timeSinceLastTargetReassign > assignNewTargetsDelay)
			{
				AssignTargets();
				DisablePulsatingAuraCircles();
				timeSinceLastTargetReassign = 0f;
			}

			timeSincePowerUpSpawn += Time.deltaTime;
			if (timeSincePowerUpSpawn > powerUpDuration)
			{
				var powerUpIndex = UnityEngine.Random.Range (0, powerUps.Length);
				var locationIndex = UnityEngine.Random.Range (0, powerUpLocations.Count);
				if (currentPowerUp != null)
					Destroy(currentPowerUp);
				currentPowerUp = Instantiate(powerUps[powerUpIndex], powerUpLocations[locationIndex].position, Quaternion.identity) as GameObject;
				timeSincePowerUpSpawn = 0;
				powerUpDuration = UnityEngine.Random.Range (5, 15);
			}
		}
	}

	public void AddPlayer (PlayerBehaviour player)
	{
		player.playerColor = (PlayerColor)players.Count;
		players.Add (player);
		if (PlayerCreated != null)
			PlayerCreated (player);
	}

	public void RemovePlayers ()
	{
		if (PlayerDestroyed != null)
		{
			foreach (var p in players)
				PlayerDestroyed (p);
		}
		players.Clear ();
	}

	public void AddSpawnPoint(GameObject spawnPoint)
	{
		spawnPoints.Add (spawnPoint);
	}

	public void RemoveSpawnPoints ()
	{
		spawnPoints.Clear();
	}

	public void AddPowerUpLocation (Transform powerUpLocation)
	{
		powerUpLocations.Add(powerUpLocation);
	}

	public void RemovePowerUpLocations ()
	{
		powerUpLocations.Clear();
	}
	
	public void SpawnPlayers () {
		List<GameObject> randomSpawnPoints = new List<GameObject> (spawnPoints);
		randomSpawnPoints.Shuffle ();
		foreach (var player in players) {
			player.transform.position = randomSpawnPoints[0].transform.position;
			randomSpawnPoints.RemoveAt (0);
		}
	}

	public void AssignTargets()
	{
		players.Shuffle();
		for (var i = 0; i < players.Count; ++i)
		{
			players[i].SetTarget (players[(i + 1) % players.Count]);
		}
	}
	
	public Vector3 GetSpawnPositionFurtestAway () {
		int spawnIndex = -1;
		float maxDist = 0;
		for (int i=0; i<spawnPoints.Count; i++) {
			float minDist = Mathf.Infinity;
			for (int j=0; j<players.Count; j++) {
				float dist = Vector2.Distance (players[j].transform.position, spawnPoints[i].transform.position);
				minDist = Mathf.Min (minDist, dist);
			}
			
			if (minDist > maxDist)
			{
				maxDist = minDist;
				spawnIndex = i;
			}
		}
		return spawnPoints[spawnIndex].transform.position;
	}
	
	public void AwardPointToPlayer (PlayerBehaviour player) {
		scoreDict[player.playerColor] += 1;
		scoreTexts[player.playerColor].text = "" + scoreDict[player.playerColor];
		if (scoreDict[player.playerColor] == scoreToWin)
		{
			winningPlayerColor = player.playerColor;
			ResetState ();
			RemovePlayers ();
			RemoveSpawnPoints();
			RemovePowerUpLocations ();
			GameEnd ();
		}
	}

	private void ResetState ()
	{
		gameRoundEnded = true;
		timeSinceLastTargetReassign = 0f;
	}

	private void NotifyIncomingTargetReassignments()
	{
		if (ColorChangeWarning != null)
			ColorChangeWarning ();
		for (var i = 0; i < players.Count; ++i)
			players[i].arrow.GetComponent<Pulsator>().enabled = true;
		aurasPulsating = true;
	}

	private void DisablePulsatingAuraCircles()
	{
		for (var i = 0; i < players.Count; ++i)
			players[i].arrow.GetComponent<Pulsator>().enabled = false;
		aurasPulsating = false;
	}

	void GameEnd ()
	{
		currentLevel = -1;
		if (GameEnded != null)
			GameEnded ();
		Application.LoadLevel("EndGameMenu");
	}

	public void NextLevel ()
	{
		currentLevel = UnityEngine.Random.Range (0, levels.Count);

		if (LevelStart != null)
			LevelStart ();

		Application.LoadLevel (levels[currentLevel]);
	}
}
