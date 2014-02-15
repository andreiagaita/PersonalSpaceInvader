using UnityEngine;
using System.Collections;

public class PickupItem : GameScript
{
	public string owner;
	private bool isCollected;
	private string holder;

	void Start()
	{
		base.Start();
	}

	void OnTriggerEnter2D (Collider2D hit)
	{
		if (hit.gameObject.layer == LayerMask.NameToLayer("ItemCollector"))
			Pickup(hit.transform.parent.name);
	}

	void OnTriggerStay2D(Collider2D hit)
	{
		if (hit.gameObject.layer == LayerMask.NameToLayer("ItemCollector"))
			Pickup(hit.transform.parent.name);
	}

	private void Pickup (string playerName)
	{
		if (isCollected || playerName == owner)
			return;

		holder = playerName;
		SendMessage("PickupObject", "PlayerDidPickupItem", playerName, name);
		isCollected = true;

		Subscribe(holder, "PlayerIsVisible", Drop);
		Subscribe(holder, "DidEnterRoom", DidEnterRoom);
	}

	private void Drop (Subscription subscription)
	{
		string opponentName = subscription.Read<string>(0);

		if (opponentName != owner)
			return;

		subscription.UnSubscribe();

		isCollected = false;
	}

	void DidEnterRoom(Subscription subscription)
	{
		subscription.UnSubscribe();

		isCollected = false;

		Destroy(gameObject);
	}

	private void Update()
	{
		if (!isCollected)
			return;

		transform.position = BlackBoard.Read<Vector3>(holder, "PublicPosition") + Vector3.up * 2;
	}
}
