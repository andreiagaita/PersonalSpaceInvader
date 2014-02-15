using UnityEngine;
using System.Collections;

public class Pulsator : MonoBehaviour {
	public float duration = 1.0f;
	private Color colorStart;
	private Color colorEnd;

	void Start () {
		colorStart = colorEnd = Color.white;
		colorStart.a = 0;
	}
	
	void Update () {
		var lerp = Mathf.PingPong(Time.time, duration) / duration;
		GetComponent<SpriteRenderer>().color = Color.Lerp(colorStart, colorEnd, lerp);
	}
}
