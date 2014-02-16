using UnityEngine;
using System.Collections;

public class VisibilityRaycaster : MonoBehaviour
{

	public LayerMask occluderLayer;

	private PlayerInfo m_Myself;

	void Update () {
		if (m_Myself == null)
		{
			m_Myself = GameStateManager.Instance.GetPlayerByGameObject(gameObject);
		}

		foreach (PlayerInfo player in GameStateManager.Instance.GetPlayersDict ().Values)
		{
			if (gameObject == player.gameObject)
				continue;

			Vector3 position = player.gameObject.transform.position;

			if (IsPlayerVisible(position))
			{
				Dispatcher.SendMessage(player.gameObject.name, "PlayerIsVisible", m_Myself.ID);
				Dispatcher.SendMessage(name, "DidSawPlayer", player.gameObject.name);
			}
		}
	}

	private bool IsPlayerVisible(Vector3 position)
	{
		Vector3 direction = position - transform.position;
		float distance = Vector3.Distance(transform.position, position);

		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, occluderLayer);
	
		return !hit;
	}
}
