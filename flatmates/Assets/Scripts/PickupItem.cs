using UnityEngine;
using System.Collections;

public class PickupItem : PickupObject
{
	private Vector3 m_PlayerOffset;

	void Start()
	{
		m_PlayerOffset = new Vector3(.6f,1.8f,0);
		base.Start();
	}

	void OnTriggerEnter2D (Collider2D hit)
	{
		if (hit.gameObject.layer == LayerMask.NameToLayer("ItemCollector"))
            OnPickup(hit.transform.parent.gameObject);
	}

	void OnTriggerStay2D(Collider2D hit)
	{
		if (hit.gameObject.layer == LayerMask.NameToLayer("ItemCollector"))
			OnPickup(hit.transform.parent.gameObject);
	}

	private void OnPickup (GameObject playerObject)
	{
        int playerID = playerObject.GetComponent<PlayerController>().playerID;
        if (IsCollected || playerID == Owner)
			return;

		PickUp (playerID);
        SendMessage("PickupObject", "PlayerDidPickupItem", playerID, ID);
	}

	public void PickUp (int playerID)
	{
		Holder = playerID;
		IsCollected = true;

		Subscribe("Player" + Holder, "PlayerIsVisible", Drop);
		Subscribe("Player" + Holder, "DidEnterRoom", DidEnterRoom);
	}

	private void Drop (Subscription subscription)
	{
		int opponentID = subscription.Read<int>(0);

        if (opponentID != Owner)
			return;

		subscription.UnSubscribe();

		PlayerInfo player = GameStateManager.Instance.GetPlayerByID (Holder);
		transform.position = transform.position - m_PlayerOffset;

		IsCollected = false;
		Holder = 0;
		
	}

	void DidEnterRoom(Subscription subscription)
	{
		subscription.UnSubscribe();

		IsCollected = false;

		//Destroy(gameObject);
		gameObject.SetActive (false);
	}

	private void Update()
	{
		if (!IsCollected)
			return;

		if (Holder == 0)
			return;

		PlayerInfo player = GameStateManager.Instance.GetPlayerByID (Holder);

		transform.position = player.gameObject.transform.position + m_PlayerOffset;
	}
}
