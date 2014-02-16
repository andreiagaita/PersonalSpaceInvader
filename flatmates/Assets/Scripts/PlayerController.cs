using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
internal class PlayerController : MonoBehaviour
{
	public float force;
	public float speed;

	public enum ControllerType { None, Xbox, Keyboard }
	public ControllerType controller;

    public int controllerID = 0;
    public int playerID = 0;
	public AnimationClip myAnimation;
	private bool AnimationSet = false;

	private Transform myTransform;
	private Transform sprite;

	private GameStateManager gameController;

	public Texture2D ScoreTexture;
	public Texture2D WantTexture;

	private PickupItem Item1;
	private PickupItem Item2;

	void OnEnable ()
	{
		myTransform = GetComponent<Transform> ();
		sprite = transform.Find("Sprite");

		gameController = GameStateManager.Instance;
		foreach(PickupItem item in GameStateManager.Instance.itemManager.ItemDatabase.Values)
		{
			if(item.ObjectiveForPlayer == playerID)
			{
				if (item.ObjectiveIndex == 0)
					Item1 = item;
				else
					Item2 = item;
			}
		}

		// applying a proper animation

	}

	
	public void Update()
	{
        //base.Update();

        //if (controller == ControllerType.Xbox && !controllerReady)
        //    return;

		float horizontal = 0;
		float vertical = 0;

		switch (controller)
		{
			case ControllerType.Keyboard:
                horizontal = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
                vertical = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0);
				break;

			case ControllerType.Xbox:
                //horizontal = state.ThumbSticks.Left.X;
                //vertical = state.ThumbSticks.Left.Y;
                horizontal = Input.GetAxis("P" + controllerID + "Horizontal");
                vertical = Input.GetAxis("P" + controllerID + "Vertical");
				break;
		}

		rigidbody2D.AddForce(new Vector2(horizontal * force, vertical * force));
		rigidbody2D.velocity = new Vector2(Mathf.Clamp(rigidbody2D.velocity.x, -speed, speed), Mathf.Clamp(rigidbody2D.velocity.y, -speed, speed));

		// flip the player sprite when going left/right
		if (horizontal < 0) {
			sprite.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
		}
		else {
			sprite.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
		}

		if (myAnimation != null && !AnimationSet)
		{
			Animator myAnimator = GetComponent<Animator>();
			myAnimator.Play(myAnimation.name);
			AnimationSet = true;
		}
	}

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(0, Screen.height - 40, 111, 29), ScoreTexture);

		Rect wantarea = new Rect(Screen.width - 100, Screen.height - 50, 94, 41);
		GUI.DrawTexture(wantarea, WantTexture);

		Sprite itemSprite = Item1.IsStolen ? Item2.GetComponent<SpriteRenderer>().sprite : Item1.GetComponent<SpriteRenderer>().sprite;
		Texture2D wantItem = itemSprite.texture;
		Rect ItemRect = new Rect(wantarea.x + 65, wantarea.y + 13, 17, 14);
		Rect textCoords = new Rect(itemSprite.rect.x / wantItem.width, itemSprite.rect.y/ wantItem.height,
			itemSprite.rect.width / wantItem.width, itemSprite.rect.height / wantItem.height);
		GUI.DrawTextureWithTexCoords(ItemRect, wantItem, textCoords, true);

	}

	public void FixedUpdate ()
	{
		Dispatcher.SendMessage("Player", "Moved", playerID, myTransform.position);
	}
}
