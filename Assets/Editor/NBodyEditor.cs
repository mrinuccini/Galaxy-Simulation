using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NBodySimulation))]
public class NBodyEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		NBodySimulation simulation = (NBodySimulation)target;

		if(GUILayout.Button("Generate Galaxy")) 
		{
			simulation.CreateGalaxy();
		}

		if(GUILayout.Button("Clear Galaxy")) 
		{
			simulation.ClearGalaxyEditor();
		}
	}
}
