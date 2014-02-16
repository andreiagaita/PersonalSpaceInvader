using UnityEngine;
using System.Collections;

public class EyeController : MonoBehaviour {
	
	public Transform eyeOpen;
	public Transform eyeClosed;
	public Transform arrow;
	
	// Use this for initialization
	void Start () {
		StartCoroutine ("Blinker");
	}
	
	// Update is called once per frame
	void LateUpdate () {
		eyeOpen.rotation = arrow.rotation;
		if (transform.localScale.x < 0)
			eyeOpen.rotation = Quaternion.Euler (0, 180, 0) * eyeOpen.rotation;
	}
	
	IEnumerator Blinker () {
		while (true) {
			yield return new WaitForSeconds (Random.Range (0.5f, 5.0f));
			eyeOpen.GetComponent<SpriteRenderer> ().enabled = false;
			eyeClosed.GetComponent<SpriteRenderer> ().enabled = true;
			yield return new WaitForSeconds (0.05f);
			eyeOpen.GetComponent<SpriteRenderer> ().enabled = true;
			eyeClosed.GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
}
	
	

