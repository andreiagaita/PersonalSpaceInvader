using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileReplacement {
	public Sprite sprite;
	public GameObject prefab;
}

public class Level : MonoBehaviour {
	
	public static Level instance;
	
	public List<GameObject> tiles;
	public GameObject defaultBrushPrefab;
	public List<Sprite> sprites;
	
	public TileReplacement[] replacements;
	
	public void Awake () {
		instance = this;
		Level.instance.ReplaceSpecials ();
	}
	
	public void ReplaceSpecials () {
		Dictionary<Sprite, GameObject> replacementDict = new Dictionary<Sprite, GameObject> ();
		foreach (var replacement in replacements)
			replacementDict[replacement.sprite] = replacement.prefab;
		
		
		foreach (var tile in tiles)
		{
			if (tile == null)
				continue;
			Sprite sprite = tile.GetComponent<SpriteRenderer> ().sprite;
			if (sprite == null)
				continue;
			if (replacementDict.ContainsKey (sprite)) {
				Vector3 pos = tile.transform.position;
				//tiles.Remove (tile);
				Destroy (tile);
				GameObject go = Instantiate (replacementDict[sprite], pos, Quaternion.identity) as GameObject;
				go.transform.parent = transform;
			}
		}
	}
}
