using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
	public GameStateManager gameStateManager;

	private PhotonView photonView;

	private void Start ()
	{
		photonView = PhotonView.Get(this);

		Dispatcher.Subscribe ("Player",  "Ready",         OnLocalPlayerReady);
		Dispatcher.Subscribe ("Player",  "SetSpawnPoint", OnSpawnPointSet);
		Dispatcher.Subscribe ("World",   "Ready",         OnWorldReady);
		Dispatcher.Subscribe ("Player",  "Moved",          OnPlayerMove);
	}

	void OnJoinedRoom()
	{
		bool isMaster = PhotonNetwork.room.playerCount == 1;
		Debug.Log ("Is master: " + isMaster);
		PlayerInfo myself = gameStateManager.CreateMySelf (PhotonNetwork.player.ID, isMaster);
		photonView.RPC("SetPlayerInfo", PhotonTargets.Others, myself.Name, myself.Position,myself.Score);
    }

	void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		gameStateManager.RegisterNewPlayer(newPlayer.ID, "Player " + newPlayer.ID);
		photonView.RPC("SetPlayerInfo", PhotonPlayer.Find(newPlayer.ID), gameStateManager.currentPlayerInfo.Name, gameStateManager.currentPlayerInfo.Position, gameStateManager.currentPlayerInfo.Score);
	}

	[RPC]
	private void SetPlayerInfo(string playerName, Vector3 position/*, Color color*/, int score, PhotonMessageInfo messageInfo)
	{
		Debug.Log("GotPlayerInfo:" + playerName);
		gameStateManager.SetPlayerInfo(messageInfo.sender.ID, playerName, position, Color.green, score);
	}

	private void OnLocalPlayerReady(Subscription subscription)
	{
		int playerId = subscription.Read<int> (0);
		photonView.RPC ("SetPlayerReady", PhotonTargets.Others);
	}

	[RPC]
	void SetPlayerReady (PhotonMessageInfo messageInfo)
	{
		int playerId = messageInfo.sender.ID;
		Debug.Log (playerId + " is ready!");
		gameStateManager.SetPlayerReady (playerId);
	}

	private void OnSpawnPointSet(Subscription subscription)
	{
		PlayerInfo player = subscription.Read<PlayerInfo>(0);
		photonView.RPC("SetPlayerSpawnPoint", PhotonTargets.Others, player.ID, player.Position);
	}

	[RPC]
	void SetPlayerSpawnPoint(int playerId, Vector3 position, PhotonMessageInfo messageInfo)
	{
		Debug.Log(playerId + " wll spawn at "+ position);
		PlayerInfo player = gameStateManager.GetPlayerByID (playerId);
		player.Position = position;
	}

	private void OnWorldReady(Subscription subscription)
	{
		photonView.RPC("SetWorldReady", PhotonTargets.Others);
	}

	[RPC]
	void SetWorldReady(PhotonMessageInfo messageInfo)
	{
		Debug.Log(messageInfo.sender.ID + " says the world is ready ");
		gameStateManager.SetWorldReady();
	}

	private void OnPlayerMove(Subscription subscription)
	{
		//Debug.Log ("Send Player Move");
		//int playerId = subscription.Read<int>(0);
		Vector3 position = subscription.Read<Vector3> (1);
		photonView.RPC("MovePlayer", PhotonTargets.Others, position);
	}

	[RPC]
	void MovePlayer(Vector3 position, PhotonMessageInfo messageInfo)
	{
		//Debug.Log ("Received a Player Move");
		gameStateManager.MoveRemotePlayer(messageInfo.sender.ID, position);
	}

}
