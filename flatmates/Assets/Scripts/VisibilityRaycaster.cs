using UnityEngine;
using System.Collections;

public class VisibilityRaycaster : MonoBehaviour
{

	public LayerMask occluderLayer;

	void Update () {

		BlackBoard.WriteInstant(name, "PublicPosition", transform.position);

		for (int i = 1; i < 3; i++)
		{
			if (name == "Player" + i)
				continue;

			Vector3 position = BlackBoard.Read<Vector3>("Player" + i, "PublicPosition");

			if (IsPlayerVisible(position))
			{
				Dispatcher.SendMessage("Player" + i, "PlayerIsVisible", name);
				Dispatcher.SendMessage(name, "DidSawPlayer", "Player" + i);

				Debug.Log(name + " saw player: " + i + " : " + position);
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
