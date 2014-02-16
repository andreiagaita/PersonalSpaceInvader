using UnityEngine;
using System.Collections;

public class Pulsator : MonoBehaviour {
	public float duration = 0.25f;
	private Color colorStart;
	private Color colorEnd;
	private float startTime;

	void Start () {
		colorStart = GetComponent<SpriteRenderer>().color;
		colorEnd = Color.white;
		startTime = 0;
	}
	
	void Update () {
		if (startTime == 0)
			startTime = Time.time;
		var lerp = Mathf.PingPong((Time.time - startTime) * 2, duration);
		GetComponent<SpriteRenderer>().color = Color.Lerp(colorStart, colorEnd, lerp);
	}
}
