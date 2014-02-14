using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor (typeof (Level))]
public class LevelEditor : Editor {

	int selectedBrush = 0;
	GameObject selectedBrushPrefab
	{
		get
		{
			if (selectedBrush < 1)
				return null;
			return (target as Level).brushPrefabs[selectedBrush-1];
		}
	}
	
	public override void OnInspectorGUI ()
	{
		Level level = target as Level;
		
		// Brush toolbar
		string[] brushNames = new string[level.brushPrefabs.Count + 1];
		brushNames[0] = "Empty";
		for (int i=0; i<level.brushPrefabs.Count; i++)
			brushNames[i+1] = level.brushPrefabs[i].name;
		selectedBrush = GUILayout.Toolbar (selectedBrush, brushNames);
		
		DrawDefaultInspector ();
	}
	
	public void OnSceneGUI () {
		Vector2 mousePos = GUIToGridPoint (Event.current.mousePosition);
		DrawCell (mousePos);
		if (Event.current.type == EventType.MouseMove)
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
