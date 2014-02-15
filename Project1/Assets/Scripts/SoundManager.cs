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
		if (instance == null)
			instance = this;
	}

	public void Start () {

	}
}
