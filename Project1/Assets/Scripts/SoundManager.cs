using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	static SoundManager soundManager = null;
	public static SoundManager instance {
		get { return soundManager; }
		private set {
			if (soundManager != null)
				Destroy (soundManager.gameObject);
			soundManager = value;
			DontDestroyOnLoad (soundManager.gameObject);
		}
	}
	// Use this for initialization
	void Awake () {
		instance = this;
		GameManager.instance.PlayerCreated += HookupPlayerSounds;
	}

	void HookupPlayerSounds (PlayerBehaviour player)
	{
		player.Died += PlayerDied;
		player.GetComponent<CharacterController>().Jumped += PlayerJumped;
	}

	void UnhookPlayerSounds (PlayerBehaviour player)
	{
		player.Died -= PlayerDied;
		player.GetComponent<CharacterController>().Jumped -= PlayerJumped;
	}

	void PlayerDied ()
	{

	}

	void PlayerJumped (string jumptype)
	{
	}
}
