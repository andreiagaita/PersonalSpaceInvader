using UnityEngine;
using System.Collections;

public class PlayerRoom : MonoBehaviour 
{
	public string owner;

	void OnTriggerEnter2D(Collider2D hit)
	{
		if (hit.name != owner)
			return;

		if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
			Dispatcher.SendMessage(hit.name, "DidEnterRoom");
	}
}
