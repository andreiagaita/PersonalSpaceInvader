using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor (typeof (Level))]
public class LevelEditor : Editor {

	int selectedTile = -1;
	Sprite selectedSprite { get {
		if (selectedTile < 0)
			return null;
		return level.sprites[selectedTile];
	} }
	
	Vector2 lastMousePos = Vector2.zero;
	Level level;
	
	public void OnEnable () {
		level = target as Level;
		//SceneView.onSceneGUIDelegate += OnSceneGUICustom;
	}
	
	public void OnDisable () {
		//SceneView.onSceneGUIDelegate -= OnSceneGUICustom;
	}
	
	public override void OnInspectorGUI ()
	{
		if (GUILayout.Toggle (selectedTile == -1, "NONE"))
			selectedTile = -1;
		
		int tileSize = 32 + 2;
		
		Rect rect = EditorGUILayout.GetControlRect ();
		rect.width = Screen.width - 40;
		int cols = Mathf.FloorToInt (rect.width / tileSize);
		int rows = Mathf.CeilToInt (level.sprites.Count / (float)cols);
		
		GUILayoutUtility.GetRect (cols*tileSize, rows*tileSize);
		for (int i=0; i<level.sprites.Count; i++) {
			float x = (i % cols) * tileSize + rect.x;
			float y = (i / cols) * tileSize + rect.y;
			Rect tileRect = new Rect (x, y, tileSize-2, tileSize-2);
			Texture tex = level.sprites[i].texture;
			Rect uv = level.sprites[i].textureRect;
			uv.width /= tex.width;
			uv.x /= tex.width;
			uv.height /= tex.height;
			uv.y /= tex.height;
			
			if (GUI.Button (tileRect, GUIContent.none, GUIStyle.none))
				selectedTile = i;
			
			if (selectedTile == i)
				GUI.DrawTexture (new Rect (tileRect.x-1, tileRect.y-2, tileSize+2, tileSize+2), EditorGUIUtility.whiteTexture);
			
			GUI.DrawTextureWithTexCoords (tileRect, tex, uv);
		}
		
		DrawDefaultInspector ();
	}
	
	public void OnSceneGUI () {
		MySceneGUI ();
	}
	
	public void OnPreSceneGUI () {
		MySceneGUI ();
	}
	
	public void MySceneGUI () {
		Tools.current = Tool.None;
		
		Vector2 mousePos = GUIToGridPoint (Event.current.mousePosition);
		DrawCell (mousePos);
		
		int id = GUIUtility.GetControlID (FocusType.Passive);
		
		if (Event.current.type == EventType.MouseDown) {
			SetTile (mousePos, selectedSprite);
			EditorGUIUtility.hotControl = id;
		}
		if (Event.current.type == EventType.MouseUp) {
			if (EditorGUIUtility.hotControl == id)
				EditorGUIUtility.hotControl = 0;
		}
		if (Event.current.type == EventType.MouseDrag) {
			if (EditorGUIUtility.hotControl == id) {
				if (mousePos != lastMousePos) {
					lastMousePos = mousePos;
					SetTile (mousePos, selectedSprite);
				}
			}
		}
		
		if (Event.current.type == EventType.MouseMove)
			Event.current.Use ();
	}
	
	void SetTile (Vector2 pos, Sprite sprite) {
		for (int i=level.tiles.Count-1; i>=0; i--) {
			GameObject go = level.tiles[i];
			if (go == null) {
				level.tiles.RemoveAt (i);
				continue;
			}
			if (go.transform.position == (Vector3)pos) {
				level.tiles.RemoveAt (i);
				DestroyImmediate (go);
				continue;
			}
		}	
		
		if (sprite != null) {
			GameObject instance = PrefabUtility.InstantiatePrefab (level.defaultBrushPrefab) as GameObject;
			instance.transform.position = pos;
			instance.transform.parent = level.transform;
			instance.GetComponent<SpriteRenderer> ().sprite = sprite;
			level.tiles.Add (instance);
		}
		Event.current.Use ();
	}
	
	void DrawCell (Vector3 pos) {
		Handles.DrawLine (pos + new Vector3 (1,1,0) * 0.5f, pos + new Vector3 (-1,1,0) * 0.5f);
		Handles.DrawLine (pos + new Vector3 (-1,1,0) * 0.5f, pos + new Vector3 (-1,-1,0) * 0.5f);
		Handles.DrawLine (pos + new Vector3 (-1,-1,0) * 0.5f, pos + new Vector3 (1,-1,0) * 0.5f);
		Handles.DrawLine (pos + new Vector3 (1,-1,0) * 0.5f, pos + new Vector3 (1,1,0) * 0.5f);
	}
	
	Vector2 GUIToGridPoint (Vector2 guiPoint) {
		Plane plane = new Plane (-Vector3.forward, Vector3.zero);
		Ray ray = HandleUtility.GUIPointToWorldRay (guiPoint);
		float dist = 0;
		plane.Raycast (ray, out dist);
		Vector2 point = ray.GetPoint (dist);
		point.x = Mathf.Round (point.x);
		point.y = Mathf.Round (point.y);
		return point;
	}
}
