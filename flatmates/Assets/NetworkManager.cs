using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
	public GameStateManager gameStateManager;

	void OnJoinedRoom()
	{
		PhotonView photonView = PhotonView.Get(this);
		PlayerInfo myself = gameStateManager.CreateMySelf (PhotonNetwork.player.ID);
		photonView.RPC("SetPlayerInfo", PhotonTargets.Others, myself.Name, myself.Position,myself.Score);
    }

	void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		gameStateManager.PlayerJoined (newPlayer.ID, "Player " + newPlayer.ID, Vector3.zero, Color.green, 0);
		PhotonView photonView = PhotonView.Get(this);
		photonView.RPC("SetPlayerInfo", PhotonPlayer.Find(newPlayer.ID), gameStateManager.currentPlayerInfo.Name, gameStateManager.currentPlayerInfo.Position, gameStateManager.currentPlayerInfo.Score);
	}

	[RPC]
	private void SetPlayerInfo(string playerName, Vector3 position/*, Color color*/, int score, PhotonMessageInfo messageInfo)
	{
		Debug.Log("GotPlayerInfo:" + playerName);
		gameStateManager.SetPlayerInfo(messageInfo.sender.ID, playerName, position, Color.green, score);
	}
}
