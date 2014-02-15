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


	AudioClip FetchClip (string type)
	{
		var list = clips.FindAll ((obj) => obj.key == type);
		if (list.Count == 0)
			return null;
		else if (list.Count > 1)
			return list[UnityEngine.Random.Range (0, list.Count - 1)].clip;
		return list[0].clip;
	}

	void PlayerDied ()
	{
		var clip = FetchClip ("hit");
		if (clip == null)
			return;
		audio.PlayOneShot (clip);
	}

	void PlayerJumped (string jumptype)
	{
		var clip = FetchClip ("jump");
		if (clip == null)
			return;
		audio.PlayOneShot (clip);
	}
}
