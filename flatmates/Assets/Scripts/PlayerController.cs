using UnityEngine;
using XInputDotNetPure; // Required in C#

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	public int playerNumber;

	// xinput
	bool playerIndexSet = false;
	PlayerIndex playerIndex;
	GamePadState state;
	GamePadState prevState;

	void Update ()
	{
		// Find a PlayerIndex
		if (!playerIndexSet || !prevState.IsConnected)
		{
			PlayerIndex testPlayerIndex = (PlayerIndex)playerNumber;
			GamePadState testState = GamePad.GetState (testPlayerIndex);
			if (testState.IsConnected)
			{
				Debug.Log (string.Format ("GamePad found {0}", testPlayerIndex));
				playerIndex = testPlayerIndex;
				playerIndexSet = true;
			}
			else
				return;
		}		

		state = GamePad.GetState (playerIndex);

		float horizontal = state.ThumbSticks.Left.X;
		float vertical = state.ThumbSticks.Left.Y;

		prevState = state;
	}

	void OnDisable()
	{
		OnApplicationQuit();
	}

	void OnDestroy ()
	{
		OnApplicationQuit ();
	}

	void OnApplicationQuit ()
	{
		GamePad.SetVibration (playerIndex, 0, 0);
	}
}
