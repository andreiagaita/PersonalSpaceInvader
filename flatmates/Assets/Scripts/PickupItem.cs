using UnityEngine;
using System.Collections;

public class PickupItem : PickupObject
{
	//public string owner;
	private bool isCollected;
	//private string holder;

	void Start()
	{
		base.Start();
	}

	void OnTriggerEnter2D (Collider2D hit)
	{
		if (hit.gameObject.layer == LayerMask.NameToLayer("ItemCollector"))
            Pickup(hit.transform.parent.gameObject);
	}

	void OnTriggerStay2D(Collider2D hit)
	{
		if (hit.gameObject.layer == LayerMask.NameToLayer("ItemCollector"))
			Pickup(hit.transform.parent.gameObject);
	}

	private void Pickup (GameObject playerObject)
	{
        int playerID = playerObject.GetComponent<PlayerController>().playerID;
        if (isCollected || playerID == Owner)
			return;

        Holder = playerID;
        SendMessage("PickupObject", "PlayerDidPickupItem", playerID, name);
		isCollected = true;

		Subscribe("Player" + Holder, "PlayerIsVisible", Drop);
        Subscribe("Player" + Holder, "DidEnterRoom", DidEnterRoom);
	}

	private void Drop (Subscription subscription)
	{
		int opponentID = subscription.Read<int>(0);

        if (opponentID != Owner)
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

        transform.position = BlackBoard.Read<Vector3>("Player" + Holder, "PublicPosition") + Vector3.up * 2;
	}
}
