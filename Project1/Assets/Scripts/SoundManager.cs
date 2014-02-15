using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

	[Serializable]
	public class AudioKeyValue
	{
		public string key;
		public AudioClip clip;
	}

	public List<AudioKeyValue> clips;

	static SoundManager soundManager = null;
	public static SoundManager instance {
		get { return soundManager; }
		private set {
			if (soundManager != null)
			{
				Destroy (value.gameObject);
				value.enabled = false;
			}
			else
			{
				soundManager = value;
				DontDestroyOnLoad (soundManager.gameObject);
			}
		}
	}

	// Use this for initialization
	void Awake () {
		instance = this;
		GameManager.instance.PlayerCreated += HookupPlayerSounds;
		GameManager.instance.LevelStart += LevelStart;
		GameManager.instance.ColorChangeWarning += ColorWarning;
		GameManager.instance.GameEnded += GameEnded;
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

	void PlayClip (string type)
	{
		var list = clips.FindAll ((obj) => obj.key == type);
		if (list.Count == 0)
			return;
		var clip = list.Count == 1 ? list[0].clip : list[UnityEngine.Random.Range (0, list.Count - 1)].clip;
		audio.PlayOneShot (clip);
	}

	void PlayerDied ()
	{
		PlayClip ("hit");
	}

	void PlayerJumped (string jumptype)
	{
		PlayClip ("jump");
	}

	void LevelStart ()
	{
		PlayClip ("start");
	}

	void ColorWarning ()
	{
		PlayClip ("warning");
	}

	void GameEnded ()
	{
		audio.Stop ();
		PlayClip ("end");
	}
}
